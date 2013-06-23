

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

: sp-show ( n -- ) d015 setbit ;
: sp-hide ( n -- ) d015 clrbit ;

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

: sp-col! ( c n -- ) d027 + ! ;
