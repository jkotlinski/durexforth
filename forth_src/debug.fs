

: id.
	2+ ( skip over link ptr )
	dup c@
	3f and ( 3f = length mask )
	begin
		dup
	while
		swap 1+
		dup c@
		emit
		swap 1-
	repeat
	2drop
;

: cfa>
	latest @ ( start at latest dictionary entry... )
	begin
		?dup ( while link ptr != 0 )
	while
		2dup swap ( cfa curr curr cfa )
		< if ( current dictionary entry < cfa ? )
			nip ( leave it on stack )
			exit
		then
		@ ( follow link ptr back )
	repeat
	drop
	0
;

: see
	word find
	here @
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

	[ char : ] literal emit space dup id. space
	dup ?immed if ." immed " then

	>dfa ( get data addr )

	begin
		2dup >
	while
		dup @

		case
		' lit of
			2 + dup @ .
		endof
		' litstring of
			[ char s ] literal emit '"' emit space
			2 + dup c@ 
			swap dup tell
			'"' emit space
			+ 1-
		endof
		' ' of
			[ char ' ] literal emit space
			2 + dup @
			cfa>
			id. space
		endof
		' branch of
			." branch ( "
			2 + dup @ .
			." ) "
		endof
		' 0branch of
			." 0branch ( "
			2 + dup @ .
			." ) "
		endof
		' exit of
			2dup
			2 +
			<> if
				." exit "
			then
		endof
			( default )
			dup
			cfa>
			id. space
		endcase
		2 +
	repeat
	[ char ; ] literal emit cr
	2drop
;

( c a b within returns true if a <= c and c < b )
: within -rot over
<= if > else 2drop 0 then ;

var last-dump

: dump ( addr -- )
	8 ( addr lines )
	begin
		?dup ( while lines > 0 )
	while
		over . space ( print addr )

		8 ( addr lines bytes )
		begin
			?dup ( while bytes > 0 )
		while
			rot ( lines bytes addr )
			dup c@ c. space ( print *addr )

			1+ ( incr addr )
			-rot ( addr lines bytes )

			1-
		repeat
		( addr lines )

		swap 8 - swap ( roll back address )

		( ... emit chars ... )

		8 ( addr lines bytes )
		begin
			?dup ( while bytes > 0 )
		while
			rot ( lines bytes addr )
			dup c@
			dup 20 5e within if
				emit
			else
				drop [ char . ] literal emit
			then

			1+ ( incr addr )
			-rot ( addr lines bytes )

			1-
		repeat

		cr
		1-
	repeat
	last-dump !
;

: n last-dump @ dump ;

: words
	latest @
	begin
		?dup
	while
		dup ?hidden not if
			dup id. space
		then
		@
	repeat
	cr
;

: sizes
	here @
	latest @ ( here *latest )
	begin
		?dup
	while
		dup ?hidden not if
			2dup - ( latest *latest diff )
			.  ( latest *latest )
			dup id. cr
			swap drop ( *latest )
		then
		dup @
	repeat
	drop
;

