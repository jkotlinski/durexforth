  

: 2dup over over ;
: bl 20 ;
: space bl emit ;
: cr d emit ;
: / /mod swap drop ;
: mod /mod drop ;
: literal immed ' lit , , ;
: loc word find ;
: [compile] immed loc >cfa , ;
: ['] immed ' lit , ;
: jsr-docol, 20 c, docol , ;
: if immed ' 0branch , here @ 0 , ;
: then immed dup here @ swap - swap ! ;
: else immed ' branch , here @ 0 ,
swap dup here @ swap - swap ! ;
: begin immed here @ ;
: until immed ' 0branch , here @ - , ;
: again immed ' branch , here @ - , ;
: while immed ' 0branch , here @ 0 , ;
: repeat immed ' branch , swap here @ - , dup here @ swap - swap ! ;
: recurse immed latest @ >cfa , ;
: ( immed begin key [ char ) ] literal = until ;
: # immed begin key d = until ; # comment
: -rot rot rot ;
: nip ( x y -- y ) swap drop ;
: tuck ( x y -- y x y ) dup -rot ;
: pick ( x_u ... x_1 x_0 u -- x_u ... x_1 x_0 x_u ) 1+ 2 * sp@ + @ ;
: invert ffff xor ;
: negate invert 1+ ;
: ?dup dup if dup then ;
: not 0= ;
: <> ( n -- b ) = not ;
: > ( n -- b ) swap < ;

: 0> ( n -- b ) 0 > ;
: <= > not ;
: >= < not ;
: 2+ 1+ 1+ ;
: max ( a b - c )
2dup < if swap then drop ;
: min ( a b - c )
2dup > if swap then drop ;
: cells dup + ;

: tell 
	dup c@ ( get strlen )
	begin 
		swap 1+ ( inc strptr )
		dup c@ emit ( print char )
		swap 1- ( dec strlen )
		dup 0= 
	until
	2drop
;
: '"' [ char " ] literal ;
: s" immed ( -- addr len )
	state @ if
		' litstring ,
		here @ ( save addr of length byte on stack )
		0 c, ( dummy length - we don't know what it is yet )
		begin
			key
			dup '"' <>
		while
			c,
		repeat
		drop
		dup
		here @ swap -
		1- ( subtract to compensate for length byte )
		swap c!
	else ( immediate mode )
		here @
		begin
			key
			dup '"' <>
		while
			over c!
			1+
		repeat
		drop
		here @ -
		here @
		swap
	then
;
: ." immed ( -- )
	state @ if ( compiling? )
		[compile] s"
		' tell ,
	else
		( in immediate mode, just read and print chars )
		begin
			key
			dup '"' = if
				drop
				exit
			then
			emit
		again
	then
;
." compile base.."

: case immed 0 ;
: of immed ' over , ' = , [compile] if ' drop , ;
: endof immed [compile] else ;
: endcase immed ' drop , begin ?dup while [compile] then repeat ;

: forget loc ?dup if dup @ latest ! here ! then ;

: .s ( -- )
	sp0 1- 1-
	begin
		dup sp@ 2+ >
	while
		dup @ .
		1- 1-
	repeat
	drop
;

: ?hidden
	2+ ( skip link ptr )
	c@
	40 and
;

: ?immed
	2+ ( skip link ptr )
	c@
	80 and
;

: +! ( num addr -- ) 
	dup @ ( num addr val )
	rot + ( addr val num )
	swap ( newval addr )
	!
;

( get pointer to first data field - skip jsr DOCOL )
: >dfa >cfa 1+ 2+ ;

: allot ( n -- addr )
	here @ ( n val )
	swap ( val n )
	here +!
;

: cells ( n -- n ) dup + ; ( 2* )

: var
	1 cells allot
	create
	jsr-docol,
	' lit , , ' exit ,
;

: value ( n -- )
	create
	jsr-docol,
	' lit , , ' exit ,
;

: to immed ( n -- )
	loc >dfa 2+
	state @ if
		' lit , , ' ! ,
	else
		!
	then
;

: hide loc hidden ;
: hide-to  ( -- )
loc latest
begin @ dup hidden 2dup = until
2drop ;

: load ( strptr strlen -- old-compile old-ae )
	compile @ -rot ( compile strptr strlen )
	ae @ -rot ( compile ae strptr strlen )
	2 pick 1+ ( compile ae strptr strlen dst )
	loadb ( old-compile old-ae )
	drop # ignore status
	dup 1+ compile !
	load-depth @ 0= if
		2drop # no need to keep this
	then
	1 load-depth +!
;

: save-forth ( strptr strlen -- )
	compile @ -rot
	0 compile !
	801
	here @
	2swap
	saveb	
	compile !
;

: modules ;
# ." debug.."
# s" debug" load
." asm.."
s" asm" load
." gfx.."
s" gfx" load
." vi.."
s" vi" load

: scratch ( strptr strlen -- )
tuck ( strlen strptr strlen )
dup here @ + ( strlen strptr strlen tmpbuf )
dup >r
dup [ char s ] literal swap c! 1+
dup [ char : ] literal swap c! 1+
( strlen strptr strlen tmpbuf+2 )
rot ( strlen strlen tmpbuf+2 strptr )
swap ( strlen strlen strptr tmpbuf+2 )
rot ( strlen strptr tmpbuf+2 strlen )
cmove ( strlen )
r> swap 2+ ( tmp strlen )
openw
closew
;

." scratch old durexforth.."
s" durexforth" scratch

." save new durexforth.."
s" durexforth" save-forth

." done!" cr
