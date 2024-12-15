C1541 = c1541
AS = acme
# deploy 1571 (d71) or 1581 (d81); e.g. make DISK_SUF=d81 deploy
DISK_SUF = d64

TAG := $(shell git describe --tags --abbrev=0 || svnversion --no-newline)
TAG_DEPLOY_DOT := $(shell git describe --tags --long --dirty=_m | sed 's/-g[0-9a-f]\+//' | tr _- -.)
TAG_DEPLOY := $(shell git describe --tags --abbrev=0 --dirty=_M | tr _. -_)
GIT_HASH := $(shell git rev-parse --short HEAD)

DEPLOY_NAME = durexforth-$(TAG_DEPLOY)
DISK_IMAGE = durexforth.$(DISK_SUF)

X64_DEPLOY_OPTS = -warp -debugcart -limitcycles 2000000000
X64 = x64sc
PETCAT = petcat # text conversion utility, included in VICE package

SRC_DIR = forth
SRC_NAMES = base debug v asm gfx gfxdemo rnd sin ls turtle fractals \
    sprite doloop sys labels mml mmldemo sid spritedemo \
    format require compat timer float viceutil turnkey \
    wordlist io open dos see accept exception
SRCS = $(addprefix $(SRC_DIR)/,$(addsuffix .fs,$(SRC_NAMES)))

TEST_SRC_NAMES = test testcore testcoreplus testcoreext testexception tester testsee 1
TEST2_SRC_NAMES = see gfx gfxdemo fractals mmldemo mml sid spritedemo sprite compat rnd sin turtle
TEST_SRCS = $(addprefix test/,$(addsuffix .fs,$(TEST_SRC_NAMES)))

SEPARATOR_NAME1 = '=-=-=-=-=-=-=-=,s'
SEPARATOR_NAME2 = '=-------------=,s'
SEPARATOR_NAME3 = '=-=---=-=---=-=,s'

all: $(DISK_IMAGE)

deploy: $(DISK_IMAGE) asm/cart.asm $(TEST_SRCS)
	python asm/header.py $(wildcard asm/*.asm) # verify .asm headers
	rm -rf deploy
	mkdir deploy
	cp $(DISK_IMAGE) deploy/$(DEPLOY_NAME).$(DISK_SUF)
	$(X64) $(X64_DEPLOY_OPTS) -exitscreenshot build/vice-build deploy/$(DEPLOY_NAME).$(DISK_SUF)
	\
	# make test disk
	echo  >build/c1541.script attach deploy/$(DEPLOY_NAME).$(DISK_SUF)
	echo >>build/c1541.script read durexforth
	echo >>build/c1541.script format "test,DF" $(DISK_SUF) deploy/tests.$(DISK_SUF)
	echo >>build/c1541.script write durexforth
	@for forth in $(TEST_SRC_NAMES); do\
		printf aa | cat - test/$$forth.fs | $(PETCAT) -text -w2 -o build/$$forth.pet - ; \
		echo >>build/c1541.script write build/$$forth.pet $$forth; \
	done;
	@for forth in $(TEST2_SRC_NAMES); do\
		printf aa | cat - $(SRC_DIR)/$$forth.fs | $(PETCAT) -text -w2 -o build/$$forth.pet - ; \
		echo >>build/c1541.script write build/$$forth.pet $$forth; \
	done;
	$(C1541) <build/c1541.script
	# run tests
	$(X64) $(X64_DEPLOY_OPTS) -exitscreenshot build/vice-test -keybuf "include test\n" deploy/tests.$(DISK_SUF)
	$(C1541) -attach deploy/tests.$(DISK_SUF) -read ok build/tests_passed
	\
	# make cartridge
	$(C1541) -attach deploy/$(DEPLOY_NAME).$(DISK_SUF) -read durexforth
	mv durexforth build/durexforth
	@$(AS) asm/cart.asm
	cartconv -t simon -i build/cart.bin -o deploy/$(DEPLOY_NAME).crt -n "DUREXFORTH $(TAG_DEPLOY_DOT)"
	asciidoctor-pdf -o deploy/$(DEPLOY_NAME).pdf manual/index.adoc

durexforth.prg: asm/*.asm
	mkdir -p build
	echo >build/version.asm !pet \"durexForth $(TAG_DEPLOY_DOT)\"
	@$(AS) -I asm asm/durexforth.asm

$(DISK_IMAGE): durexforth.prg Makefile $(SRCS)
	mkdir -p build
	touch build/empty
	echo  >build/c1541.script format "durexforth,DF" $(DISK_SUF) $@
	echo >>build/c1541.script write durexforth.prg durexforth
	echo >>build/c1541.script write build/empty $(SEPARATOR_NAME1)
	echo >>build/c1541.script write build/empty $(TAG_DEPLOY_DOT),s
	echo >>build/c1541.script write build/empty '  '$(GIT_HASH),s
	echo >>build/c1541.script write build/empty $(SEPARATOR_NAME2)
	@for forth in $(SRC_NAMES); do\
		printf aa | cat - $(SRC_DIR)/$$forth.fs | $(PETCAT) -text -w2 -o build/$$forth.pet - ; \
		echo >>build/c1541.script write build/$$forth.pet $$forth; \
	done;
	echo >>build/c1541.script write build/empty $(SEPARATOR_NAME3)
	$(C1541) <build/c1541.script

docs: docs/index.html

docs/index.html: manual/index.adoc manual/words.adoc manual/links.adoc manual/sid.adoc manual/asm.adoc \
	manual/mnemonics.adoc manual/memmap.adoc manual/anatomy.adoc LICENSE.txt manual/tutorial.adoc \
	manual/intro.adoc
	rm -rf docs
	asciidoctor -a revnumber=$(shell git describe --tags --dirty) -a revdate=$(shell git log -1 --format=%as) -o docs/index.html manual/index.adoc

check: $(DISK_IMAGE)
	$(X64) $(DISK_IMAGE)

clean:
	rm -f *.lbl *.prg *.$(DISK_SUF)
	rm -rf build deploy
