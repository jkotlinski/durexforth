\ Use logical file as input device
\ ioresult is 0 on success, kernal
\ error # on failure.
code chkin ( file# -- ioresult )
w stx,
lsb lda,x tax, \ x = file#
$ffc6 jsr, \ CHKIN
+branch bcs, \ carry set = error
0 lda,# \ A is only valid on error
:+
w ldx,
lsb sta,x
0 lda,# msb sta,x
;code

\ Use logical file as output device
\ ioresult is 0 on success, kernal
\ error # on failure.
code chkout ( file# -- ioresult )
w stx,
lsb lda,x tax, \ x = file#
$ffc9 jsr, \ CHKOUT
+branch bcs, \ carry set = error
0 lda,# \ A is only valid on error
:+
w ldx,
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
\ nonzero, print error message and
\ abort.
: berr ( ioresult -- )
?dup if
rvs 55 1 c! 1-
2* $a328 + @
begin dup c@ dup 128 and 0= while
emit 1+ repeat 128 - emit
cr abort then ;

\ handle out of range ioresult
: ioabort  ( ioresult -- ? )
dup 9 > if rev ." io err" drop
else berr then ;
