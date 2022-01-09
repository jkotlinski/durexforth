: s= ( caddr u caddr u -- flag )
  rot
  over <> if 2drop drop 0 exit then
  0 do
    2dup i + c@ swap
    i + c@ <> if 2drop 0 unloop exit then
  loop
  2drop -1 ;

: find-name ( caddr u -- nt | 0 )
  latest begin
    dup c@ while
    dup name>string ( caddr u naddr caddr u )
    4 pick 4 pick s= if nip nip exit then
    dup c@ $3f and + 3 +
  repeat
  2drop drop 0 ;

: hide ( "name" -- )
parse-name find-name ?dup if
dup latest - ( nt size )
>r c@ $1f and 3 + ( off )
latest swap over +  ( srca dsta )
dup to latest
r> move then ;
: defcode ( "name" -- )
parse-name find-name ?dup 0=
abort" not found."
count $1f and + here swap ! ;
: define defcode ] ;
