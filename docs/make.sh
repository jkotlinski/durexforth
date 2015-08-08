#!/bin/bash
pdflatex manual.tex 
mv manual.pdf durexForth.pdf
cygstart durexForth.pdf
