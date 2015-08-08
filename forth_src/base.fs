: 2dup over over ;
: cr d emit ;
: nip swap drop ;
: * d* nip ;
: loc word find ;
: ' loc >cfa ;
: jmp, 4c c, ;
: compile, 20 c, , ;
: [compile] immed ' compile, ;
: ['] immed ' [compile] literal ;
: [char] immed key [compile] literal ;
: if immed ['] 0branch compile, here 0 , ;
: then immed here swap ! ;
: else immed jmp, here 0 ,
swap here swap ! ;
: begin immed here ;
: until immed ['] 0branch compile, , ;
: again immed jmp, , ;
: while immed ['] 0branch compile, here 0 , ;
: repeat immed jmp,
swap , here swap ! ;
: recurse immed latest @ >cfa compile, ;
: ( immed begin key [char] ) = until ;
: # immed begin key d = until ;
: tuck ( x y -- y x y ) swap over ;
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

: litstring ( -- addr len )
r> 1+ dup 2+ swap @ 2dup + 1- >r ;

: s" immed ( -- addr len )
state if ( compile mode )
['] litstring compile, here 0 , 0
begin key dup [char] " <>
while c, 1+ repeat
drop swap !
else ( immediate mode )
here here
begin key dup [char] " <>
while over c! 1+ repeat
drop here - then ;

: tell ( addr len -- )
begin ?dup while
swap dup c@ emit 1+ swap 1-
repeat drop ;

: ." immed [compile] s" ['] tell compile, ;
: .( begin key dup [char] ) <>
while emit repeat drop ;
.( compile base..)

: case immed 0 ;
: of immed 
['] over compile, 
['] = compile, 
[compile] if 
['] drop compile, ;
: endof immed [compile] else ;
: endcase immed 
['] drop compile, 
begin ?dup while [compile] then 
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
here exit
: create
header ['] dodoes compile, literal , ;
: does> r> 1+ latest @ >dfa ! ;

.( asm..)
s" asm" load

:asm rot ( a b c -- b c a )
sp1 2+ ldy,x sp1 1+ lda,x 
sp1 2+ sta,x sp1    lda,x
sp1 1+ sta,x sp1    sty,x
sp0 2+ ldy,x sp0 1+ lda,x 
sp0 2+ sta,x sp0    lda,x
sp0 1+ sta,x sp0    sty,x ;asm
: -rot rot rot ;

: /mod 0 -rot um/mod ;
: / /mod nip ;
: mod /mod drop ;
: */mod -rot d* rot um/mod ;
: */ */mod nip ;

:asm 100/
sp1 lda,x sp0 sta,x 
0 lda,#   sp1 sta,x ;asm

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

20 value bl
: space bl emit ;

0 value 0 1 value 1
8b value zptmp
8d value zptmp2
9e value zptmp3

# "0 to foo" sets value foo to 0
: (to) over 100/ over 2+ c! c! ;
: to immed ' 1+
state if
[compile] literal ['] (to) compile,
else (to) then ;

: hex 10 to base ;
: decimal a to base ;

:asm 2drop ( a b -- )
inx, inx, ;asm

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

:asm 2/ sp1 lsr,x sp0 ror,x ;asm
:asm or
sp1 lda,x sp1 1+ ora,x sp1 1+ sta,x
sp0 lda,x sp0 1+ ora,x sp0 1+ sta,x
inx, ;asm
:asm xor
sp1 lda,x sp1 1+ eor,x sp1 1+ sta,x
sp0 lda,x sp0 1+ eor,x sp0 1+ sta,x
inx, ;asm
: invert ffff xor ;
: negate invert 1+ ;
:asm +! ( num addr -- ) 
sp0 lda,x zptmp sta,
sp1 lda,x zptmp 1+ sta,
0 ldy,# clc,
zptmp lda,(y) sp0 1+ adc,x 
zptmp sta,(y) iny,
zptmp lda,(y) sp1 1+ adc,x 
zptmp sta,(y)
inx, inx, ;asm

: lshift ( x1 u -- x2 )
begin ?dup while swap 2* swap 1- repeat ;
: rshift ( x1 u -- x2 )
begin ?dup while swap 2/ swap 1- repeat ;

: allot ( n -- prev-here )
here tuck + to here ;

: variable 2 allot value ;

# signedness
: 0< 7fff > ;
: abs dup 0< if negate then ;
: s< - 0< ;
: s> swap s< ;

: . 0 >r begin base /mod swap
dup a < if 7 - then 37 + >r
?dup 0= until
begin r> ?dup while emit repeat space ;
: .s depth begin ?dup while
dup pick . 1- repeat ;

:asm sei sei, ;asm
:asm cli cli, ;asm

: assert 0= if
begin 1 d020 +! again then ;

header modules
.( labels..)
s" labels" load
.( doloop..)
s" doloop" load
.( jsr..)
s" jsr" load
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

.( done!) cr
