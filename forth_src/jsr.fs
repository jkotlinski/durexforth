

( Jumps to Basic/Kernal methods.
  Use ar, xr, yr, sr for register 
  I/O. )
30c value ar 30d value xr
30e value yr 30f value sr
:asm jsr
0 lda,x 14 sta, 1 lda,x 15 sta,
txa, pha, e130 jsr,
pla, tax, inx, inx, ;asm
