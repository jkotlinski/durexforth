

: bmpbase a000 ;
: colbase 8c00 ;

:asm hires 
bb lda,# d011 sta, # enable
15 lda,# dd00 sta, # vic bank 2
38 lda,# d018 sta,
56 lda,# 1 sta, # no basic
;asm

:asm lores
9b lda,# d011 sta,
17 lda,# dd00 sta,
17 lda,# d018 sta,
;asm

: clrcol ( fgbgcol -- )
colbase 3e8 fill
0 bmpbase 1f40 fill ;

: blkcol ( x y c -- )
rot 2/ 2/ 2/ rot 2/ 2/ 2/ 28 *
+ colbase + c! ;

create mask
80 c, 40 c, 20 c, 10 c,
8 c, 4 c, 2 c, 1 c,

var penx var peny
0 penx ! 0 peny !

# blit operations for plot, line
create blitop
0 , # doplot
0 , # lineplot

: 100/ 2/ 2/ 2/ 2/ 2/ 2/ 2/ 2/ ;

:asm blitloc ( x y -- mask addr )
0 lda,x zptmp sta,
7 and,# zptmp3 sta,
1 lda,x zptmp 1+ sta,

zptmp lda, f8 and,# zptmp sta,

# * 8
zptmp asl, zptmp 1+ rol,
zptmp asl, zptmp 1+ rol,
zptmp asl, zptmp 1+ rol,

zptmp lda, zptmp2 sta,
zptmp 1+ lda, zptmp2 1+ sta,

# * 20
zptmp asl, zptmp 1+ rol,
zptmp asl, zptmp 1+ rol,

clc,
zptmp lda, zptmp2 adc, zptmp sta,
zptmp 1+ lda, zptmp2 1+ adc,
zptmp 1+ sta,
clc,
zptmp lda, zptmp3 adc, zptmp sta,
2 bcc, zptmp 1+ inc,

zptmp lda, 0 sta,x
zptmp 1+ lda, 1 sta,x

# ...

loc mask >cfa 100/
lda,# zptmp 1+ sta,

clc,
2 lda,x 7 and,# loc mask >cfa adc,#
zptmp sta,
2 bcc, zptmp 1+ inc,

# zptmp = mask
0 ldy,#
zptmp lda,(y) zptmp3 sta,

clc,
2 lda,x f8 and,# 0 adc,x 0 sta,x
3 lda,x 1 adc,x a0 eor,# 1 sta,x
zptmp3 lda, 2 sta,x
0 lda,# 3 sta,x
;asm # blitloc

: doplot ( x y -- )
blitloc tuck c@
[ here @ loc blitop >cfa ! ] or
swap c! ;

: chkplot ( x y -- )
over 13f > over c7 > or
if 2drop else doplot then ;

: plot ( x y -- )
2dup peny ! penx ! chkplot ;

: peek ( x y -- b )
blitloc c@ and ;

: dx 0 ; : dy 0 ;
: sx 0 ; : sy 0 ;
var err

var mask var addr

:asm lineplot ( -- )

# penx @ 140 <
penx lda,# zptmp sta,
penx 100/ lda,# zptmp 1+ sta,
1 ldy,# zptmp lda,(y)
12 beq,
1 cmp,# 3 beq, ;asm
dey, zptmp lda,(y)
sec, 40 sbc,#
3 bcc, ;asm

# peny @ c8 <
peny lda,# zptmp sta,
peny 100/ lda,# zptmp 1+ sta,
1 ldy,# zptmp lda,(y)
3 beq, ;asm
dey, zptmp lda,(y)
sec, c8 sbc,#
3 bcc, ;asm

# addr
addr 100/
lda,# zptmp 1+ sta,
addr lda,# zptmp sta,

# @
0 ldy,#
zptmp lda,(y) zptmp2 sta, iny,
zptmp lda,(y) zptmp2 1+ sta, dey,

# c@ mask c@ or
zptmp2 lda,(y)
here @ loc blitop >cfa 2+ !
mask ora, 0

# addr @ c!
zptmp2 sta,(y)
;asm

: line ( x y -- )
2dup peny @ - abs to dy
penx @ - abs to dx
2dup
peny @ swap s< if 1 else ffff then to sy
penx @ swap s< if 1 else ffff then to sx
dx dy - err !
dy negate to dy

penx @ peny @ blitloc addr ! mask !

begin
 err @ 2* dup
 dy s> if
  dy err +!
  sx penx +! 
  mask c@ sx 1 = if
  2/ dup 0= if drop 80 8 addr +! then
  else 2* dup 100 = if drop 1 fff8 addr +! then
  then mask c!
 then
 dx s< if
  dx err +!
  sy peny +!
  addr @ sy 1 = if ( down )
   1+ dup 7 and 0= if 138 + then
  else ( up )
   dup 7 and 0= if 138 - then 1-
  then
  addr !
 then
 lineplot 
 dup peny @ = if over penx @ = if
  2drop exit
 then then
again ;

# --- circle

: cx 0 ; : cy 0 ;

: plot4 ( x y -- x y )
over cx + over cy + chkplot
over if # x?
over cx swap - over cy + chkplot
then
dup if # y?
over cx + over cy swap - chkplot
then
over not not over not not and if
over cx swap - over cy swap - chkplot
then ;

: plot8 ( x y -- x y )
plot4
2dup <> if
swap plot4 swap
then ;

: circle ( cx cy r -- )
dup negate err !
swap to cy
swap to cx
0 # x y
begin 2dup s< not while
plot8
dup err +!
1+
dup err +!
err @ 0< not if
over negate err +!
swap 1- swap
over negate err +!
then
repeat 2drop ;

hide cx hide cy

: erase dup if
4d ['] xor else
d ['] or then ['] blitop @ ! 
['] blitop 2+ @ c! ;

# paul heckbert seed fill
# from graphics gems
var stk
: spush ( y xl xr dy -- )
# y out of bounds?
3 pick over + dup 0< swap c7 > or if
2drop 2drop exit then
stk @ tuck c! 1+ # dy
tuck ! 2+ # xr
tuck ! 2+ # xl
tuck c! 1+ # y
stk ! ;

: x1 0 ; : x2 0 ;

: spop ( -- y )
stk @ 1- dup c@ swap # y
1- 1- dup @ to x1
1- 1- dup @ to x2
1- dup c@
ff = if ffff else 1 then to dy
stk ! ;

var l

# this one must be fast
: fillr ( x y -- newx y )
over 140 >= if exit then

over penx !
2dup blitloc # x y mask addr

# bitwise scan
begin over while
# x y mask addr
2dup c@ and if # end?
2drop nip penx @ swap exit
else # advance
2dup c@ or over c!
1 penx +! swap 2/ swap 
then repeat

# reached end?
penx @ 140 >= if
2drop nip penx @ swap exit
then

# bytewise
nip 8 + # x y addr
begin
penx @ 140 <
over c@ 0= and while
ff over c!
8 dup penx +! + repeat

# bitwise
80 swap
penx @ 140 < if
begin over while
# x y mask addr
2dup c@ and if # end?
2drop nip penx @ swap exit
else # advance
2dup c@ or over c!
1 penx +! swap 2/ swap 
then repeat then

2drop nip penx @ swap ;

: paint ( x y -- )
2dup c8 >= swap 140 >= or
if 2drop exit then
2dup peek if 2drop exit then

here @ stk !
# push y x x 1
2dup swap dup 1 spush
# push y+1 x x -1
1+ swap dup ffff spush

begin here @ stk @ < while
spop dy + # y

# left line
x1 over # y x y
begin
2dup peek not # y x y !peek
2 pick 0< not and while
2dup doplot
swap 1- swap repeat
over x1 # y x y x x1
s< not if
branch [ here @ >r 0 , ] # goto skip
then
# y x y ...
over 1+ dup l ! 
# y x y l
x1 < if # l < x1?
# y x y
dup l @ # y x y y l
x1 1- # y x y y l x1-1
dy negate spush
then
# y x y
nip x1 1+ swap # x=x1+1

begin
fillr
# y x y
dup l @
# y x y y l
3 pick 1-
# y x y y l x-1
dy spush
# y x y

# leak on right?
over x2 1+ > if
dup # y x y y
x2 1+ # y x y y x2+1
3 pick 1- # y x y y x2+1 x-1
dy negate spush
then

# skip: y x y
[ r> here @ over - swap ! ]

swap 1+ swap
begin
2dup peek not not
# y x2 x y peek
2 pick x2 <= and while
swap 1+ swap repeat

over l ! # l=x

# y x y
over x2 > until

2drop drop repeat ; 

hide dx hide dy
hide sx hide sy
hide err
hide x1 hide x2 hide l
hide plot4 hide plot8
hide blitop
hide bmpbase hide colbase
hide mask hide fillr

# test paint
( hires 5 clrcol

0 90 plot 13e 90 line
2 0 plot 2 c7 line
5 0 plot 5 c7 line
3 30 paint

60 60 20 circle
60 60 10 circle
7f 60 paint

10 10 plot
20 10 line
20 35 line
10 35 line
10 12 line
8 8 plot
25 8 line
25 40 line
8 40 line
8 18 line
18 24 paint lores )
