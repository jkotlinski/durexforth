: 2+ 1+ 1+ ;
: cr d emit ;
: nip swap drop ;
: jmp, 4c c, ;
: ['] ' [ ' literal compile, ]
; immediate
: [char] char [ ' literal compile, ]
; immediate
: else jmp, here 0 ,
swap here swap ! ; immediate
: postpone bl word find -1 = if
[ ' literal compile, ] ['] compile,
then compile, ; immediate
: until postpone 0branch , ; immediate
: again jmp, , ; immediate
: recurse
latest >xt compile, ; immediate
: ( begin getc dup 0= if refill then
')' = if exit then again ; immediate
: \ refill ; immediate
: <> ( a b -- c ) = 0= ;
: u> ( n -- b ) swap u< ;
: 0<> ( x -- flag ) 0= 0= ;

: lits ( -- addr len )
r> 1+ dup 2+ swap @ 2dup + 1- >r ;

: s" ( -- addr len )
postpone lits here 0 , 0
begin getc dup '"' <>
while c, 1+ repeat
drop swap ! ; immediate

: ." postpone s" postpone type
; immediate
: .( begin getc dup ')' <>
while emit repeat drop ; immediate
.( compile base..)

: case 0 ; immediate
: (of) over = if drop r> 2+ >r exit
then branch ;
: of postpone (of) here 0 , ; immediate
: endof postpone else ; immediate
: endcase postpone drop
begin ?dup while postpone then
repeat ; immediate

( Returns data field address, which is
after jsr dodoes )
: >dfa >xt 1+ 2+ ;

( dodoes words contain:
 1. jsr dodoes
 2. two-byte code pointer. default: rts
 3. variable length data )
here 60 c, ( rts )
: create
header postpone dodoes [ swap ] literal , ;
: does> r> 1+ latest >dfa ! ;

.( asm..)
parse-name asm included

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
dup code lda,# 100/ ldy,#
['] pushya jmp, ;
: constant value ;
$33c constant pad
: space bl emit ;
: spaces ( n -- )
begin ?dup while space 1- repeat ;

8b value w
8d value w2
9e value w3

( "0 to foo" sets value foo to 0 )
: (to) over 100/ over 2+ c! c! ;
: to ' 1+ state c@ if
postpone literal postpone (to) exit
then (to) ; immediate

: hex 10 base ! ;
: decimal a base ! ;

: 2drop ( a b -- )
postpone drop postpone drop ; immediate

: :noname here 0 ] ;

: dsize ( -- n) \ not really needed?
top latest - ;

: top! ( addr -- )
latest swap dsize 2dup - to latest 
over to top swap over - swap 1+ move ;

\ first pass unfactored code ahead
\ basic dictionary stuff needs to be in base

: name>string ( nametoken -- caddr u )
count $1f and ;

: (xt>) ( xt1 nt -- nt xt 0 | xt1 1 )
2dup name>string + @
< invert if swap drop 0  
exit then drop 1 ;

: xt> ( codepointer -- nametoken )
['] (xt>) dowords ;

: compare ( caddr u caddr u -- 1 | 0 )
2 pick over <> if 2drop 2drop 1 exit then
drop swap
begin ( caddr caddr u )
1- 2dup + c@ ( caddr caddr u c )
over 4 pick + c@ <> if ( caddr caddr u )
2drop drop 1 exit then
dup 0= if 2drop drop 0 exit then again ;

: name>xta count $1f and + ;

: (string>name) ( caddr u nt ) ( nt 0 | caddr u )
dup count $1f and ( caddr u nt caddr u )
4 pick 4 pick ( caddr u nt caddr u caddr u )
compare if drop 1 exit then ( caddr u nt )
nip nip 0 0 ;

: show ( xt word -- )
parse-name ['] (string>name) dowords
if 2drop exit then name>xta ! ;
\ show usage: create a header, assign it an xt
\ effectively aliases an xt with no overhead
header hide:
' latest show hide:
: ;hide to latest ;


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
latest >xt jmp,
code rshift ( x1 u -- x2 )
lsb dec,x -branch bmi,
msb 1+ lsr,x lsb 1+ ror,x
latest >xt jmp,

: allot ( n -- ) here + to here ;

: variable
0 value
here latest >xt 1+ (to)
2 allot ;

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
: /mod >r s>d r> fm/mod ;
: / /mod nip ;
: mod /mod drop ;
: */mod >r m* r> fm/mod ;
: */ */mod nip ;
( ...from FIG UK )

.( format..) parse-name format included

: .s depth begin ?dup while
dup pick . 1- repeat ;

code rvs 12 lda,# ffd2 jsr, ;code

: abort"
postpone if
postpone rvs
postpone ."
postpone cr
postpone abort
postpone then ; immediate

: hide
parse-name find-name 
0= abort" not found"
xt> ( nt )
dup c@ 3 + ( offset )
swap latest - >r ( offset ) 
latest + ( dst )
latest swap dup to latest ( src dst ) r>
move ;

header save-prg
header save-pack

hide:

top value oldtop
start @ value oldstart

: restore-forth
oldtop top! 
oldstart execute ;

:noname ( strptr strlen -- )
start @ to oldstart
top to oldtop 
['] restore-forth start ! 
here 20 + dsize + top!
801 top 1+ d word count saveb ;
show save-pack

:noname ( strptr strlen -- )
here 0 , top to latest top!
save-pack ;
show save-prg

;hide

: save-forth ( strptr strlen -- )
801 top 1+ d word count saveb ;

\ hashes of INCLUDED file names
\ see required.fs
variable (includes) $1e allot
(includes) $20 0 fill

: marker top latest - here create , , 
(includes) begin dup @ while 2+ repeat , 
does> dup @ to here
2+ dup @ top swap - to latest 
2+ @ 0 swap ! ;

: include parse-name included ;

marker ---modules---

.( labels..) include labels
.( doloop..) include doloop
.( sys..) include sys
.( debug..) include debug
.( ls..) include ls
.( require..) include require
.( v..) include v

decimal

.( save new durexforth..)
save-pack @0:durexforth
.( ok!) cr
