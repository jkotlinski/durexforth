: 2+ 1+ 1+ ;
: cr d emit ;
: nip swap drop ;
: * um* drop ;
: jmp, 4c c, ;
: ['] ' [ ' literal compile, ]
; immediate
: [char] char [ ' literal compile, ] 
; immediate
: else jmp, here 0 ,
swap here swap ! ; immediate
: postpone
bl word find -1 = if
[ ' literal compile, ] ['] compile, 
then compile, ; immediate
: until
postpone 0branch , ; immediate
: again jmp, , ; immediate
: recurse latest @ >cfa compile, 
; immediate
: (
begin getc dup
0= if refill then
')' = if exit then
again ; immediate
: \ refill ; immediate
: tuck ( x y -- y x y ) swap over ;
: ?dup dup if dup then ;
: <> ( a b -- c ) = 0= ;
: u> ( n -- b ) swap u< ;
: 0<> ( x -- flag ) 0= 0= ;

: litstring ( -- addr len )
r> 1+ dup 2+ swap @ 2dup + 1- >r ;

: s" ( -- addr len )
state c@ if ( compile mode )
postpone litstring here 0 , 0
begin getc dup '"' <>
while c, 1+ repeat
drop swap ! exit
then ( immediate mode )
here here
begin getc dup '"' <>
while over c! 1+ repeat
drop here - ; immediate

: type ( caddr u -- )
0 d4 c! ( quote mode off )
begin ?dup while
swap dup c@ emit 1+ swap 1-
repeat drop ;

\ s" 'a' emit  " evaluate


: ." postpone s" postpone type 
; immediate
: .( begin getc dup ')' <>
while emit repeat drop ; immediate
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
repeat ; immediate

( gets pointer to first data field, 
i.e., skips the first jsr )
: >dfa >cfa 1+ 2+ ;

( dodoes words contain:
 1. jsr dodoes
 2. two-byte code pointer. default: rts
 3. variable length data )
: >body ( xt -- dataaddr ) 5 + ;
here 60 c, ( rts )
: create
header postpone dodoes literal , ;
: does> r> 1+ latest @ >dfa ! ;

.( asm..)
s" asm" included

code m+ ( d1 u -- d2 )
0 ldy,# msb lda,x +branch bpl, dey, 
:+ clc,
lsb lda,x lsb 2+ adc,x lsb 2+ sta,x
msb lda,x msb 2+ adc,x msb 2+ sta,x
tya, lsb 1+ adc,x lsb 1+ sta,x
tya, msb 1+ adc,x msb 1+ sta,x
inx, ;code

code rot ( a b c -- b c a )
msb 2+ ldy,x msb 1+ lda,x 
msb 2+ sta,x msb    lda,x
msb 1+ sta,x msb    sty,x
lsb 2+ ldy,x lsb 1+ lda,x 
lsb 2+ sta,x lsb    lda,x
lsb 1+ sta,x lsb    sty,x ;code
: -rot rot rot ;

code 100/
msb lda,x lsb sta,x 
0 lda,#   msb sta,x ;code

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
8b value w
8d value w2
9e value w3

( "0 to foo" sets value foo to 0 )
: (to) over 100/ over 2+ c! c! ;
: to ' 1+
state c@ if
postpone literal postpone (to) exit
then (to) ; immediate

: hex 10 base ! ;
: decimal a base ! ;

: 2drop ( a b -- ) 
postpone drop postpone drop ; immediate
code 2over ( a b c d -- a b c d a b )
dex,
msb 4 + lda,x msb sta,x
lsb 4 + lda,x lsb sta,x
dex,
msb 4 + lda,x msb sta,x
lsb 4 + lda,x lsb sta,x ;code
code 2swap ( a b c d -- c d a b )
lsb lda,x lsb 2+ ldy,x
lsb sty,x lsb 2+ sta,x
msb lda,x msb 2+ ldy,x
msb sty,x msb 2+ sta,x
lsb 1+ lda,x lsb 3 + ldy,x
lsb 1+ sty,x lsb 3 + sta,x
msb 1+ lda,x msb 3 + ldy,x
msb 1+ sty,x msb 3 + sta,x ;code

: save-forth ( strptr strlen -- )
801 -rot here -rot saveb ;

code 2/ 
msb lda,x 80 cmp,# msb ror,x lsb ror,x
;code
code or
msb lda,x msb 1+ ora,x msb 1+ sta,x
lsb lda,x lsb 1+ ora,x lsb 1+ sta,x
inx, ;code
code xor
msb lda,x msb 1+ eor,x msb 1+ sta,x
lsb lda,x lsb 1+ eor,x lsb 1+ sta,x
inx, ;code
code +! ( num addr -- ) 
lsb lda,x w sta,
msb lda,x w 1+ sta,
0 ldy,# clc,
w lda,(y) lsb 1+ adc,x 
w sta,(y) iny,
w lda,(y) msb 1+ adc,x 
w sta,(y)
inx, inx, ;code

:- dup inx, ;code
code lshift ( x1 u -- x2 )
lsb dec,x -branch bmi,
lsb 1+ asl,x msb 1+ rol,x
latest @ >cfa jmp, 
code rshift ( x1 u -- x2 )
lsb dec,x -branch bmi,
msb 1+ lsr,x lsb 1+ ror,x
latest @ >cfa jmp, 

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

code 0< msb lda,x 80 and,# +branch beq,
ff lda,# :+ lsb sta,x msb sta,x ;code

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
lsb 1+ lda,x lsb sbc,x 
msb 1+ lda,x msb sbc,x
+branch bvc, 80 eor,# :+ 
+branch bpl, dey, :+
inx, lsb sty,x msb sty,x ;code
: > swap < ;

: max ( a b - c )
2dup < if swap then drop ;
: min ( a b - c )
2dup > if swap then drop ;

.( format..) s" format" included

: .s depth begin ?dup while
dup pick . 1- repeat ;

: 2@ ( addr -- x1 x2 )
dup 2+ @ swap @ ;
: 2! ( x1 x2 addr -- )
swap over ! 2+ ! ;

: abort" 
postpone if
12 lda,# ffd2 jsr, \ reverse on
postpone ."
postpone cr
postpone abort
postpone then ; immediate

code sei sei, ;code
code cli cli, ;code

: assert 0= if
begin 1 d020 +! again then ;
: marker create latest @ ,
does> @ dup to here @ latest ! ;

marker modules

.( labels..) s" labels" included
.( doloop..) s" doloop" included
.( sys..) s" sys" included
.( debug..) s" debug" included
.( ls..) s" ls" included
.( gfx..) s" gfx" included
.( vi..) s" vi" included
.( require..) s" require" included

.( save new durexforth..)
s" @0:durexforth" save-forth .( ok!) cr
