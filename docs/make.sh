#!/bin/bash
pdflatex manual.tex 
mv manual.pdf durexforth.pdf
cygstart durexforth.pdf
