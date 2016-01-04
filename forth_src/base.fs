: 2dup over over ;
: 2+ 1+ 1+ ;
: cr d emit ;
: nip swap drop ;
: * um* drop ;
: jmp, 4c c, ;
: ['] ' 
[ ' literal compile, ] ; immediate no-tce
: [char] char 
[ ' literal compile, ] ; immediate no-tce
: else jmp, here 0 ,
swap here swap ! ; immediate
: postpone
bl word find -1 = if
[ ' literal compile, ] ['] compile, then
compile, ; immediate
: until
postpone 0branch , ; immediate no-tce
: again jmp, , ; immediate
: recurse latest @ >cfa compile, ; immediate
: (
begin getc dup
0= if refill then
[char] ) = if exit then
again ; immediate no-tce
: \ refill ; immediate no-tce
: tuck ( x y -- y x y ) swap over ;
: ?dup dup if dup then ;
: <> ( a b -- c ) = 0= ;
: u> ( n -- b ) swap u< ;
: 0<> ( x -- flag ) 0= 0= ;

: litstring ( -- addr len )
r> 1+ dup 2+ swap @ 2dup + 1- >r ;

: count dup 1+ swap c@ ;
: s" ( -- addr len )
state c@ if ( compile mode )
postpone litstring here 0 , 0
begin getc dup [char] " <>
while c, 1+ repeat
drop swap !
else ( immediate mode )
here here
begin getc dup [char] " <>
while over c! 1+ repeat
drop here - then ; immediate no-tce

: type ( caddr u -- )
0 d4 c! ( quote mode off )
begin ?dup while
swap dup c@ emit 1+ swap 1-
repeat drop ;

: ." postpone s" postpone type ; immediate
: .( begin getc dup [char] ) <>
while emit repeat drop ;
.( compile base..)

: case 0 ; immediate
: of
postpone over
postpone =
postpone if 
postpone drop ; immediate
: endof postpone else ; immediate
: endcase 
postpone drop
begin ?dup while postpone then 
repeat ; immediate no-tce

: move ( src dst u -- )
>r 2dup u< r> swap
if cmove> else cmove then ;

( gets pointer to first data field, i.e., skips
the first jsr )
: >dfa >cfa 1+ 2+ ;

( dodoes words contain:
 1. jsr dodoes
 2. two-byte code pointer. default: point to exit
 3. variable length data )
: >body ( xt -- dataaddr ) 5 + ;
here 60 c, ( rts )
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

code 100/
sp1 lda,x sp0 sta,x 
0 lda,#   sp1 sta,x ;code

( creates value that is fast to read
  but can only be rewritten by "to".
   0 value foo
   foo . \ prints 0
   1 to foo
   foo . \ prints 1 )
: value ( n -- )
dup code
lda,# 100/ ldy,# 
['] pushya jmp, ;
: constant value ;

: space bl emit ;
: spaces ( n -- )
begin ?dup while space 1- repeat ;

1 value 1
8b value zptmp
8d value zptmp2
9e value zptmp3

( "0 to foo" sets value foo to 0 )
: (to) over 100/ over 2+ c! c! ;
: to ' 1+
state c@ if
postpone literal postpone (to)
else (to) then ; immediate

: hex 10 base ! ;
: decimal a base ! ;

: 2drop ( a b -- ) 
postpone drop postpone drop ; immediate no-tce
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

: save-forth ( strptr strlen -- )
801 -rot here -rot saveb ;

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
begin ?dup while swap 
0 2 um/mod nip 
swap 1- repeat ;

: allot ( n -- ) here + to here ;

: variable 
0 value
here latest @ >cfa 1+ (to)
2 allot ;

( fairly pointless ANS compat.. )
: environment? 2drop 0 ;
: cells 2* ;
: cell+ 2+ ;
: char+ 1+ ;
: chars ; : align ; : aligned ;
( ..fairly pointless ANS compat )

code 0< sp1 lda,x 80 and,# +branch beq,
ff lda,# :+ sp0 sta,x sp1 sta,x ;code

( from FIG UK... )
: s>d dup 0< ;
: ?negate 0< if negate then ;
: abs dup ?negate ;
: dnegate invert >r invert r> 1 m+ ;
: ?dnegate 0< if dnegate then ;
: dabs dup ?dnegate ;
: m* 2dup xor >r >r abs r> 
abs um* r> ?dnegate ;
: * m* drop ;
( ...from FIG UK )

: fm/mod ( from Gforth )
dup >r
dup 0< if negate >r dnegate r> then
over 0< if tuck + swap then
um/mod
r> 0< if swap negate swap then ;

( from FIG UK... )
: sm/rem 
2dup xor >r over >r abs >r dabs
r> um/mod swap r> ?negate
swap r> ?negate ;
: /mod >r s>d r> fm/mod ;
: / /mod nip ;
: mod /mod drop ;
: */mod >r m* r> fm/mod ;
: */ */mod nip ;
( ...from FIG UK )

code <
0 ldy,# sec,
sp0 1+ lda,x sp0 sbc,x 
sp1 1+ lda,x sp1 sbc,x
+branch bvc, 80 eor,# :+ 
+branch bpl, dey, :+
inx, sp0 sty,x sp1 sty,x ;code
: > swap < ;

: max ( a b - c )
2dup < if swap then drop ;
: min ( a b - c )
2dup > if swap then drop ;

: #pet dup a < if 7 - then 37 + ;
: u. 0 >r begin 0 base @ um/mod swap
#pet >r ?dup 0= until
begin r> ?dup while emit repeat space ;
: . dup 0< if [char] - emit negate
then u. ;
: .s depth begin ?dup while
dup pick . 1- repeat ;

: 2@ ( addr -- x1 x2 )
dup 2+ @ swap @ ;
: 2! ( x1 x2 addr -- )
swap over ! 2+ ! ;

: abort" 
postpone if
postpone ."
postpone abort
postpone then ; immediate no-tce

code sei sei, ;code
code cli cli, ;code

: assert 0= if
begin 1 d020 +! again then ;

( header modules )

.( labels..) s" labels" load
.( doloop..) s" doloop" load
.( format..) s" format" load
.( sys..) s" sys" load
.( debug..) s" debug" load

.( ls..) s" ls" load
.( gfx..) s" gfx" load
( s" sprite" load
  ." gfxdemo.."
  s" gfxdemo" load
  ." turtle.."
  s" turtle" load )
.( vi..) s" vi" load

.( save new durexforth..)
s" @:durexforth" save-forth .( ok!) cr
