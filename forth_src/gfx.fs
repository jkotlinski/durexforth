

a000 value bmpbase
8c00 value colbase

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

: blkcol ( col row c -- )
rot rot 28 * + colbase + c! ;

header mask
80 c, 40 c, 20 c, 10 c,
8 c, 4 c, 2 c, 1 c,

var penx var peny
0 penx ! 0 peny !

# blit operations for plot, line
header blitop
0 , # doplot
0 , # lineplot

create .blitloc
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
3 lda,x 1 adc,x clc, a0 adc,# 1 sta,x
zptmp3 lda, 2 sta,x
0 lda,# 3 sta,x
rts,

hide mask

:asm blitloc ( x y -- mask addr )
.blitloc jsr, ;asm

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

var dy
var sy var sx
var err var 2err

var mask var addr

create lineplot ( -- )

# penx @ 140 <
penx lda,# zptmp sta,
penx 100/ lda,# zptmp 1+ sta,
1 ldy,# zptmp lda,(y)
+branch beq,
1 cmp,# 1 beq, rts,
dey, zptmp lda,(y)
sec, 40 sbc,#
1 bcc, rts,
:+

# peny @ c8 <
peny lda,# zptmp sta,
peny 100/ lda,# zptmp 1+ sta,
1 ldy,# zptmp lda,(y)
1 beq, rts,
dey, zptmp lda,(y)
sec, c8 sbc,#
1 bcc, rts,

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
mask ora,

# addr @ c!
zptmp2 sta,(y) rts,

var dx2 var dy2

create stepx
# 2err @ dx2 @ s< if
sec, 2err lda, dx2 sbc,
2err 1+ lda, dx2 1+ sbc,
3 bmi, lineplot jmp,

# dx2 @ err +!
clc, dx2 lda, err adc, err sta,
dx2 1+ lda, err 1+ adc, err 1+ sta,
# sy @ peny +!
clc, sy lda, peny adc, peny sta,
sy 1+ lda, peny 1+ adc, peny 1+ sta,

# sy @ 1 = if down else up then
sy lda, 1 cmp,# +branch beq,
# up
addr lda, 7 and,# +branch bne,
sec, addr lda, 38 sbc,# addr sta,
addr 1+ lda, 1 sbc,# addr 1+ sta,
:+ 
addr lda, 3 bne, addr 1+ dec, addr dec,
lineplot jmp,
:+ # down
addr inc, 3 bne, addr 1+ inc,
addr lda, 7 and,# 3 beq, lineplot jmp,
clc, addr lda, 38 adc,# addr sta,
addr 1+ lda, 1 adc,# addr 1+ sta,
lineplot jmp,

hide lineplot

create step ( 2err -- 2err )
# err @ 2* 2err !
err lda, 2err sta,
err 1+ lda, 2err 1+ sta,
2err asl, 2err 1+ rol,

# step up/down

# 2err @ dy2 @ s> if 
sec, dy2 lda, 2err sbc,
dy2 1+ lda, 2err 1+ sbc,
3 bmi, stepx jmp,

# dy2 @ err +!
clc, dy2 lda, err adc, err sta,
dy2 1+ lda, err 1+ adc, err 1+ sta,
# sx @ penx +! 
clc, sx lda, penx adc, penx sta,
sx 1+ lda, penx 1+ adc, penx 1+ sta,

# sx @ 1 = if maskror else maskrol then
sx lda, 1 cmp,# +branch bne,
# right
# maskror.mask>>1,addr+8?
mask lsr, 3 bcs, stepx jmp,
80 lda,# mask sta,
clc, addr lda, 8 adc,# addr sta,
3 bcc, addr 1+ inc, stepx jmp,
:+ # left
# mask<<1,addr-8?
mask asl, 3 bcs, stepx jmp,
1 lda,# mask sta,
sec, addr lda, 8 sbc,# addr sta,
3 bcs, addr 1+ dec, stepx jmp,

:asm doline
:- :- :- :-
step jsr,
peny lda, 0 cmp,x -branch bne,
penx lda, 2 cmp,x -branch bne,
peny 1+ lda, 1 cmp,x -branch bne,
penx 1+ lda, 3 cmp,x -branch bne,
inx, inx, inx, inx, ;asm

: line ( x y -- )
2dup peny @ - abs dy2 !
penx @ - abs dx2 !
2dup
peny @ swap s< if 1 else ffff then sy !
penx @ swap s< if 1 else ffff then sx !
dx2 @ dy2 @ - err !
dy2 @ negate dy2 !

penx @ peny @ blitloc addr ! mask !

doline ;

hide doline

# --- circle

0 value cx 0 value cy

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

: erase if
4d ['] xor else
d ['] or then ['] blitop @ ! 
['] blitop 2+ @ c! ;

# --------------------------

# paul heckbert seed fill
# from graphics gems
var stk
create dopush
stk lda, zptmp sta,
stk 1+ lda, zptmp 1+ sta,

# dy
0 ldy,# 0 lda,x zptmp sta,(y)
# xr
iny, 2 lda,x zptmp sta,(y)
iny, 3 lda,x zptmp sta,(y)
# xl
iny, 4 lda,x zptmp sta,(y)
iny, 5 lda,x zptmp sta,(y)
# y
iny, 6 lda,x zptmp sta,(y)

clc, stk lda, 6 adc,# stk sta,
3 bcc, stk 1+ inc, rts,

:asm spush ( y xl xr dy -- )
# y out of bounds?
clc, 0 lda,x 6 adc,x tay,
1 lda,x 7 adc,x +branch bne,
tya, sec, c8 cmp,# 3 bcs, dopush jsr,
:+
inx, inx, inx, inx,
inx, inx, inx, inx, ;asm

var x1 var x2

:asm spop ( -- y )
stk lda,
sec, 6 sbc,# zptmp sta, stk sta,
3 bcs, stk 1+ dec,
stk 1+ lda, zptmp 1+ sta,

# ff = if ffff else 1 then dy !
0 ldy,# zptmp lda,(y)
dy sta, dy 1+ sta,
1 cmp,# 3 bne, dy 1+ sty,

dex, dex,
1 sty,x # msb y=0
iny, zptmp lda,(y) x2 sta,
iny, zptmp lda,(y) x2 1+ sta,
iny, zptmp lda,(y) x1 sta,
iny, zptmp lda,(y) x1 1+ sta,
iny, zptmp lda,(y) 0 sta,x
;asm

var l

# ---

create .bitblt ( mask addr --
                  mask addr )
0 lda,x zptmp sta,
1 lda,x zptmp 1+ sta,
0 ldy,# zptmp lda,(y)
2 ora,x zptmp sta,(y)
# 1 penx +! swap 2/ swap 
penx inc, 3 bne, penx 1+ inc,
2 lsr,x rts,

create rightend
# nip 80 swap # mask
80 lda,# 2 sta,x 0 lda,# 3 sta,x

:-
2 lda,x 1 bne, rts,
0 lda,x zptmp sta,
1 lda,x zptmp 1+ sta,
0 ldy,# zptmp lda,(y)
2 and,x 1 beq, rts,
.bitblt jsr, jmp, # recurse

create bytewise
# penx @ 140 < if 
penx 1+ lda, 0 cmp,# +branch beq,
3f lda,# penx cmp, 1 bcs, rts,
:+

:- # 8 +
clc, 0 lda,x 8 adc,# 0 sta,x
2 bcc, 1 inc,x
# penx=140?
penx lda, 40 cmp,# +branch bne,
penx 1+ lda, 1 cmp,# +branch bne,
rts,
:+ :+
0 lda,x zptmp sta,
1 lda,x zptmp 1+ sta,
0 ldy,# zptmp lda,(y)
rightend -branch bne,

# ff over c!
ff lda,# zptmp sta,(y)
# 8 penx +!
clc, penx lda, 8 adc,# penx sta,
3 bcc, penx 1+ inc,
jmp, # recurse

create leave
# 2drop nip penx @ swap 
inx, inx, inx, inx,
penx lda, 2 sta,x
penx 1+ lda, 3 sta,x rts,

# this one must be fast
:asm fillr ( x y -- newx y )
# over 140 >= if exit then
3 lda,x 0 cmp,# +branch beq,
3f lda,# 2 cmp,x 3 bcs, ;asm
:+

# over penx !
2 lda,x penx sta,
3 lda,x penx 1+ sta,
# 2dup blitloc # x y mask addr
dex, dex, dex, dex,
4 lda,x 0 sta,x
5 lda,x 1 sta,x
6 lda,x 2 sta,x
7 lda,x 3 sta,x 
.blitloc jsr,

# leftend ( x y mask addr --
#           x y mask addr more? )
:-
2 lda,x +branch bne,
# continue bytewise
bytewise jsr, leave jsr, ;asm
:+
0 lda,x zptmp sta,
1 lda,x zptmp 1+ sta,
0 ldy,# zptmp lda,(y)
2 and,x +branch beq,
# done
leave jsr, ;asm
:+
.bitblt jsr, jmp, # recurse

:asm scanl
:-
# x<0?
3 lda,x 3 bpl, ;asm

addr lda, zptmp sta,
addr 1+ lda, zptmp 1+ sta,
0 ldy,# zptmp lda,(y)
mask and, 3 beq, ;asm

zptmp lda,(y)
mask ora, zptmp sta,(y)

mask asl, +branch bcc,
1 lda,# mask sta,
addr lda, sec, 8 sbc,# addr sta, 
3 bcs, addr 1+ dec,

:+ # 1-
2 lda,x 2 bne, 3 dec,x 2 dec,x
jmp, # recurse

create .scanr
# over l ! # l=x
2 lda,x l sta, 3 lda,x l 1+ sta,
;asm

:asm scanr ( x y mask addr -- newx y )
0 lda,x addr sta,
1 lda,x addr 1+ sta,
2 lda,x mask sta,
inx, inx, inx, inx,

:-
# addr @ c@ mask c@ and
addr lda, zptmp sta,
addr 1+ lda, zptmp 1+ sta,
0 ldy,# zptmp lda,(y)
mask and, .scanr -branch beq,

# x<=x2?
x2 1+ lda, 3 cmp,x .scanr -branch bcc,
+branch bne,
x2 lda, 2 cmp,x .scanr -branch bcc,
:+

mask lsr, +branch bne,
80 lda,# mask sta,
clc, addr lda, 8 adc,# addr sta,
3 bcc, addr 1+ inc,

:+ # x++
2 inc,x 2 bne, 3 inc,x
jmp, # recurse

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
spop dy @ + # y

# left line
x1 @ over # y x y
2dup blitloc addr ! mask !
scanl
over x1 @ # y x y x x1
s< not if
branch [ here @ >r 0 , ] # goto skip
then
# y x y ...
over 1+ dup l ! 
# y x y l
x1 @ < if # l < x1?
# push y,l,x1-1,-dy
dup l @ x1 @ 1- dy @ negate spush
then
# y x y
nip x1 @ 1+ swap # x=x1+1

begin
fillr
# push y,l,x-1,dy
dup l @ 3 pick 1- dy @ spush

# leak on right?
over x2 @ 1+ > if
# push y,x2+1,x-1,-dy
dup x2 @ 1+ 3 pick 1- dy @ negate spush
then

# skip: y x y
[ r> here @ over - swap ! ]

swap 1+ swap
2dup blitloc scanr 

# y x y
over x2 @ > until

2drop drop repeat ; 

hide dy
hide sx hide sy
hide err
hide x1 hide x2 hide l
hide plot4 hide plot8
hide blitop
hide colbase
hide fillr
hide dy2
hide dx2
hide step hide stepx
hide 2err
hide rightend
hide bytewise
hide leave
hide scanl
hide .scanr
hide scanr
hide .bitblt
hide spop
hide spush
hide dopush
hide stk
hide chkplot
hide doplot
hide blitloc
hide .blitloc
hide mask

: text ( col row str strlen -- )
# addr=dst
rot 140 * addr !
rot 8 * bmpbase + addr +!
# disable interrupt,enable char rom
1 c@ dup >r fb and 1 sei c!
begin ?dup while
swap dup c@ 8 * d800 + # strlen str ch
addr @ 8 cmove
1+ swap 8 addr +! 1- repeat
r> 1 c! cli drop ;

hide addr

: getbit
2* key [ key 1 literal ] = if 1+ then ;
: getrow ( dst -- dst )
0 getbit getbit getbit getbit
getbit getbit getbit getbit over c! 1+
key drop ; # skip cr
: defchar 8 allot dup value
getrow getrow getrow getrow
getrow getrow getrow getrow drop ;
: drawchar ( col row srcaddr -- )
swap 140 * rot 8 * + bmpbase +
8 cmove ;

hide bmpbase
hide getbit hide getrow
