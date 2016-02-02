C1541   = c1541
AS = acme
TAG = `git describe --tags --abbrev=0 || svnversion --no-newline`

# generic rules

all:	durexforth.pdf

durexforth.pdf: anatomy.tex  editor.tex  gfx.tex    manual.tex  mml.tex asm.tex      forth.tex   intro.tex  memmap.tex  tutorial.tex sid.tex mnemonics.tex
	pdflatex -jobname=durexforth manual.tex

clean:
	rm -f *.pdf
