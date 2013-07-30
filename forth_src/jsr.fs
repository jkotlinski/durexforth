

( Used to make Basic/Kernal calls.
  Variables ar, xr, yr are used for
  register I/O. )
var ar var xr var yr 
:asm jsr
0 lda,x here 1+ 1234 sta, # lsb
1 lda,x here 1+ 1234 sta, # msb
txa, pha,
ar lda, xr ldx, yr ldy,
here 2+ swap ! here 1+ swap !
1234 jsr,
ar sta, xr stx, yr sty,
pla, tax, inx, inx, ;asm
hide T
