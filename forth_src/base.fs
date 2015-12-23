: 2dup over over ;
: 2+ 1+ 1+ ;
: cr d emit ;
: nip swap drop ;
: * um* drop ;
: loc word find ;
: ' loc >cfa ;
: jmp, 4c c, ;
: ['] immediate ' 
[ ' literal compile, ] ;
: [char] immediate key 
[ ' literal compile, ] ;
: if immediate no-tce
['] 0branch compile, 
here 0 , ;
: then immediate no-tce
here swap ! ;
: else immediate jmp, here 0 ,
swap here swap ! ;
: postpone immediate
loc dup >cfa swap 2+ c@ 80 and 0= if
[ ' literal compile, ] ['] compile, then
compile, ;
: begin immediate here ;
: until immediate no-tce 
postpone 0branch , ;
: again immediate jmp, , ;
: while immediate postpone 0branch here 0 , ;
: repeat immediate no-tce
jmp, swap , here swap ! ;
: recurse immediate latest @ >cfa compile, ;
: ( immediate no-tce 
begin key [char] ) = until ;
: \ immediate no-tce 
begin key d = until ;
: tuck ( x y -- y x y ) swap over ;
: ?dup dup if dup then ;
: <> ( a b -- c ) = 0= ;
: u> ( n -- b ) swap u< ;
: 0<> ( x -- flag ) 0= 0= ;

: litstring ( -- addr len )
r> 1+ dup 2+ swap @ 2dup + 1- >r ;

: s" immediate no-tce ( -- addr len )
state if ( compile mode )
postpone litstring here 0 , 0
begin key dup [char] " <>
while c, 1+ repeat
drop swap !
else ( immediate mode )
here here
begin key dup [char] " <>
while over c! 1+ repeat
drop here - then ;

: type ( caddr u -- )
begin ?dup while
swap dup c@ emit 1+ swap 1-
repeat drop ;

: source ( -- caddr u ) 200 dup
begin dup c@ while 1+ repeat
over - ;

: ." immediate postpone s" postpone type ;
: .( begin key dup [char] ) <>
while emit repeat drop ;
.( compile base..)

: case immediate 0 ;
: of immediate 
postpone over
postpone =
postpone if 
postpone drop ;
: endof immediate postpone else ;
: endcase immediate no-tce
postpone drop
begin ?dup while postpone then 
repeat ;

( gets pointer to first data field, i.e., skips
the first jsr )
: >dfa >cfa 1+ 2+ ;

: hide
loc ?dup if hidden else ." err" then ;

( dodoes words contain:
 1. jsr dodoes
 2. two-byte code pointer. default: point to exit
 3. variable length data )
here 60 c, \ rts
: create
header postpone dodoes literal , ;
: does> r> 1+ latest @ >dfa ! ;

.( asm..)
s" asm" load

code m+ ( d1 u -- d2 )
0 ldy,# sp1 lda,x +branch bpl, dey, 
:+ clc,
sp0 lda,x sp0 2+ adc,x sp0 2+ sta,x
sp1 lda,x sp1 2+ adc,x sp1 2+ sta,x
tya, sp0 1+ adc,x sp0 1+ sta,x
tya, sp1 1+ adc,x sp1 1+ sta,x
inx, ;code

code rot ( a b c -- b c a )
sp1 2+ ldy,x sp1 1+ lda,x 
sp1 2+ sta,x sp1    lda,x
sp1 1+ sta,x sp1    sty,x
sp0 2+ ldy,x sp0 1+ lda,x 
sp0 2+ sta,x sp0    lda,x
sp0 1+ sta,x sp0    sty,x ;code
: -rot rot rot ;

: /mod 0 swap um/mod ;
: / /mod nip ;
: mod /mod drop ;
: */mod -rot um* rot um/mod ;
: */ */mod nip ;

code 100/
sp1 lda,x sp0 sta,x 
0 lda,#   sp1 sta,x ;code

\ creates value that is fast to read
\ but can only be rewritten by "to".
\  0 value foo
\  foo . \ prints 0
\  1 to foo
\  foo . \ prints 1
: value ( n -- )
dup code
lda,# 100/ ldy,# 
['] pushya jmp, ;
: constant value ;

20 value bl
: space bl emit ;
: spaces ( n -- )
begin ?dup while space 1- repeat ;

0 value 0 1 value 1
8b value zptmp
8d value zptmp2
9e value zptmp3

\ "0 to foo" sets value foo to 0
: (to) over 100/ over 2+ c! c! ;
: to immediate ' 1+
state if
postpone literal postpone (to)
else (to) then ;

: hex 10 to base ;
: decimal a to base ;

: 2drop ( a b -- ) immediate no-tce
postpone drop postpone drop ;
code 2over ( a b c d -- a b c d a b )
dex,
sp1 4 + lda,x sp1 sta,x
sp0 4 + lda,x sp0 sta,x
dex,
sp1 4 + lda,x sp1 sta,x
sp0 4 + lda,x sp0 sta,x ;code
code 2swap ( a b c d -- c d a b )
sp0 lda,x sp0 2+ ldy,x
sp0 sty,x sp0 2+ sta,x
sp1 lda,x sp1 2+ ldy,x
sp1 sty,x sp1 2+ sta,x
sp0 1+ lda,x sp0 3 + ldy,x
sp0 1+ sty,x sp0 3 + sta,x
sp1 1+ lda,x sp1 3 + ldy,x
sp1 1+ sty,x sp1 3 + sta,x ;code

: forget loc ?dup if
dup @ latest ! to here then ;

: hide-to  ( -- )
loc latest
begin @ dup hidden 2dup = until
2drop ;

: save-forth ( strptr strlen -- )
compile-ram @ -rot 0 compile-ram !
801 -rot here -rot saveb
compile-ram ! ;

code 2/ 
sp1 lda,x 80 cmp,# sp1 ror,x sp0 ror,x
;code
code or
sp1 lda,x sp1 1+ ora,x sp1 1+ sta,x
sp0 lda,x sp0 1+ ora,x sp0 1+ sta,x
inx, ;code
code xor
sp1 lda,x sp1 1+ eor,x sp1 1+ sta,x
sp0 lda,x sp0 1+ eor,x sp0 1+ sta,x
inx, ;code
code +! ( num addr -- ) 
sp0 lda,x zptmp sta,
sp1 lda,x zptmp 1+ sta,
0 ldy,# clc,
zptmp lda,(y) sp0 1+ adc,x 
zptmp sta,(y) iny,
zptmp lda,(y) sp1 1+ adc,x 
zptmp sta,(y)
inx, inx, ;code

: lshift ( x1 u -- x2 )
begin ?dup while swap 2* swap 1- repeat ;
: rshift ( x1 u -- x2 )
begin ?dup while swap 2/ swap 1- repeat ;

: allot ( n -- prev-here )
here tuck + to here ;

: cells 2* ;
: cell+ 2+ ;
: variable 2 allot value ;

code 0< sp1 lda,x 80 and,# +branch beq,
ff lda,# :+ sp0 sta,x sp1 sta,x ;code
: abs dup 0< if negate then ;
: < - 0< ;
: > swap < ;

: max ( a b - c )
2dup < if swap then drop ;
: min ( a b - c )
2dup > if swap then drop ;
: within ( test low high -- flag )
over - >r - r> u< ;

: u. 0 >r begin base /mod swap
dup a < if 7 - then 37 + >r
?dup 0= until
begin r> ?dup while emit repeat space ;
: . dup 0< if [char] - emit negate
then u. ;
: .s depth begin ?dup while
dup pick . 1- repeat ;

code sei sei, ;code
code cli cli, ;code

: assert 0= if
begin 1 d020 +! again then ;

header modules
.( labels..)
s" labels" load
.( doloop..)
s" doloop" load
.( sys..)
s" sys" load
.( debug..)
s" debug" load
.( ls..)
s" ls" load
.( gfx..)
s" gfx" load
\ s" sprite" load
\ ." gfxdemo.."
\ s" gfxdemo" load
\ ." turtle.."
\ s" turtle" load
.( vi..)
s" vi" load

: scratch ( strptr strlen -- )
2dup here 2+ swap cmove>
[char] s here c!
[char] : here 1+ c!
nip 2+ here swap f openw f closew ;

hide pushya

s" purge-hidden" load

.( scratch old durexforth..)
s" durexforth" scratch

.( save new durexforth..)
s" durexforth" save-forth

depth 0= assert
.( done!) cr
