

here @
80 c, 40 c, 20 c, 10 c,
8 c, 4 c, 2 c, 1 c,
: 80lsr literal + c@ ;
: setbit ( n addr -- )
swap 80lsr over c@ or swap c! ;
: clrbit ( n addr -- )
swap 80lsr invert over c@ and swap c! ;
hide 80lsr

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

( bg priority )
: sp-front ( n -- ) d01b clrbit ;
: sp-back ( n -- ) d01b setbit ;

: sp-on ( n -- ) d015 setbit ;
: sp-off ( n -- ) d015 clrbit ;

: sp-col! ( c n -- ) d027 + c! ;

( read sprite byte )
: ks key bl <> and ;
: rdb ( addr -- addr )
80 ks 40 ks or 20 ks or 10 ks or
8 ks or 4 ks or 2 ks or 1 ks or
over c! 1+ ;

( read sprite line )
: rdl rdb rdb rdb key drop ;

( read sprite to address )
: sp-data ( addr -- )
rdl rdl rdl rdl rdl rdl rdl
rdl rdl rdl rdl rdl rdl rdl
rdl rdl rdl rdl rdl rdl rdl drop ;

hide rdl hide rdb hide ks
hide setbit hide clrbit

( demo

340 sp-data
DDD  UU U RRR  EEEEX   X
DDD  UU U RRR  EEEEX   X
D DD UU U R RR E    X X 
D DD UU U R RR E    X X 
D  D UU U RRR  EEE   X  
D  D UU U RRR  EEE   X  
D  D UU U R RR E    X X 
D  D UU U R RR E    X X 
DDD   UUU R RR EEEEX   X
DDD   UUU R RR EEEEX   X
                        
FFFF  OO  RRR TTTTTTH  H
FFFF OOOO RRR TTTTTTH  H
FF   O  O R RR  TT  H  H
FF   O  O R RR  TT  H  H
FFFF O  O RRR   TT  HHHH
FFFF O  O RRR   TT  HHHH
FF   O  O R RR  TT  H  H
FF   O  O R RR  TT  H  H
FF    OO  R RR  TT  H  H
FF    OO  R RR  TT  H  H

18 33 0 sp-xy!
18 19 + 33 1 sp-xy!
18 19 2 * + 33 2 sp-xy!
18 19 3 * + 33 3 sp-xy!
18 19 4 * + 33 4 sp-xy!
18 19 5 * + 33 5 sp-xy!
18 19 6 * + 33 6 sp-xy!
18 19 7 * + 33 7 sp-xy!

: setup
7 begin 
340 40 / over 7f8 + c!
dup sp-on
1 over + over sp-col!
?dup while 1- repeat ; setup
)
