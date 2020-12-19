require open

\ Set input device to open file #
\ returns 0 on success, file # on error
code chkin? ( file# -- file# result )
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

\ Set output device to open file #
\ returns 0 on success, file # on error
code chkout? ( file# -- file# result )
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

\ handle return value from open?, chkin?
\ and chkout?. If result is nonzero,
\ close file, print error msg, and abort
: ioerr ( file# result -- )
?dup if rvs case
2 of ." file# in use" endof
3 of ." file not open" endof
5 of ." device not present" endof
6 of ." not input file" endof
7 of ." not output file" endof
." io err" 
endcase clrchn close cr abort
else drop then ;

\ Open a file
\ 'easy' version, aborts on error
: open ( caddr u file# sa -- )
open? ioerr ;

\ Set input device to open file #
\ 'easy' version, aborts on error
: chkin ( file# -- ) 
chkin? ioerr ;

\ Set output device to open file #
\ 'easy' version, aborts on error
: chkout ( file# -- )
chkout? ioerr ;
