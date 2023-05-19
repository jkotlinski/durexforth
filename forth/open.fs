\ Open a logical file
\ ioresult is 0 on success, kernal
\ error # on failure.
( nameaddr namelen file# sa --
  ioresult )
code open
w stx,
lsb 1+ lda,x \ a = file #
lsb ldy,x \ y = sec. address
$ba ldx, \ x = device
$ffba jsr, \ SETLFS

w ldx,
lsb 2+ lda,x pha, \ a = namelen
msb 3 + ldy,x
lsb 3 + lda,x tax, pla, \ xy = nameptr
$ffbd jsr, \ SETNAM

$ffc0 jsr, \ OPEN
+branch bcs, \ carry set = error
0 lda,# \ A is only valid on error
:+
w ldx,
inx, inx, inx,
lsb sta,x
0 lda,# msb sta,x
end-code

\ Close a logical file
code close ( file# -- )
txa, pha,
lsb lda,x \ x = file#
$ffc3 jsr, \ CLOSE
pla, tax, inx,
end-code
