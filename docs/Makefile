C1541 = c1541
AS = acme
TAG = `git describe --tags --abbrev=0 || svnversion --no-newline`

# generic rules

all:	durexforth.pdf

SECTIONS = release_license.tex \
        intro.tex tutorial.tex editor.tex forth.tex gfx.tex sid.tex mml.tex \
        asm.tex mnemonics.tex memmap.tex anatomy.tex
GENERATED = params-g.tex

durexforth.pdf:	manual.tex $(SECTIONS)
	pdflatex -jobname=durexforth manual.tex

release_license.tex: params-g.tex

params-g.tex:
	echo "\\def\\releasetag{$(TAG)}" > $@

clean:
	rm -f *.pdf $(GENERATED)

