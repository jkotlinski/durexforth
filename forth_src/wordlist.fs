: hide ( "name" -- )
parse-name find-name ?dup if
dup latest @ - ( nt size )
>r c@ $1f and 3 + ( off )
latest @ swap over +  ( srca dsta )
dup latest !
r> move then ;
: defcode ( "name" -- )
parse-name find-name ?dup 0=
abort" not found."
count $1f and + here swap ! ;
: define defcode 0 ] ;
