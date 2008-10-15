C1541   = ~/bin/c1541
AS = ~/bin/acme

# generic rules

all:	durexforth.d64

durexforth.prg: durexforth.a
	@$(AS) durexforth.a

forth_src/base.pet: forth_src/base.src ext/petcom
	cat forth_src/base.src | ext/petcom - > forth_src/base.pet

forth_src/debug.pet: forth_src/debug.src ext/petcom
	cat forth_src/debug.src | ext/petcom - > forth_src/debug.pet

forth_src/edit.pet: forth_src/edit.src ext/petcom
	cat forth_src/edit.src | ext/petcom - > forth_src/edit.pet

forth_src/test.pet: forth_src/test.src ext/petcom
	cat forth_src/test.src | ext/petcom - > forth_src/test.pet

FORTHLIST=base debug edit test

durexforth.d64: durexforth.prg forth_src/base.pet forth_src/debug.pet forth_src/edit.pet forth_src/test.pet
	$(C1541) -format durexforth,DF  d64 durexforth.d64 > /dev/null
	$(C1541) -attach $@ -write durexforth.prg  > /dev/null
	$(C1541) -attach $@ -write forth_src/test.pet debug.bak
	@for forth in $(FORTHLIST); do\
        $(C1541) -attach $@ -write forth_src/$$forth.pet $$forth; \
    done;

clean:
	rm -f *.lbl *.prg forth_src/*.pet
