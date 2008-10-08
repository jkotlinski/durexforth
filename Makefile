C1541   = ~/bin/c1541
AS = ~/bin/acme

# generic rules

all:	durexforth.d64

durexforth.prg: durexforth.a
	@$(AS) durexforth.a

forth_src/base.pet: forth_src/base.src ext/petcom
	@echo "acme durexforth.a"
	cat forth_src/base.src | ext/petcom - > forth_src/base.pet

FORTHLIST=base

durexforth.d64: durexforth.prg forth_src/base.pet
	$(C1541) -format durexforth,DF  d64 durexforth.d64 > /dev/null
	$(C1541) -attach $@ -write durexforth.prg  > /dev/null
	@for forth in $(FORTHLIST); do\
        $(C1541) -attach $@ -write forth_src/$$forth.pet $$forth; \
    done;

clean:
	rm -f *.lbl *.prg forth_src/*.pet
