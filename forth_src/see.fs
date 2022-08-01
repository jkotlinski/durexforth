( decompile forth word and print to
screen. try "see see". watch out:
hidden words are not supported! )

:noname 2dup name>string + @
< invert if swap drop 0
exit then drop 1 ;
: xt> literal dowords ;

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

: (see)
>xt dup 3 pick
> if ( xt xt-1 xt0 )
2dup < if drop else
nip then 1 exit then drop 1 ;

: see
bl word find 0= if notfound then
here ['] (see) dowords
swap

':' emit space dup xt> dup
name>string type space
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
