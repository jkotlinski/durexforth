: id. ( header -- )
2+ dup 1+ swap c@ 1f and type space ;
: cfa> ( codepointer -- word )
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
['] litstring of
    [char] s emit
    [char] " emit space
    2+ dup 2+ over @ type
    [char] " emit space
    dup @ +
endof
['] ['] of
    ." ['] "
    2+ dup @ cfa> id.
endof
['] (loop) of
    ." (loop) " 2+
endof
['] branch of
    ." branch ( "
    2+ dup @ .
    ." ) "
endof
['] 0branch of
    ." 0branch ( "
    2+ dup @ .
    ." ) "
endof ( default )
    dup
    cfa> id.
endcase
2+ ;

: see
	loc ?dup 0= if exit then
	here
	latest @
	begin
		2 pick
		over
		<>
	while
		nip
		dup @
	repeat
	
	drop
	swap ( end-of-word start-of-word )

	[char] : emit space dup id.
	dup 2+ c@ 80 and if ." immediate " then

	>cfa

	begin
		2dup >
	while
		dup c@ case 
        20 of see-jsr endof
        4c of ." jmp( " see-jsr ." ) " endof
        e8 of 1+ ." inx " endof
        60 of 1+ ." rts " endof
        ." ? " swap 1+ swap
        endcase
	repeat
	[char] ; emit cr
	2drop
;

variable last-dump

: c. dup fff0 and 0= if
[char] 0 emit then . ;
: dump ( addr -- )
8 0 do dup u. space
dup 8 0 do dup c@ c. 1+ loop drop
8 0 do dup c@
dup bl [char] ] within 0= if
drop [char] .
then emit 1+ loop cr loop
last-dump ! ;

: n last-dump @ dump ;

: more d6 c@ 18 = if ." <more>"
key drop page then ;
: words
page latest @ begin ?dup while
more dup id. @ repeat cr ;

\ size foo prints size of foo
: size ( -- )
loc >r
here latest @ \ prev curr
begin dup while
dup r@ = if
- . r> drop exit then
nip dup @ repeat
. drop r> drop ;

