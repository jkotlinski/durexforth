

# lindenmayer systems

s" turtle" load

: Da 0 ; # delta angle
: Dd 0 ; # delta distance
var rule var rulel

: dofract ( depth -- depth )
0 begin dup rulel < while
dup rule + c@ case
[ key f literal ] of
over if
swap 1- recurse 1+ swap
else Dd forward then
endof
[ key + literal ] of Da right endof
[ key - literal ] of Da left endof
[ key [ literal ] of turtle@ >r >r >r
endof
[ key ] literal ] of r> r> r> turtle!
endof
endcase
1+ repeat drop ;

: fractal
( ax axl depth Dd Da rule rulel -- )
to rulel to rule to Da to Dd
0 # axiom axioml depth i
begin 2 pick over > while
3 pick over + c@ case
[ key f literal ] of over if
swap 1- dofract 1+ swap
else Dd forward then endof
[ key + literal ] of Da right endof
[ key - literal ] of Da left endof
endcase
1+ repeat 2drop drop ;

: koch init 10 clrcol
50 30 0 turtle!
s" f++f++f" 4 2 3c s" f-f++f-f" fractal ;
: bush1 init d clrcol
a0 bb 10e turtle!
s" f" 4 3 19 s" ff+[+f-f-f]-[-f+f+f]" fractal ;
