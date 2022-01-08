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

$ffc0 jsr,   \ OPEN
'1' @@ bcc,  \ carry set = error or rs232
$ba ldx, 2 cpx,# 
'2' @@ bne,  \ rs232 ?
$f0 cmp,     \ rs232 go on that error
'3' @@ bne, 
'1' @:       \ carry clear or rs232 = $f0
0 lda,#      \ A is only valid on error
'2' @:       \ not rs232 and carry set
'3' @:       \ carry set and rs232 <> $f0
w ldx,
inx, inx, inx,
lsb sta,x
0 lda,# msb sta,x
;code

\ Close a logical file
code close ( file# -- )
txa, pha,
lsb lda,x \ x = file#
$ffc3 jsr, \ CLOSE
pla, tax, inx,
;code
