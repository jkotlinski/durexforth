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

SRC_DIR = forth_src
SRC_NAMES = base debug v asm gfx gfxdemo rnd sin ls turtle fractals \
    sprite doloop sys labels mml mmldemo sid spritedemo test testcore \
    testcoreplus tester format require compat timer float viceutil turnkey \
    wordlist io open dos
SRCS = $(addprefix $(SRC_DIR)/,$(addsuffix .fs,$(SRC_NAMES)))

EMPTY_FILE = _empty.txt
SEPARATOR_NAME1 = '=-=-=-=-=-=-=-=,s'
SEPARATOR_NAME2 = '=-------------=,s'
SEPARATOR_NAME3 = '=-=---=-=---=-=,s'

all: $(DISK_IMAGE)

deploy: $(DISK_IMAGE) cart.asm
	rm -rf deploy
	mkdir deploy
	$(MAKE) -C docs
	cp docs/durexforth.pdf deploy/$(DEPLOY_NAME).pdf
	cp $(DISK_IMAGE) deploy/$(DEPLOY_NAME).$(DISK_SUF)
	$(X64) $(X64_OPTS) deploy/$(DEPLOY_NAME).$(DISK_SUF)
	# make cartridge
	c1541 -attach deploy/$(DEPLOY_NAME).$(DISK_SUF) -read durexforth
	mv durexforth build/durexforth
	@$(AS) cart.asm
	cartconv -t simon -i build/cart.bin -o deploy/$(DEPLOY_NAME).crt -n "DUREXFORTH $(TAG_DEPLOY_DOT)"

durexforth.prg: *.asm
	@$(AS) durexforth.asm

$(DISK_IMAGE): durexforth.prg Makefile ext/petcom $(SRCS)
	touch $(EMPTY_FILE)
	echo  >c1541.script format "durexforth,DF" $(DISK_SUF) $@
	echo >>c1541.script write durexforth.prg durexforth
	echo >>c1541.script write $(EMPTY_FILE) $(SEPARATOR_NAME1)
	echo >>c1541.script write $(EMPTY_FILE) $(TAG_DEPLOY_DOT),s
	echo >>c1541.script write $(EMPTY_FILE) '  '$(GIT_HASH),s
	echo >>c1541.script write $(EMPTY_FILE) $(SEPARATOR_NAME2)
# $(C1541) -attach $@ -write debug.bak
	mkdir -p build
	echo -n "aa" > build/header
	@for forth in $(SRC_NAMES); do\
        cat build/header $(SRC_DIR)/$$forth.fs | ext/petcom - > build/$$forth.pet; \
        echo >>c1541.script write build/$$forth.pet $$forth; \
    done;
	echo >>c1541.script write $(EMPTY_FILE) $(SEPARATOR_NAME3)
	c1541 <c1541.script
	rm -f $(EMPTY_FILE)

clean:
	$(MAKE) -C docs clean
	rm -f *.lbl *.prg *.$(DISK_SUF)
	rm -rf build deploy
