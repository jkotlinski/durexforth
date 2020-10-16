: name>string ( nametoken -- caddr u )
count $1f and ;

: (xt>) ( xt1 nt -- nt xt 0 | xt1 1 )
2dup name>string + @
< invert if swap drop 0  
exit then drop 1 ;

: xt> ( codepointer -- nametoken )
['] (xt>) dowords ;

: see-jsr
1+ dup @
case
['] lit of
    2+ dup @ .
endof
['] litc of
    2+ dup c@ . 1-
endof
['] lits of
    's' emit
    '"' emit space
    2+ dup 2+ over @ type
    '"' emit space
    dup @ +
endof
['] (loop) of
    ." (loop) " 2+
endof
['] (of) of
    ." (of) " 2+
endof
['] branch of
    ." branch( "
    2+ dup @ over - .
    ." ) "
endof
['] 0branch of
    ." 0branch( "
    2+ dup @ over - .
    ." ) "
endof ( default )
    dup xt> name>string type
    dup dup xt> >xt
    2dup <> if '+' emit - .
    else 2drop space then
endcase
2+ ;

\ finding the next closest word requires scanning
\ the whole dictionary, sadly
: (see) ( xt xt-1 nt -- xt xt-1 1 | xt xt-1 0 )
>xt dup 3 pick 
> if ( xt xt-1 xt0 )
2dup < if drop else
nip then 1 exit then drop 1 ;

\ size foo prints size of foo

: size ( word -- )
' here ['] (see) dowords
swap - . cr ; 

: see
bl word find 0= if
rvs count type '?' emit abort then
( xt )
here ['] (see) dowords
swap

':' emit space dup xt> dup name>string type space
2+ c@ $80 and if ." immediate " then
begin
    2dup >
while
    dup c@ case
    $20 of see-jsr endof
    $4c of ." jp( " see-jsr ." ) " endof
    $e8 of 1+ ." drop " endof \ inx
    $60 of 1+ ." exit " endof \ rts
    ." ? " swap 1+ swap
    endcase
repeat
';' emit cr 2drop ; 

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

: (words) more name>string type space 1 ;
: words ['] (words) dowords ;
