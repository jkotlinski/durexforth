

( Used to make Basic or Kernal calls.
  Variables ar, xr, yr are used for
  register I/O. $100-$11c is used as
  temporary storage for the Forth
  zeropage, so that CHRGET can be in
  place for any Basic calls. )
var ar var xr var yr 
:asm jsr
0 lda,x here 1+ 1234 sta, # lsb
1 lda,x here 1+ 1234 sta, # msb
txa, pha,
1c ldx,# :-
73 lda,x 100 sta,x # save zp
e3a2 lda,x 73 sta,x # restore CHRGET
dex, -branch bpl,
ar lda, xr ldx, yr ldy,
here 2+ swap ! here 1+ swap !
1234 jsr,
ar sta, xr stx, yr sty,
1c ldx,# :-
100 lda,x 73 sta,x # restore zp
dex, -branch bpl,
pla, tax, inx, inx, ;asm
