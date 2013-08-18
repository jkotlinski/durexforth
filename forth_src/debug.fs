

: id. ( header -- )
2+ dup 1+ swap c@ 3f and tell space ;
: cfa> ( codepointer -- word )
latest @ begin ?dup while
2dup > if nip exit then
@ repeat drop 0 ;

: see
	loc
    dup 0= if exit then
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
	dup 2+ c@ 80 and if ." immed " then

	>dfa ( get data addr )

	begin
		2dup >
	while
		dup @

		case
		' lit of
			2+ dup @ .
		endof
		' litc of
			2+ dup c@ . 1-
		endof
		' litstring of
			[char] s emit
            [char] " emit space
			2+ dup 1+ over c@ tell
			[char] " emit space
            dup c@ + 1-
		endof
		' ' of
			[char] ' emit space
			2+ dup @
			cfa> id.
		endof
		' (loop) of
			." (loop) ( "
			2+ dup @ .
			." ) "
		endof
		' branch of
			." branch ( "
			2+ dup @ .
			." ) "
		endof
		' 0branch of
			." 0branch ( "
			2+ dup @ .
			." ) "
		endof
		' exit of
			2dup
			2+
			<> if
				." exit "
			then
		endof
			( default )
			dup
			cfa> id.
		endcase
		2+
	repeat
	[char] ; emit cr
	2drop
;
hide (loop)

( c a b within returns true if a <= c and c < b )
: within -rot over
<= if > else 2drop 0 then ;

var last-dump

: c. dup fff0 and 0= if
[char] 0 emit then . ;
: dump ( addr -- )
8 0 do dup . space
dup 8 0 do dup c@ c. 1+ loop drop
8 0 do dup c@
dup 20 5e within 0= if
drop [char] .
then emit 1+ loop cr loop
last-dump ! ;

: n last-dump @ dump ;

: words
93 emit latest @ begin ?dup while
d6 c@ 18 = if ." <more>"
key drop 93 emit then
dup ?hidden 0= if
dup id.
then @ repeat cr ;

# size foo prints size of foo
: size ( -- )
loc >r
here latest @ # prev curr
begin dup while
dup r@ = if
- . r> drop exit then
nip dup @ repeat
. drop r> drop ;

