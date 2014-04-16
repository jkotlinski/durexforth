C1541   = c1541
AS = acme

# generic rules

all:	durexforth.d64

durexforth.prg: durexforth.a number.a ummod.a
	@$(AS) durexforth.a

forth_src/purge-hidden.pet: forth_src/purge-hidden.fs ext/petcom
	cat forth_src/purge-hidden.fs | ext/petcom - > forth_src/purge-hidden.pet
forth_src/base.pet: forth_src/base.fs ext/petcom
	cat forth_src/base.fs | ext/petcom - > forth_src/base.pet
forth_src/debug.pet: forth_src/debug.fs ext/petcom
	cat forth_src/debug.fs | ext/petcom - > forth_src/debug.pet
forth_src/vi.pet: forth_src/vi.fs ext/petcom
	cat forth_src/vi.fs | ext/petcom - > forth_src/vi.pet
forth_src/asm.pet: forth_src/asm.fs ext/petcom
	cat forth_src/asm.fs | ext/petcom - > forth_src/asm.pet
forth_src/labels.pet: forth_src/labels.fs ext/petcom
	cat forth_src/labels.fs | ext/petcom - > forth_src/labels.pet
forth_src/gfx.pet: forth_src/gfx.fs ext/petcom
	cat forth_src/gfx.fs | ext/petcom - > forth_src/gfx.pet
forth_src/gfxdemo.pet: forth_src/gfxdemo.fs ext/petcom
	cat forth_src/gfxdemo.fs | ext/petcom - > forth_src/gfxdemo.pet
forth_src/turtle.pet: forth_src/turtle.fs ext/petcom
	cat forth_src/turtle.fs | ext/petcom - > forth_src/turtle.pet
forth_src/fractal.pet: forth_src/fractal.fs ext/petcom
	cat forth_src/fractal.fs | ext/petcom - > forth_src/fractal.pet
forth_src/rnd.pet: forth_src/rnd.fs ext/petcom
	cat forth_src/rnd.fs | ext/petcom - > forth_src/rnd.pet
forth_src/sin.pet: forth_src/sin.fs ext/petcom
	cat forth_src/sin.fs | ext/petcom - > forth_src/sin.pet
forth_src/ls.pet: forth_src/ls.fs ext/petcom
	cat forth_src/ls.fs | ext/petcom - > forth_src/ls.pet
forth_src/sprite.pet: forth_src/sprite.fs ext/petcom
	cat forth_src/sprite.fs | ext/petcom - > forth_src/sprite.pet
forth_src/doloop.pet: forth_src/doloop.fs ext/petcom
	cat forth_src/doloop.fs | ext/petcom - > forth_src/doloop.pet
forth_src/jsr.pet: forth_src/jsr.fs ext/petcom
	cat forth_src/jsr.fs | ext/petcom - > forth_src/jsr.pet
forth_src/float.pet: forth_src/float.fs ext/petcom
	cat forth_src/float.fs | ext/petcom - > forth_src/float.pet
forth_src/mml.pet: forth_src/mml.fs ext/petcom
	cat forth_src/mml.fs | ext/petcom - > forth_src/mml.pet
forth_src/mmldemo.pet: forth_src/mmldemo.fs ext/petcom
	cat forth_src/mmldemo.fs | ext/petcom - > forth_src/mmldemo.pet

FORTHLIST=base debug vi asm gfx gfxdemo rnd sin ls turtle fractal purge-hidden sprite doloop jsr float labels mml mmldemo

durexforth.d64: durexforth.prg forth_src/base.pet forth_src/debug.pet forth_src/vi.pet forth_src/asm.pet forth_src/gfx.pet forth_src/gfxdemo.pet forth_src/rnd.pet forth_src/sin.pet forth_src/ls.pet forth_src/turtle.pet forth_src/fractal.pet forth_src/purge-hidden.pet forth_src/sprite.pet forth_src/doloop.pet forth_src/jsr.pet forth_src/float.pet forth_src/labels.pet forth_src/mml.pet forth_src/mmldemo.pet Makefile
	$(C1541) -format durexforth,DF  d64 durexforth.d64 # > /dev/null
	$(C1541) -attach $@ -write durexforth.prg durexforth  # > /dev/null
	# $(C1541) -attach $@ -write debug.bak
	@for forth in $(FORTHLIST); do\
        $(C1541) -attach $@ -write forth_src/$$forth.pet $$forth; \
    done;

clean:
	rm -f *.lbl *.prg forth_src/*.pet *.d64
