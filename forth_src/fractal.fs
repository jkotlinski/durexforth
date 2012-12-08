

# lindenmayer systems

s" turtle" load

0 value Da # delta angle
0 value Dd # delta distance

var rule var rulel var scale

: dofract ( depth -- depth )
scale @ 100 <> if
Dd dup >r scale @ * 100/ to Dd then
0 begin dup rulel < while
dup rule + c@ case
[ key f literal ] of over if
swap 1- recurse 1+ swap
else Dd forward then
endof
[ key @ literal ] of Dd forward endof
[ key + literal ] of Da right endof
[ key - literal ] of Da left endof
[ key [ literal ] of turtle@ >r >r >r
endof
[ key ] literal ] of r> r> r> turtle!
endof endcase 1+ repeat drop
scale @ 100 <> if r> to Dd then ;

: fractal ( ax axl depth scale Dd Da
rule rulel -- )
to rulel to rule to Da to Dd scale !
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
20 4c 0 moveto
s" f" 3 100 9 3c s" f-f++f-f" fractal
20 88 0 moveto
s" f" 4 100 3 3c s" f-f++f-f" fractal
20 c4 0 moveto
s" f" 5 100 1 3c s" f-f++f-f" fractal ;
: weed1 init d clrcol
a0 c4 10e moveto
s" f" 3 100 7 19 s" f[-f]f[+f]f"
fractal ;
: bush1 init d clrcol
a0 bb 10e moveto
s" f" 4 100 3 19
s" ff+[+f-f-f]-[-f+f+f]" fractal ;
: bush2 init d clrcol d d020 c!
a0 c8 10e moveto
s" f" 6 80 64 14
s" @[+f]@[-f]+f" fractal ;
