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
end-code

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
end-code

\ Reset input and output to console
code clrchn ( -- )
txa, pha,
$ffcc jsr,  \ CLRCH
pla, tax,
end-code

\ Read status of last IO operation
code readst ( -- status )
dex, 0 lda,# msb sta,x
$ffb7 jsr, \ READST
lsb sta,x
end-code

\ Get a byte from input device
code chrin ( -- chr )
dex, w stx, 0 lda,# msb sta,x
$ffcf jsr, \ CHRIN
w ldx, lsb sta,x
end-code
