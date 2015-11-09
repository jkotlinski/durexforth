C1541   = c1541
AS = acme

# generic rules

all:	durexforth.d64

durexforth.prg: durexforth.a number.a math.a cmove.a disk.a
	@$(AS) durexforth.a

FORTHLIST=base debug vi asm gfx gfxdemo rnd sin ls turtle fractals purge-hidden sprite doloop sys float labels mml mmldemo sid spritedemo test

durexforth.d64: durexforth.prg forth_src/base.fs forth_src/debug.fs forth_src/vi.fs forth_src/asm.fs forth_src/gfx.fs forth_src/gfxdemo.fs forth_src/rnd.fs forth_src/sin.fs forth_src/ls.fs forth_src/turtle.fs forth_src/fractals.fs forth_src/purge-hidden.fs forth_src/sprite.fs forth_src/doloop.fs forth_src/sys.fs forth_src/float.fs forth_src/labels.fs forth_src/mml.fs forth_src/mmldemo.fs forth_src/sid.fs forth_src/spritedemo.fs forth_src/test.fs Makefile ext/petcom
	$(C1541) -format durexforth,DF  d64 durexforth.d64 # > /dev/null
	$(C1541) -attach $@ -write durexforth.prg durexforth # > /dev/null
# $(C1541) -attach $@ -write debug.bak
	mkdir -p build
	echo -n "aa" > build/header
	@for forth in $(FORTHLIST); do\
        cat build/header forth_src/$$forth.fs | ext/petcom - > build/$$forth.pet; \
        $(C1541) -attach $@ -write build/$$forth.pet $$forth; \
    done;

clean:
	rm -f *.lbl *.prg *.d64 
	rm -rf build
