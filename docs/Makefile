C1541   = c1541
AS = acme
TAG = `git describe --tags --abbrev=0 || svnversion --no-newline`

# generic rules

all:	durexforth.pdf

durexforth.pdf: anatomy.tex  editor.tex  gfx.tex    manual.tex  mml.tex asm.tex      forth.tex   intro.tex  memmap.tex  tutorial.tex sid.tex
	pdflatex manual.tex
	mv manual.pdf durexforth.pdf

clean:
	rm -f *.pdf
