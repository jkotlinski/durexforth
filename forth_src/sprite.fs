here
80 c, 40 c, 20 c, 10 c,
8 c, 4 c, 2 c, 1 c,
: 80lsr literal + c@ ;
: setbit ( n addr -- )
swap 80lsr over c@ or swap c! ;
: clrbit ( n addr -- )
swap 80lsr invert over c@ and swap c! ;
hide 80lsr

: sp-x! ( x n -- )
2dup 2* d000 + c! \ lsb
swap 100 and if d010 setbit
else d010 clrbit then ;

: sp-y! ( y n -- ) 2* d001 + c! ;

: sp-xy! ( x y n -- )
tuck sp-y! sp-x! ;

( expand width/height )
: sp-1w ( n -- ) 7 swap - d01d clrbit ;
: sp-2w ( n -- ) 7 swap - d01d setbit ;
: sp-1h ( n -- ) 7 swap - d017 clrbit ;
: sp-2h ( n -- ) 7 swap - d017 setbit ;

: sp-on ( n -- ) d015 setbit ;
: sp-off ( n -- ) d015 clrbit ;

: sp-col! ( c n -- ) d027 + c! ;

( read sprite byte )
: ks getc bl <> and or ;
: rdb ( addr -- addr )
0 80 ks 40 ks 20 ks 10 ks
8 ks 4 ks 2 ks 1 ks
over c! 1+ ;

( read sprite line )
: rdl rdb rdb rdb getc drop ;

( read sprite to address )
: sp-data ( addr -- )
rdl rdl rdl rdl rdl rdl rdl
rdl rdl rdl rdl rdl rdl rdl
rdl rdl rdl rdl rdl rdl rdl drop ;

hide rdl hide rdb hide ks
hide setbit hide clrbit
