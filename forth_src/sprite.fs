

here @
80 c, 40 c, 20 c, 10 c,
8 c, 4 c, 2 c, 1 c,
: 80lsr literal + c@ ;
: setbit ( n addr -- )
swap 80lsr over c@ or swap c! ;
: clrbit ( n addr -- )
swap 80lsr invert over c@ and swap c! ;

: sp-x! ( x n -- )
2dup 2* d000 + c! # lsb
swap 100 and if d010 setbit
else d010 clrbit then ;

: sp-y! ( y n -- ) 2* d001 + c! ;

: sp-xy! ( x y n -- )
tuck sp-y! sp-x! ;

( expand width/height )
: sp-1w ( n -- ) d01d clrbit ;
: sp-2w ( n -- ) d01d setbit ;
: sp-1h ( n -- ) d017 clrbit ;
: sp-2h ( n -- ) d017 setbit ;

( multicolor )
: sp-mc ( n -- ) d01c setbit ;
: sp-1c ( n -- ) d01c clrbit ;

( multicolor registers )
: sp-mc0! ( c -- ) d022 ! ;
: sp-mc1! ( c -- ) d023 ! ;

( bg priority )
: sp-front ( n -- ) d01b clrbit ;
: sp-back ( n -- ) d01b setbit ;

: sp-on ( n -- ) d015 setbit ;
: sp-off ( n -- ) d015 clrbit ;

: sp-col! ( c n -- ) d027 + c! ;

: sp-init
( assume screen at $400, lowres,
place sprites at 3e00-3fff )
f8 7f8 ! f9 7f9 ! fa 7fa ! fb 7fb !
fc 7fc ! fd 7fd ! fe 7fe ! ff 7ff ! ;

: demo
sp-init
7 begin 
dup sp-on
1 over + over sp-col!
?dup while 1- repeat

18 33 0 sp-xy!
18 19 + 33 1 sp-xy!
18 19 2 * + 33 2 sp-xy!
18 19 3 * + 33 3 sp-xy!
18 19 4 * + 33 4 sp-xy!
18 19 5 * + 33 5 sp-xy!
18 19 6 * + 33 6 sp-xy!
18 19 7 * + 33 7 sp-xy!

begin
1 7f8 +! 1 7f9 +! 1 7fa +! 1 7fb +!
1 7fc +! 1 7fd +! 1 7fe +! 1 7ff +!
again ;

demo
