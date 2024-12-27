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

: \ source >in ! drop ; immediate
: <> = 0= ;
: u> swap u< ;
: 0<> 0= 0= ;

: parse >r source >in @ /string
over swap begin dup while over c@ r@ <>
while 1 /string repeat then r> drop >r
over - dup r> if 1+ then >in +! ;

: ( source-id 0= if ')' parse drop drop
else begin >in @ ')' parse nip >in @ rot
- = while refill drop repeat then ;
immediate

: lits ( -- addr len )
r> 1+ count 2dup + 1- >r ;

( "0 to foo" sets value foo to 0 )
: (to) >r split r@ 2+ c! r> c! ;
: to ' 1+ state c@ if
postpone literal postpone (to) exit
then (to) ; immediate

: allot ( n -- ) here + to here ;

: s" ( -- addr len )
'"' parse state @ if postpone lits
dup c, tuck here swap move allot
then ; immediate

: ." postpone s" postpone type
; immediate
: .( ')' parse type ; immediate
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

( creates value that is fast to read
  but can only be rewritten by "to".
   0 value foo
   foo . \ prints 0
   1 to foo
   foo . \ prints 1 )
: value ( n -- )
( TO relies on this lda/ldy order )
code split swap lda,# ldy,#
['] pushya jmp, ;
: constant value ;
( to free up space, pad could be
  e.g. HERE+34 instead )
$35b constant pad
: spaces ( n -- )
begin ?dup while space 1- repeat ;

8b value w
8d value w2
9e value w3

: hex 10 base ! ;
: decimal a base ! ;

: 2drop ( a b -- )
postpone drop postpone drop ; immediate


: save-forth ( strptr strlen -- )
801 $a000 d word count saveb ;

code 2/
msb lda,x 80 cmp,# msb ror,x lsb ror,x
rts, end-code
code or
msb lda,x msb 1+ ora,x msb 1+ sta,x
lsb lda,x lsb 1+ ora,x lsb 1+ sta,x
inx, rts, end-code
code xor
msb lda,x msb 1+ eor,x msb 1+ sta,x
lsb lda,x lsb 1+ eor,x lsb 1+ sta,x
inx, rts, end-code

:- dup inx, rts, end-code
code lshift ( x1 u -- x2 )
lsb dec,x -branch bmi,
lsb 1+ asl,x msb 1+ rol,x
latest >xt jmp,
code rshift ( x1 u -- x2 )
lsb dec,x -branch bmi,
msb 1+ lsr,x lsb 1+ ror,x
latest >xt jmp,

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

: .s depth begin ?dup while
dup pick . 1- repeat ;

: abort -1 throw ;
: abort" postpone if
postpone s" postpone (abort")
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
hide latestxt
hide dodoes hide (abort")

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
