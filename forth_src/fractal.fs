

# lindenmayer systems

s" turtle" load

: Da 0 ; # delta angle
: Dd 0 ; # delta distance
var rule var rulel

var depth

: dofract ( -- )
0 begin dup rulel < while
dup rule + c@ case
[ key f literal ] of
depth @ if
ffff depth +! recurse 1 depth +!
else Dd forward then
endof
[ key + literal ] of Da right endof
[ key - literal ] of Da left endof
endcase
1+ repeat drop ;

: fractal
( ax axl depth Dd Da rule rulel -- )
to rulel to rule to Da to Dd depth !
0 # axiom axioml i
begin 2dup > while
2 pick over + c@ case
[ key f literal ] of depth @ if
ffff depth +! dofract 1 depth +!
else Dd forward then endof
[ key + literal ] of Da right endof
[ key - literal ] of Da left endof
endcase
1+ repeat 2drop drop ;

: koch init 10 clrcol
50 30 0 turtle!
s" f++f++f" 4 2 3c s" f-f++f-f" fractal ;
