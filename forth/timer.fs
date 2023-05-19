\ jiffy clock timer

code start ( -- clk )
dex, sei,
$a1 lda, msb sta,x
$a2 lda, cli, lsb sta,x end-code

\ stop & print elapsed time
: stop ( clk -- )
start swap - base @ swap decimal
#60 /mod s>d <# '.' hold #s #> type
#1000 #60 */ s>d <# # # # #> type
base ! ;

( : timertest ." $1000 loops..."
start $1000 0 do loop stop ." s" cr ;
timertest )
