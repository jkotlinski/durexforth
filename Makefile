C1541   = c1541
AS = acme
TAG = `git describe --tags --abbrev=0 || svnversion --no-newline`
TAG_DEPLOY_DOT = `git describe --tags --abbrev=0`
TAG_DEPLOY = `git describe --tags --abbrev=0 | tr . _`

all:	durexforth.d64

deploy: durexforth.d64 cart.asm
	rm -rf deploy
	mkdir deploy
	$(MAKE) -C docs
	cp docs/durexforth.pdf deploy/durexforth-$(TAG_DEPLOY).pdf
	cp durexforth.d64 deploy/durexforth-$(TAG_DEPLOY).d64
	x64 deploy/durexforth-$(TAG_DEPLOY).d64
	# make cartridge
	c1541 -attach deploy/durexforth-$(TAG_DEPLOY).d64 -read durexforth
	mv durexforth build/durexforth
	@$(AS) cart.asm
	cartconv -t simon -i build/cart.bin -o deploy/durexforth-$(TAG_DEPLOY).crt -n "DUREXFORTH $(TAG_DEPLOY_DOT)"

durexforth.prg: durexforth.asm number.asm math.asm move.asm disk.asm lowercase.asm
	@$(AS) durexforth.asm

FORTHLIST=base asm format labels tvattrad sprite doloop sprites
# debug vi asm gfx gfxdemo rnd sin ls turtle fractals sprite doloop sys labels mml mmldemo sid spritedemo test testcore testcoreplus tester format require compat timer float

tvattrad: forth_src/tvattrad.fs
	cat build/header forth_src/tvattrad.fs | ext/petcom - > build/tvattrad.pet
	$(C1541) -attach durexforth.d64 -delete tvattrad -write build/tvattrad.pet tvattrad

durexforth.d64: durexforth.prg forth_src/base.fs forth_src/debug.fs forth_src/vi.fs forth_src/asm.fs forth_src/gfx.fs forth_src/gfxdemo.fs forth_src/rnd.fs forth_src/sin.fs forth_src/ls.fs forth_src/turtle.fs forth_src/fractals.fs forth_src/sprite.fs forth_src/doloop.fs forth_src/sys.fs forth_src/labels.fs forth_src/mml.fs forth_src/mmldemo.fs forth_src/sid.fs forth_src/spritedemo.fs forth_src/test.fs Makefile ext/petcom forth_src/testcore.fs forth_src/testcoreplus.fs forth_src/tester.fs forth_src/format.fs forth_src/require.fs forth_src/compat.fs forth_src/timer.fs forth_src/float.fs
	$(C1541) -format "durexforth$(TAG),DF"  d64 durexforth.d64 # > /dev/null
	$(C1541) -attach $@ -write durexforth.prg durexforth # > /dev/null
# $(C1541) -attach $@ -write debug.bak
	mkdir -p build
	echo -n "aa" > build/header
	@for forth in $(FORTHLIST); do\
        cat build/header forth_src/$$forth.fs | ext/petcom - > build/$$forth.pet; \
        $(C1541) -attach $@ -write build/$$forth.pet $$forth; \
    done;

clean:
	$(MAKE) -C docs clean
	rm -f *.lbl *.prg *.d64
	rm -rf build deploy
