

: fac, bbca jsr
57 here 5 cmove here 5 + to here ;

# 5-byte float word from string
: strfac ( str strlen -- )
ar ! 22 ! b7b5 jsr create fac, ;
# 5-byte float word from signed int
: intfac ( s -- ) create
dup 100/ ar ! yr ! b391 jsr fac, ;
# s" 3.14159" strfac pi
# 4 intfac 4.
# 8 negate intfac -8.
: fac@ ( faddr -- )
dup 100/ yr ! ar ! bba2 jsr ;
: fac* ( faddr -- )
dup 100/ yr ! ar ! ba28 jsr ;
: fac. bddd jsr b487 jsr ab21 jsr ;
# : test 4. fac@ -8. fac* fac. ;

