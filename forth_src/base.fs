  

: 2dup over over ;
: bl 20 ;
: space bl emit ;
: cr d emit ;
: nip swap drop ;
: * d* nip ;
: loc word find ;
: [compile] immed loc >cfa , ;
: ['] immed ' ' , ;
: if immed ' 0branch , here 0 , ;
: then immed here swap ! ;
: else immed ' branch , here 0 ,
swap here swap ! ;
: begin immed here ;
: until immed ' 0branch , , ;
: again immed ' branch , , ;
: while immed ' 0branch , here 0 , ;
: repeat immed ' branch , swap , here swap ! ;
: recurse immed latest @ >cfa , ;
: ( immed begin key [ key ) ] literal = until ;
: # immed begin key d = until ; # comment
: tuck ( x y -- y x y ) swap over ;
: pick ( x_u ... x_1 x_0 u -- x_u ... x_1 x_0 x_u )
1+ 2* sp@ + @ ;
: ?dup dup if dup then ;
: <> ( a b -- c ) = 0= ;
: > ( n -- b ) swap < ;
: 0<> ( x -- flag ) 0= 0= ;

: <= > 0= ;
: >= < 0= ;
: 2+ 1+ 1+ ;
: max ( a b - c )
2dup < if swap then drop ;
: min ( a b - c )
2dup > if swap then drop ;

: '"' [ key " ] literal ;
: s" immed ( -- addr len )
	state if
		' litstring ,
		here ( save addr of length byte on stack )
		0 c, ( dummy length - we don't know what it is yet )
		begin
			key
			dup '"' <>
		while
			c,
		repeat
		drop
		dup
		here swap -
		1- ( subtract to compensate for length byte )
		swap c!
	else ( immediate mode )
		here
		begin
			key
			dup '"' <>
		while
			over c!
			1+
		repeat
		drop
		here -
		here
		swap
	then
;

: tell ( addr len -- )
begin over c@ emit ( print char )
swap 1+ swap ( inc strptr )
1- ( dec strlen )
?dup 0= until drop ;

: ." immed [compile] s" ' tell , ;
: .( begin key dup [ key ) literal ] <>
while emit repeat drop ;
.( compile base..)

: case immed 0 ;
: of immed ' over , ' = , [compile] if ' drop , ;
: endof immed [compile] else ;
: endcase immed ' drop , begin ?dup while [compile] then repeat ;

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

: hide
loc ?dup if hidden else ." err" then ;

here [compile] exit
: create
# default behavior = exit
header bl c, ['] dodoes , literal , ;
: does> r> latest @ >dfa ! ;

.( asm..)
s" asm" load

:asm rot ( a b c -- b c a )
5 ldy,x 3 lda,x 5 sta,x 1 lda,x
3 sta,x 1 sty,x
4 ldy,x 2 lda,x 4 sta,x 0 lda,x
2 sta,x 0 sty,x ;asm
: -rot rot rot ;

: /mod 0 -rot um/mod ;
: / /mod nip ;
: mod /mod drop ;
: */mod -rot d* rot um/mod ;
: */ */mod nip ;

:asm 100/
1 lda,x 0 sta,x 0 lda,# 1 sta,x ;asm

# creates value that is fast to read
# but can only be rewritten by "to".
#  0 value foo
#  foo . # prints 0
#  1 to foo
#  foo . # prints 1
: value ( n -- )
dup :asm
lda,# 100/ ldy,# 
['] pushya jmp, ;

0 value 0 1 value 1
88 value zptmp
8a value zptmp2
8c value zptmp3
8e value ip

# "0 to foo" sets value foo to 0
: (to) over 100/ over 2+ c! c! ;
: to immed loc >cfa 1+
state if [compile] ['] , ' (to) ,
else (to) then ;

: hex 10 to base ;
: decimal a to base ;

:asm 2drop ( a b -- )
inx, inx, inx, inx, ;asm

: forget loc ?dup if dup @ latest ! to here then ;

: hide-to  ( -- )
loc latest
begin @ dup hidden 2dup = until
2drop ;

: save-forth ( strptr strlen -- )
compile-ram @ -rot 0 compile-ram !
801 -rot here -rot saveb
compile-ram ! ;

:asm 2/ 1 lsr,x 0 ror,x ;asm
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

: allot ( n -- prev-here )
here swap over + to here ;

: var 2 allot value ;

var ar var xr var yr

:asm jsr
0 lda,x here 1+ 1234 sta, # lsb
1 lda,x here 1+ 1234 sta, # msb
txa, pha,
ar lda, xr ldx, yr ldy,
here 2+ swap ! here 1+ swap !
1234 jsr,
ar sta, xr stx, yr sty,
pla, tax, inx, inx, ;asm

# signedness
: 0< 7fff > ;
: abs dup 0< if negate then ;
: s< - 0< ;
: s> swap s< ;

# return stack
:asm >r 0 lda,x pha, 1 lda,x pha,
inx, inx, ;asm
:asm r@ dex, dex,
pla, 1 sta,x pla, 0 sta,x
pha, 1 lda,x pha, ;asm

:asm sei sei, ;asm
:asm cli cli, ;asm

: modules ;
.( doloop..)
s" doloop" load
.( debug..)
s" debug" load
.( ls..)
s" ls" load
.( gfx..)
s" gfx" load
# s" sprite" load
# ." gfxdemo.."
# s" gfxdemo" load
# ." turtle.."
# s" turtle" load
.( vi..)
s" vi" load

: scratch ( strptr strlen -- )
tuck ( strlen strptr strlen )
dup here + ( strlen strptr strlen tmpbuf )
dup >r
dup [ key s ] literal swap c! 1+
dup [ key : ] literal swap c! 1+
( strlen strptr strlen tmpbuf+2 )
rot ( strlen strlen tmpbuf+2 strptr )
swap ( strlen strlen strptr tmpbuf+2 )
rot ( strlen strptr tmpbuf+2 strlen )
cmove ( strlen )
r> swap 2+ ( tmp strlen )
f openw f closew ;

hide pushya

s" purge-hidden" load

.( scratch old durexforth..)
s" durexforth" scratch

.( save new durexforth..)
s" durexforth" save-forth

.( done!) cr
