\ included into base.fs

: hide ( "name" -- )
parse-name find-name ?dup if
dup latest - ( nt size )
>r c@ $1f and 3 + ( off )
latest swap over +  ( srca dsta )
dup to latest
r> move then ;
: defcode ( "name" -- )
parse-name 2dup find-name ?dup 0=
if notfound then nip nip
count $1f and + here swap ! ;
: define defcode ] ;
