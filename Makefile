C1541   = ~/bin/c1541
AS = ~/bin/acme

# generic rules

all:	durexforth.d64

durexforth.prg: durexforth.a
	@$(AS) durexforth.a

forth_src/base.pet: forth_src/base.fs ext/petcom
	cat forth_src/base.fs | ext/petcom - > forth_src/base.pet
forth_src/debug.pet: forth_src/debug.fs ext/petcom
	cat forth_src/debug.fs | ext/petcom - > forth_src/debug.pet
forth_src/vi.pet: forth_src/vi.fs ext/petcom
	cat forth_src/vi.fs | ext/petcom - > forth_src/vi.pet
forth_src/asm.pet: forth_src/asm.fs ext/petcom
	cat forth_src/asm.fs | ext/petcom - > forth_src/asm.pet
forth_src/gfx.pet: forth_src/gfx.fs ext/petcom
	cat forth_src/gfx.fs | ext/petcom - > forth_src/gfx.pet
forth_src/gfxdemo.pet: forth_src/gfxdemo.fs ext/petcom
	cat forth_src/gfxdemo.fs | ext/petcom - > forth_src/gfxdemo.pet
forth_src/rnd.pet: forth_src/gfxdemo.fs ext/petcom
	cat forth_src/rnd.fs | ext/petcom - > forth_src/rnd.pet
forth_src/sin.pet: forth_src/sin.fs ext/petcom
	cat forth_src/sin.fs | ext/petcom - > forth_src/sin.pet
forth_src/ls.pet: forth_src/ls.fs ext/petcom
	cat forth_src/ls.fs | ext/petcom - > forth_src/ls.pet
forth_src/hexdec.pet: forth_src/hexdec.fs ext/petcom
	cat forth_src/hexdec.fs | ext/petcom - > forth_src/hexdec.pet

FORTHLIST=base debug vi asm gfx gfxdemo rnd sin ls hexdec

durexforth.d64: durexforth.prg forth_src/base.pet forth_src/debug.pet forth_src/vi.pet forth_src/asm.pet forth_src/gfx.pet forth_src/gfxdemo.pet forth_src/rnd.pet forth_src/sin.pet forth_src/hexdec.pet forth_src/ls.pet Makefile
	$(C1541) -format durexforth,DF  d64 durexforth.d64 > /dev/null
	$(C1541) -attach $@ -write durexforth.prg durexforth  > /dev/null
	# $(C1541) -attach $@ -write debug.bak
	@for forth in $(FORTHLIST); do\
        $(C1541) -attach $@ -write forth_src/$$forth.pet $$forth; \
    done;

clean:
	rm -f *.lbl *.prg forth_src/*.pet
