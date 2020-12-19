\ Open a file, no error handling
\ returns 0 on success, file # on error
( nameaddr namelen file# sa -- 
  file# result )
code open?
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
inx, inx,
lsb sta,x
lsb 1- lda,x lsb 1+ sta,x
0 lda,# msb sta,x msb 1+ sta,x
;code

\ Close a file
code close ( file# -- )
txa, pha,
lsb lda,x \ x = file#
$ffc3 jsr, \ CLOSE
pla, tax, inx,
;code
