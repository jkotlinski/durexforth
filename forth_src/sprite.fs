here
$80 c, $40 c, $20 c, $10 c,
8 c, 4 c, 2 c, 1 c,
: 80lsr [ swap ] literal + c@ ;
: setbit ( n addr -- )
swap 80lsr over c@ or swap c! ;
: clrbit ( n addr -- )
swap 80lsr invert over c@ and swap c! ;

: sp-x! ( x n -- )
2dup 2* $d000 + c! \ lsb
swap $100 and if $d010 setbit
else $d010 clrbit then ;

: sp-y! ( y n -- ) 2* $d001 + c! ;

: sp-xy! ( x y n -- )
tuck sp-y! sp-x! ;

: 7s- 7 swap - ;

( expand width/height )
: sp-1w ( n -- ) 7s- $d01d clrbit ;
: sp-2w ( n -- ) 7s- $d01d setbit ;
: sp-1h ( n -- ) 7s- $d017 clrbit ;
: sp-2h ( n -- ) 7s- $d017 setbit ;

: sp-on ( n -- ) 7s- $d015 setbit ;
: sp-off ( n -- ) 7s- $d015 clrbit ;

: sp-col! ( c n -- ) $d027 + c! ;

( read sprite byte )
: ks
2* source drop >in @ + c@
1 >in +! '.' <> 1 and or ;
: rdb ( addr -- addr )
0 ks ks ks ks ks ks ks ks
over c! 1+ ;

( read sprite to address )
: sp-data ( addr -- )
#21 0 do refill rdb rdb rdb loop drop ;
