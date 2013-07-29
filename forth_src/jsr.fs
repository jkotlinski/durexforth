

var ar var xr var yr 
( For BASIC CHRGET support,
  please include all out-
  commented code lines!

  1d allot value jsrtmp )
:asm jsr
0 lda,x here 1+ 1234 sta, # lsb
1 lda,x here 1+ 1234 sta, # msb
txa, pha,
( 1c ldx,# :-
  73 lda,x jsrtmp sta,x
  e3a2 lda,x 73 sta,x
  dex, -branch bpl, )
ar lda, xr ldx, yr ldy,
here 2+ swap ! here 1+ swap !
1234 jsr,
ar sta, xr stx, yr sty,
( 1c ldx,# :-
  jsrtmp lda,x 73 sta,x
  dex, -branch bpl, )
pla, tax, inx, inx, ;asm
