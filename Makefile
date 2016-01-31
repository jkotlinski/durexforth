C1541   = c1541
AS = acme
TAG = `git describe --tags --abbrev=0 || svnversion --no-newline`
TAG_DEPLOY = `git describe --tags --abbrev=0 | tr . _`

all:	durexforth.d64 durexforth.crt

deploy: durexforth.d64 durexforth.crt
	$(MAKE) -C docs
	rm -rf deploy
	mkdir deploy
	cp durexforth.d64 deploy/durexforth-$(TAG_DEPLOY).d64
	cp durexforth.crt deploy/durexforth-$(TAG_DEPLOY).crt
	cp docs/durexforth.pdf deploy/durexforth-$(TAG_DEPLOY).pdf

durexforth.crt: durexforth.prg cart.a core.a
	@$(AS) cart.a
	cartconv -t normal -i build/cart.bin -o durexforth.crt -n "DUREXFORTH $(TAG_DEPLOY)"

durexforth.prg: durexforth.a core.a number.a math.a move.a disk.a lowercase.a
	@$(AS) durexforth.a

FORTHLIST=base debug v asm gfx gfxdemo rnd sin ls turtle fractals sprite doloop sys labels mml mmldemo sid spritedemo test testcore testcoreplus tester format require compat

durexforth.d64: durexforth.prg forth_src/base.fs forth_src/debug.fs forth_src/v.fs forth_src/asm.fs forth_src/gfx.fs forth_src/gfxdemo.fs forth_src/rnd.fs forth_src/sin.fs forth_src/ls.fs forth_src/turtle.fs forth_src/fractals.fs forth_src/sprite.fs forth_src/doloop.fs forth_src/sys.fs forth_src/labels.fs forth_src/mml.fs forth_src/mmldemo.fs forth_src/sid.fs forth_src/spritedemo.fs forth_src/test.fs Makefile ext/petcom forth_src/testcore.fs forth_src/testcoreplus.fs forth_src/tester.fs forth_src/format.fs forth_src/require.fs forth_src/compat.fs
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
	rm -rf build
