C1541 = c1541
AS = acme
TAG := $(shell git describe --tags --abbrev=0 || svnversion --no-newline)
REV_TAG_P := $(shell git describe --tags --abbrev=0 --dirty=-M || svnversion --no-newline)
REV_TAG_DOC := $(shell git describe --tags --long --dirty=_MODIFIED | sed 's/-g[0-9a-f]\+//' | tr _- -.)
REV_DATE := $(shell git log -1 --format=%ai)

# generic rules

all:	durexforth.pdf

SECTIONS = release_license.tex \
        intro.tex tutorial.tex editor.tex words.tex gfx.tex sid.tex mml.tex \
        asm.tex mnemonics.tex memmap.tex anatomy.tex
GENERATED = params-g.tex

durexforth.pdf:	manual.tex $(SECTIONS)
	pdflatex -jobname=durexforth manual.tex

release_license.tex: params-g.tex
	diff -q $^ $^.tmp; if [ $$? -eq 1 ]; then touch $@; fi
	rm $^.tmp

params-g.tex:
	touch $@
	cp $@ $@.tmp
	echo "\\def\\\revisiontagdoc{$(REV_TAG_DOC)}" > $@
	echo "\\def\\\revisiondate{$(REV_DATE)}" >> $@

clean:
	rm -f *.pdf $(GENERATED)

.PHONY: $(GENERATED)

