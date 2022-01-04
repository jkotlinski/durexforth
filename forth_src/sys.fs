( Calls Basic/Kernal routines.
  Uses ar/xr/yr/sr for register I/O. )
$30c value ar $30d value xr
$30e value yr $30f value sr
code sys ( addr -- )
lsb lda,x $14 sta, msb lda,x $15 sta,
txa, pha,
$e130 jsr, \ perform [sys]
pla, tax, inx, ;code

code bank ( u -- )
lsb     lda,x
w       sta,
$01     lda,
$f8     and,#
w       ora,
$01     sta,
        inx,
;code        

\ bank  A000  D000  D800  E000
\ 0     RAM   RAM   RAM   RAM
\ 1     RAM   CHARG CHARG RAM
\ 2     RAM   CHARG CHARG ROM
\ 3     ROM   CHARG CHARG ROM
\ 4     RAM   RAM   RAM   RAM
\ 5     RAM   I/O   NYBLE RAM
\ 6     RAM   I/O   NYBLE ROM
\ 7     ROM   I/O   NYBLE ROM

