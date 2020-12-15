ASFLAGS :=

C1541 = c1541
AS = acme $(ASFLAGS)
TAG = $(shell git describe --tags --abbrev=0 || svnversion --no-newline)
TAG_DEPLOY_DOT = $(shell git describe --tags --abbrev=0 --dirty=-M)
TAG_DEPLOY = $(shell git describe --tags --abbrev=0 --dirty=_M | tr _. -_)

SRC_DIR = forth_src
SRCS_COMMON = $(wildcard $(SRC_DIR)/*.fs)
SRCS_C64 = $(SRCS_COMMON) $(wildcard $(SRC_DIR)/c64/*.fs)
SRCS_C128 = $(SRCS_COMMON) $(wildcard $(SRC_DIR)/c128/*.fs)

PETSRCS_C64 = $(subst forth_src/,build/,$(SRCS_C64:%.fs=%.pet))
PETSRCS_C128 = $(subst forth_src/,build/,$(SRCS_C128:%.fs=%.pet))

SRCNAME = $(patsubst %.pet,%,$(notdir $(1)))

EMPTY_FILE = _empty.txt
SEPARATOR_NAME1 = '=-=-=-=-=-=-=-=,s'
SEPARATOR_NAME2 = '=-------------=,s'
SEPARATOR_NAME3 = '=-=---=-=---=-=,s'

all: durexforth.d64

docs:
	$(MAKE) -C docs

deploy: deploy/durexforth-$(TAG_DEPLOY).pdf deploy/durexforth-$(TAG_DEPLOY).d64 deploy/durexforth-$(TAG_DEPLOY).crt deploy/durexforth128-$(TAG_DEPLOY).d64

.PHONY: all clean docs deploy

deploy/durexforth-$(TAG_DEPLOY).pdf: docs
	@mkdir -p deploy
	cp docs/durexforth.pdf $@

# Precompile and save a packed Forth for release
deploy/durexforth-$(TAG_DEPLOY).d64: durexforth.d64
	@mkdir -p deploy
	cp $< $@
	x64 -warp +confirmonexit $@

deploy/durexforth128-$(TAG_DEPLOY).d64: durexforth128.d64
	@mkdir -p deploy
	cp $< $@
	x128 -warp +confirmonexit $@

# Build a cartridge image out of the precompiled Forth
deploy/durexforth-$(TAG_DEPLOY).crt: deploy/durexforth-$(TAG_DEPLOY).d64 cart.asm
	c1541 -attach deploy/durexforth-$(TAG_DEPLOY).d64 -read durexforth build/durexforth
	$(AS) cart.asm
	cartconv -t simon -i build/cart.bin -o deploy/durexforth-$(TAG_DEPLOY).crt -n "DUREXFORTH $(TAG_DEPLOY_DOT)"

durexforth.d64: durexforth.prg $(EMPTY_FILE) $(PETSRCS_C64)
	$(C1541) -format "durexforth,DF"  d64 $@ \
	-attach $@ -write durexforth.prg durexforth \
	-attach $@ -write $(EMPTY_FILE) $(SEPARATOR_NAME1) \
	-attach $@ -write $(EMPTY_FILE) $(TAG_DEPLOY_DOT),s \
	-attach $@ -write $(EMPTY_FILE) $(SEPARATOR_NAME2) \
	$(foreach f,$(PETSRCS_C64),-write $f $(call SRCNAME,$f)) \
	-attach $@ -write $(EMPTY_FILE) $(SEPARATOR_NAME3)

durexforth128.d64: durexforth128.prg $(EMPTY_FILE) $(PETSRCS_C128)
	$(C1541) -format "durexforth,DF"  d64 $@ \
	-attach $@ -write durexforth128.prg durexforth \
	-attach $@ -write $(EMPTY_FILE) $(SEPARATOR_NAME1) \
	-attach $@ -write $(EMPTY_FILE) $(TAG_DEPLOY_DOT),s \
	-attach $@ -write $(EMPTY_FILE) $(SEPARATOR_NAME2) \
	$(foreach f,$(PETSRCS_C128),-write $f $(call SRCNAME,$f)) \
	-attach $@ -write $(EMPTY_FILE) $(SEPARATOR_NAME3)

durexforth.prg: *.asm
	$(AS) -f cbm -DTARGET=64 -o $@ --vicelabels durexforth.lbl --report durexforth.lst durexforth.asm

durexforth128.prg: *.asm
	$(AS) -f cbm -DTARGET=128 -o $@ --vicelabels durexforth.lbl --report durexforth.lst durexforth.asm

build/%.pet: $(SRC_DIR)/%.fs | build/header
	@mkdir -p $(dir $@)
	cat build/header $< | ext/petcom - > $@

.INTERMEDIATE: $(EMPTY_FILE) build/header

build/header:
	@mkdir -p build
	echo -n "aa" > build/header
	
 $(EMPTY_FILE):
	touch $@

clean:
	$(MAKE) -C docs clean
	rm -f *.lbl *.prg *.d64
	rm -rf build deploy
