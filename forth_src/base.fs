  

7 287 c! 0 blink
: 2dup over over ;
: bl 20 ;
: space bl emit ;
: cr d emit ;
: nip swap drop ;
: / /mod nip ;
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
: ( immed begin key [ key ) ] literal = until ;
: # immed begin key d = until ; # comment
: -rot rot rot ;
: tuck ( x y -- y x y ) swap over ;
: pick ( x_u ... x_1 x_0 u -- x_u ... x_1 x_0 x_u ) 1+ 2 * sp@ + @ ;
: ?dup dup if dup then ;
: not 0= ;
: <> ( a b -- c ) = 0= ;
: > ( n -- b ) swap < ;

: <= > 0= ;
: >= < 0= ;
: 2+ 1+ 1+ ;
: max ( a b - c )
2dup < if swap then drop ;
: min ( a b - c )
2dup > if swap then drop ;

: tell ( addr len -- )
begin over c@ emit ( print char )
swap 1+ swap ( inc strptr )
1- ( dec strlen )
?dup 0= until drop ;
: '"' [ key " ] literal ;
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
		( read and print chars )
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

: ?hidden 2+ c@ 40 and ;
: ?immed 2+ c@ 80 and ;

( get pointer to first data field - skip jsr DOCOL )
: >dfa >cfa 1+ 2+ ;

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

: save-forth ( strptr strlen -- )
	compile-ram @ -rot
	0 compile-ram !
	801
	here @
	2swap
	saveb	
	compile-ram !
;

." asm.."
s" asm" load

:asm 2*
0 lda,x asla, 0 sta,x
1 lda,x rola, 1 sta,x ;asm
:asm 2/
1 lda,x lsra, 1 sta,x
0 lda,x rora, 0 sta,x ;asm
:asm or
1 lda,x 3 ora,x 3 sta,x
0 lda,x 2 ora,x 2 sta,x
inx, inx, ;asm
:asm xor
1 lda,x 3 eor,x 3 sta,x
0 lda,x 2 eor,x 2 sta,x
inx, inx, ;asm
: invert ffff xor ;
: negate invert 1+ ;
:asm +! ( num addr -- ) 
0 lda,x zptmp sta,
1 lda,x zptmp 1+ sta,
0 ldy,# clc,
zptmp lda,(y) 2 adc,x zptmp sta,(y)
iny,
zptmp lda,(y) 3 adc,x zptmp sta,(y)
inx, inx, inx, inx, ;asm

: allot ( n -- addr )
here @ ( n val )
swap ( val n )
here +! ;

: var
2 allot create jsr-docol,
' lit , , ' exit , ;

# signedness
: 0< 8000 and not not ;
: abs dup 0< if negate then ;
: s< - 0< ;
: s> swap s< ;

# return stack
:asm rdrop pla, pla, ;asm
:asm >r 0 lda,x pha, 1 lda,x pha,
inx, inx, ;asm
:asm r> dex, dex,
pla, 1 sta,x pla, 0 sta,x ;asm
:asm r@ dex, dex,
pla, 1 sta,x pla, 0 sta,x
pha, 1 lda,x pha, ;asm

: modules ;
# ." debug.."
# s" debug" load
# ." gfx.."
# s" gfx" load
# ." gfxdemo.."
# s" gfxdemo" load
# ." turtle.."
# s" turtle" load
." vi.."
s" vi" load
." ok" cr

: scratch ( strptr strlen -- )
tuck ( strlen strptr strlen )
dup here @ + ( strlen strptr strlen tmpbuf )
dup >r
dup [ key s ] literal swap c! 1+
dup [ key : ] literal swap c! 1+
( strlen strptr strlen tmpbuf+2 )
rot ( strlen strlen tmpbuf+2 strptr )
swap ( strlen strlen strptr tmpbuf+2 )
rot ( strlen strptr tmpbuf+2 strlen )
cmove ( strlen )
r> swap 2+ ( tmp strlen )
openw closew ;

." scratch old durexforth.."
s" durexforth" scratch

." save new durexforth.."
s" durexforth" save-forth

." done!" cr
1 blink
