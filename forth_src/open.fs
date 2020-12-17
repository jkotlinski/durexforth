\ Open a file, no error handling
( nameaddr namelen sa file# -- result )
code (open)
w stx,
lsb lda,x \ a = file #
lsb 1+ ldy,x \ y = sec. address
$ba ldx, \ x = device
$ffba jsr, \ SETLFS

w ldx,
lsb 2+ lda,x pha, \ a = namelen
msb 3 + ldy,x
lsb 3 + lda,x tax, pla, \ xy = nameptr
$ffbd jsr, \ SETNAM

$ffc0 jsr, \ OPEN
0 lda,#
+branch bcc, \ carry clear: OK
1 lda,# \ carry set: report error
:+
w ldx, inx, inx, inx,
lsb sta,x
0 lda,# msb sta,x \ push result
;code

\ Close a file
code close ( file# -- )
txa, pha,
lsb lda,x \ x = file#
$ffc3 jsr, \ CLOSE
pla, tax, inx,
;code
