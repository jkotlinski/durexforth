: fac, bbca sys
57 here 5 cmove here 5 + to here ;
# 5-byte float word from string
: strf ( str strlen -- )
ar ! 22 ! b7b5 sys create fac, ;
# 5-byte float word from signed int
: intf ( s -- ) create
dup 100/ ar ! yr ! b391 sys fac, ;
: fac! ( faddr -- )
dup 100/ yr ! ar ! bba2 sys ;
: fac* ( faddr -- )
dup 100/ yr ! ar ! ba28 sys ;
: fac. bddd sys b487 sys ab21 sys ;

( example:

s" .5" strf .5
s" .8" strf .8
.5 fac! .8 fac* fac.

...prints .4! )
