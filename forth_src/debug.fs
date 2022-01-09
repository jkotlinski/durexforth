:noname ( xt xt-1 nt -- xt xt-1 1 | xt xt-1 0 )
>xt dup 3 pick
> if ( xt xt-1 xt0 )
2dup < if drop else
nip then 1 exit then drop 1 ;

: size ( word -- )
' here literal dowords
swap - . cr ;

variable last-dump

: dump ( addr -- )
base @ swap hex
8 0 do dup u.
dup 8 0 do dup c@ 0 <# # # #> type
space 1+ loop drop
8 0 do dup c@
dup $7f and $20 < if drop '.' then
emit 1+ loop cr loop
last-dump ! base ! ;
: n last-dump @ dump ;

: more
$d6 c@ $18 = if $12 emit
." more" $92 emit key drop page then ;

: name>string ( nametoken -- caddr u )
count $1f and ;
:noname more name>string type space 1 ;
: words page literal dowords ;
