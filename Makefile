C1541   = c1541
AS = acme
TAG = `git describe --tags --abbrev=0 || svnversion --no-newline`
TAG_DEPLOY_DOT = `git describe --tags --abbrev=0 --dirty=-M`
TAG_DEPLOY = `git describe --tags --abbrev=0 --dirty=_M | tr _. -_`

SRC_DIR = forth_src
SRC_NAMES = base debug v asm gfx gfxdemo rnd sin ls turtle fractals \
    sprite doloop sys labels mml mmldemo sid spritedemo test testcore \
    testcoreplus tester format require compat timer float viceutil \
    filesystem
SRCS = $(addprefix $(SRC_DIR)/,$(addsuffix .fs,$(SRC_NAMES)))

EMPTY_FILE = _empty.txt
SEPARATOR_NAME1 = '=-=-=-=-=-=-=-=,s'
SEPARATOR_NAME2 = '=-------------=,s'
SEPARATOR_NAME3 = '=-=---=-=---=-=,s'

all:	durexforth.d64

deploy: durexforth.d64 cart.asm
	rm -rf deploy
	mkdir deploy
	$(MAKE) -C docs
	cp docs/durexforth.pdf deploy/durexforth-$(TAG_DEPLOY).pdf
	cp durexforth.d64 deploy/durexforth-$(TAG_DEPLOY).d64
	x64 -default -warp +confirmexit deploy/durexforth-$(TAG_DEPLOY).d64
	# make cartridge
	c1541 -attach deploy/durexforth-$(TAG_DEPLOY).d64 -read durexforth
	mv durexforth build/durexforth
	@$(AS) cart.asm
	cartconv -t simon -i build/cart.bin -o deploy/durexforth-$(TAG_DEPLOY).crt -n "DUREXFORTH $(TAG_DEPLOY_DOT)"

durexforth.prg: *.asm
	@$(AS) durexforth.asm

durexforth.d64: durexforth.prg Makefile ext/petcom $(SRCS)
	touch $(EMPTY_FILE)
	$(C1541) -format "durexforth,DF"  d64 durexforth.d64 # > /dev/null
	$(C1541) -attach $@ -write durexforth.prg durexforth # > /dev/null
	$(C1541) -attach $@ -write $(EMPTY_FILE) $(SEPARATOR_NAME1) # > /dev/null
	$(C1541) -attach $@ -write $(EMPTY_FILE) $(TAG_DEPLOY_DOT),s # > /dev/null
	$(C1541) -attach $@ -write $(EMPTY_FILE) $(SEPARATOR_NAME2) # > /dev/null
# $(C1541) -attach $@ -write debug.bak
	mkdir -p build
	echo -n "aa" > build/header
	@for forth in $(SRC_NAMES); do\
        cat build/header $(SRC_DIR)/$$forth.fs | ext/petcom - > build/$$forth.pet; \
        $(C1541) -attach $@ -write build/$$forth.pet $$forth; \
    done;
	$(C1541) -attach $@ -write $(EMPTY_FILE) $(SEPARATOR_NAME3) # > /dev/null
	rm -f $(EMPTY_FILE)

clean:
	$(MAKE) -C docs clean
	rm -f *.lbl *.prg *.d64
	rm -rf build deploy
