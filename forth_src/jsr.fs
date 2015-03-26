

( Calls Basic/Kernal routines.
  Uses ar, xr, yr,sr for register I/O.
  The Forth stack is temporarily 
  stored away so that CHRGET is in 
  place for Basic calls. )
30c value ar 30d value xr
30e value yr 30f value sr
1d allot dup >r >r
:asm jsr ( addr -- )
sp0 lda,x 14 sta, sp1 lda,x 15 sta,
txa, pha,
1c ldx,# :-
73 lda,x r> sta,x # save zp
e3a2 lda,x 73 sta,x # restore CHRGET
dex, -branch bpl,
e130 jsr, # perform [sys]
1c ldx,# :- 
r> lda,x 73 sta,x # restore zp
dex, -branch bpl,
pla, tax, inx, ;asm
