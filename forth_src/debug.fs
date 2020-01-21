: name>string ( word -- caddr u )
2+ dup 1+ swap c@ $1f and ;
: xt> ( codepointer -- word )
latest @ begin ?dup while
2dup > if nip exit then
@ repeat drop 0 ;

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

: see
bl word find 0= if
rvs count type '?' emit abort then
here latest @
begin 2 pick over <
while nip dup @ repeat

rot drop \ eow sow

':' emit space dup name>string type space
dup 2+ c@ $80 and if ." immediate " then

>xt

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

: more $d6 c@ $18 = if $12 emit
." more" $92 emit key drop page then ;
: words
page latest @ begin ?dup while
more dup name>string type space @ repeat cr ;

\ size foo prints size of foo
: size ( -- )
bl word find drop >r
here latest @ \ prev curr
begin dup while
dup r@ < if
- . r> drop exit then
nip dup @ repeat
. drop r> drop ;

