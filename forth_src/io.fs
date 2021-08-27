require open

\ Use logical file as input device
\ ioresult is 0 on success, kernal
\ error # on failure.
code chkin ( file# -- file# ioresult )
w stx,
lsb lda,x tax, \ x = file#
$ffc6 jsr, \ CHKIN
+branch bcs, \ carry set = error
0 lda,# \ A is only valid on error
:+
w ldx, dex, 
lsb sta,x
0 lda,# msb sta,x
;code

\ Use logical file as output device
\ ioresult is 0 on success, kernal
\ error # on failure.
code chkout ( file# -- file# ioresult )
w stx,
lsb lda,x tax, \ x = file#
$ffc9 jsr, \ CHKOUT
+branch bcs, \ carry set = error
0 lda,# \ A is only valid on error
:+
w ldx, dex,
lsb sta,x
0 lda,# msb sta,x
;code

\ Reset input and output to console 
code clrchn ( -- ) 
txa, pha,
$ffcc jsr,  \ CLRCH
pla, tax,
;code

\ Read status of last IO operation
code readst ( -- status )
dex, 0 lda,# msb sta,x
$ffb7 jsr, \ READST
lsb sta,x
;code

\ Get a byte from input device
code chrin ( -- chr )
dex, w stx, 0 lda,# msb sta,x
$ffcf jsr, \ CHRIN
w ldx, lsb sta,x
;code

\ handle errors returned by open,
\ close, and chkin. If ioresult is 
\ nonzero, close file and abort with
\ an appropriate error message.
: ioabort ( file# ioresult -- )
?dup if rvs case
1 of ." too many files" endof
2 of ." file# in use" endof
3 of ." file not open" endof
4 of ." file not found" endof
5 of ." device not present" endof
6 of ." not input file" endof
7 of ." not output file" endof
8 of ." missing filename" endof
9 of ." illegal device number" endof
." io err" 
endcase clrchn close cr abort
else drop then ;
