\ jiffy clock

hex

\ resets jiffy clock
: clkreset ( -- )
0 a1 [ sei, ] ! [ cli, ] ;

\ reads jiffy clock
code clkget ( -- clk )
dex, sei,
a1 lda, msb sta,x
a2 lda, cli, lsb sta,x ;code

\ prints seconds using format "14.325"
: clkprint ( clk -- )
base @ swap decimal
#60 /mod s>d <# '.' hold #s #> type
#100 * #6 / s>d <# # # # #> type
base ! ;

( : clktest clkreset
begin clkget clkprint cr again ;
clktest )
