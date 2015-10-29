( Calls Basic/Kernal routines.
  Uses ar/xr/yr/sr for register I/O. )
30c value ar 30d value xr
30e value yr 30f value sr
code sys ( addr -- )
sp0 lda,x 14 sta, sp1 lda,x 15 sta,
txa, pha,
e130 jsr, \ perform [sys]
pla, tax, inx, ;code
