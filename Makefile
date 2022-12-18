C1541   = c1541
AS = acme
# deploy 1571 (d71) or 1581 (d81); e.g. make DISK_SUF=d81 deploy
DISK_SUF = d64

TAG := $(shell git describe --tags --abbrev=0 || svnversion --no-newline)
TAG_DEPLOY_DOT := $(shell git describe --tags --long --dirty=_m | sed 's/-g[0-9a-f]\+//' | tr _- -.)
TAG_DEPLOY := $(shell git describe --tags --abbrev=0 --dirty=_M | tr _. -_)
GIT_HASH := $(shell git rev-parse --short HEAD)

DEPLOY_NAME = durexforth-$(TAG_DEPLOY)
DISK_IMAGE = durexforth.$(DISK_SUF)

X64_OPTS = -warp
X64 = x64sc
X64_OPTS += +confirmonexit
PETCAT = petcat # text conversion utility, included in VICE package

SRC_DIR = forth_src
SRC_NAMES = base debug v asm gfx gfxdemo rnd sin ls turtle fractals \
    sprite doloop sys labels mml mmldemo sid spritedemo \
    format require compat timer float viceutil turnkey \
    wordlist io open dos see
SRCS = $(addprefix $(SRC_DIR)/,$(addsuffix .fs,$(SRC_NAMES)))

TEST_SRC_NAMES = test testcore testcoreplus testcoreext tester testsee
TEST2_SRC_NAMES = see gfx gfxdemo fractals mmldemo mml sid spritedemo sprite compat rnd sin turtle
TEST_SRCS = $(addprefix test/,$(addsuffix .fs,$(TEST_SRC_NAMES)))

SEPARATOR_NAME1 = '=-=-=-=-=-=-=-=,s'
SEPARATOR_NAME2 = '=-------------=,s'
SEPARATOR_NAME3 = '=-=---=-=---=-=,s'

all: $(DISK_IMAGE)

deploy: $(DISK_IMAGE) asm_src/cart.asm $(TEST_SRCS)
	rm -rf deploy
	mkdir deploy
	cp $(DISK_IMAGE) deploy/$(DEPLOY_NAME).$(DISK_SUF)
	$(X64) $(X64_OPTS) deploy/$(DEPLOY_NAME).$(DISK_SUF)
	\
	# make test disk
	echo  >build/c1541.script attach deploy/$(DEPLOY_NAME).$(DISK_SUF)
	echo >>build/c1541.script read durexforth
	echo >>build/c1541.script format "test,DF" $(DISK_SUF) deploy/tests.$(DISK_SUF)
	echo >>build/c1541.script write durexforth
	@for forth in $(TEST_SRC_NAMES); do\
		cat build/header test/$$forth.fs | $(PETCAT) -text -w2 -o build/$$forth.pet - ; \
		echo >>build/c1541.script write build/$$forth.pet $$forth; \
	done;
	@for forth in $(TEST2_SRC_NAMES); do\
		cat build/header $(SRC_DIR)/$$forth.fs | $(PETCAT) -text -w2 -o build/$$forth.pet - ; \
		echo >>build/c1541.script write build/$$forth.pet $$forth; \
	done;
	c1541 <build/c1541.script
	\
	# make cartridge
	c1541 -attach deploy/$(DEPLOY_NAME).$(DISK_SUF) -read durexforth
	mv durexforth build/durexforth
	@$(AS) asm_src/cart.asm
	cartconv -t simon -i build/cart.bin -o deploy/$(DEPLOY_NAME).crt -n "DUREXFORTH $(TAG_DEPLOY_DOT)"
	asciidoctor-pdf -o deploy/$(DEPLOY_NAME).pdf docs_src/index.adoc

durexforth.prg: asm_src/*.asm
	echo >build/version.asm !pet \"durexForth $(TAG)\"
	@$(AS) -I asm_src asm_src/durexforth.asm

.ONESHELL:
$(DISK_IMAGE): durexforth.prg Makefile $(SRCS)
	mkdir -p build
	touch build/empty
	echo  >build/c1541.script format "durexforth,DF" $(DISK_SUF) $@
	echo >>build/c1541.script write durexforth.prg durexforth
	echo >>build/c1541.script write build/empty $(SEPARATOR_NAME1)
	echo >>build/c1541.script write build/empty $(TAG_DEPLOY_DOT),s
	echo >>build/c1541.script write build/empty '  '$(GIT_HASH),s
	echo >>build/c1541.script write build/empty $(SEPARATOR_NAME2)
	echo -n "aa" > build/header
	@for forth in $(SRC_NAMES); do\
		cat build/header $(SRC_DIR)/$$forth.fs | $(PETCAT) -text -w2 -o build/$$forth.pet - ; \
		echo >>build/c1541.script write build/$$forth.pet $$forth; \
	done;
	echo >>build/c1541.script write build/empty $(SEPARATOR_NAME3)
	c1541 <build/c1541.script

docs: docs/index.html

docs/index.html: docs_src/index.adoc docs_src/words.adoc docs_src/links.adoc docs_src/sid.adoc docs_src/asm.adoc \
	docs_src/mnemonics.adoc docs_src/memmap.adoc docs_src/anatomy.adoc LICENSE.txt docs_src/tutorial.adoc \
	docs_src/intro.adoc
	rm -rf docs
	asciidoctor -a revnumber=$(shell git describe --tags --dirty) -a revdate=$(shell git log -1 --format=%as) -o docs/index.html docs_src/index.adoc

clean:
	rm -f *.lbl *.prg *.$(DISK_SUF)
	rm -rf build deploy
