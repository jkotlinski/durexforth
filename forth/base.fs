: 2+ 1+ 1+ ;
: jmp, 4c c, ;
: postpone bl word dup find ?dup 0= if
count notfound then
rot drop -1 = if [ ' literal compile,
' compile, literal ] then compile,
; immediate
: ['] ' postpone literal ; immediate
: [char] char postpone literal
; immediate
: else jmp, here 0 ,
swap here swap ! ; immediate
: until postpone 0branch , ; immediate
: again jmp, , ; immediate
: recurse
latestxt compile, ; immediate
: ( begin getc ')' = until ; immediate
: \ source >in ! drop ; immediate
: <> ( a b -- c ) = 0= ;
: u> ( n -- b ) swap u< ;
: 0<> ( x -- flag ) 0= 0= ;

: lits ( -- addr len )
r> 1+ count 2dup + 1- >r ;

: parse >r source >in @ /string
over swap begin dup while over c@ r@ <>
while 1 /string repeat then r> drop >r
over - dup r> if 1+ then >in +! ;

: s" ( -- addr len )
postpone lits here 0 c, 0
begin getc dup '"' <>
while c, 1+ repeat
drop swap c! ; immediate

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

( dodoes words contain:
 1. jsr dodoes
 2. two-byte code pointer. default: rts
 3. variable length data )
here 60 c, ( rts )
: create
header postpone dodoes literal , ;
: does> r> 1+ latest >xt 1+ 2+ ! ;

.( asm..)
parse-name asm included

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
( to free up space, pad could be
  e.g. HERE+34 instead )
$35b constant pad
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


: save-forth ( strptr strlen -- )
801 $a000 d word count saveb ;

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

( from FIG UK... )
: / /mod nip ;
: mod /mod drop ;
: */mod >r m* r> fm/mod ;
: */ */mod nip ;
( ...from FIG UK )

.( format..) parse-name format included

: .s depth begin ?dup while
dup pick . 1- repeat ;

: abort"
postpone if
postpone rvs
postpone ."
postpone cr
postpone abort
postpone then ; immediate

( linked list. each element contains
  backlink + hashed file name )
0 value (includes)

: marker ( -- )
(includes) latest here create , , ,
does> dup @ to here
   2+ dup @ to latest
   2+     @ to (includes) ;

: include parse-name included ;

: :noname here here to latestxt ] ;

marker ---modules---

.( wordlist..) include wordlist

\ hides private words
hide 1mi hide 2mi hide 23mi hide 3mi
hide holdp hide latestxt
hide dodoes

.( labels..) include labels
.( doloop..) include doloop
.( sys..) include sys
.( debug..) include debug
.( ls..) include ls
.( require..) include require
.( open..) include open
.( accept..) include accept
.( v..) include v

decimal
include turnkey
cr
.( cart: )
$4000 $6b - \ available ROM
here $801 - \ code + data
top 1+ latest - \ dictionary
$20 + + - \ save-pack padding
. .( bytes remain.) cr

.( save new durexforth..)
save-pack @0:durexforth
.( ok!) cr

0 $d7ff c! \ for vice -debugcart
