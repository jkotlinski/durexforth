C1541 = c1541
AS = acme
TAG := $(shell git describe --tags --abbrev=0 || svnversion --no-newline)
REV_TAG_P := $(shell git describe --tags --abbrev=0 --dirty=-M || svnversion --no-newline)
REV_TAG_DOC := $(shell git describe --tags --long --dirty=_MODIFIED | sed 's/-g[0-9a-f]\+//' | tr _- -.)
REV_DATE := $(shell git log -1 --format=%ai)

# generic rules

all:	durexforth.pdf durexforth.html durexforth.epub

SECTIONS = release_license.tex \
        intro.tex tutorial.tex editor.tex words.tex gfx.tex sid.tex mml.tex \
        asm.tex mnemonics.tex memmap.tex anatomy.tex
GENERATED = params-g.tex

durexforth.pdf:	manual.tex $(SECTIONS) manual.ist
	cp manual.ist durexforth.mst
	pdflatex -jobname=durexforth manual.tex
	pdflatex -jobname=durexforth manual.tex
	pdflatex -jobname=durexforth manual.tex
	rm durexforth.mst

durexforth.html durexforth.css: manual.tex $(SECTIONS)
	make4ht -e manual.mk4 -j durexforth $<

durexforth.epub: manual.tex $(SECTIONS)
	tex4ebook -f epub3 -e manual.mk4 -j durexforth $<

release_license.tex: params-g.tex
	diff -q $^ $^.tmp; if [ $$? -eq 1 ]; then touch $@; fi
	rm $^.tmp

params-g.tex:
	touch $@
	cp $@ $@.tmp
	printf %s\\n "\\def\\revisiontagdoc{$(REV_TAG_DOC)}" > $@
	printf %s\\n "\\def\\revisiondate{$(REV_DATE)}" >> $@

clean:
	rm -f *.pdf $(GENERATED)

.PHONY: $(GENERATED)

