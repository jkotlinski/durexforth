base @ hex
e000 value bmpbase
cc00 value colbase

code kernal-in
36 lda,# 1 sta, cli, ;code
code kernal-out
sei, 35 lda,# 1 sta, ;code

code hires
bb lda,# d011 sta, \ enable bitmap mode
dd00 lda,
%11111100 and,# \ vic bank 2
dd00 sta,
38 lda,# d018 sta,
;code

code lores
9b lda,# d011 sta,
dd00 lda,
%11 ora,#
dd00 sta,
17 lda,#
d018 sta,
;code

: clrcol ( fgbgcol -- )
colbase 3e8 rot fill
bmpbase 1f40 0 fill ;

: blkcol ( col row c -- )
-rot 28 * + colbase + c! ;

header mask
80 c, 40 c, 20 c, 10 c,
8 c, 4 c, 2 c, 1 c,

variable penx variable peny
0 penx ! 0 peny !

\ blit operations for plot, line
header blitop
0 , \ doplot
0 , \ lineplot

code blitloc ( x y -- mask addr )
lsb lda,x w sta,
7 and,# w3 sta,
msb lda,x w 1+ sta,

w lda, f8 and,# w sta,

\ * 8
w asl, w 1+ rol,
w asl, w 1+ rol,
w asl, w 1+ rol,

w lda, w2 sta,
w 1+ lda, w2 1+ sta,

\ * 20
w asl, w 1+ rol,
w asl, w 1+ rol,

clc,
w lda, w2 adc, w sta,
w 1+ lda, w2 1+ adc,
w 1+ sta,
clc,
w lda, w3 adc, w sta,
2 bcc, w 1+ inc,

w lda, lsb sta,x
w 1+ lda, msb sta,x

\ ...

' mask 100/
lda,# w 1+ sta,

clc,
lsb 1+ lda,x 7 and,# ' mask adc,#
w sta,
2 bcc, w 1+ inc,

\ w = mask
0 ldy,#
w lda,(y) w3 sta,

clc,
lsb 1+ lda,x f8 and,# lsb adc,x lsb sta,x
msb 1+ lda,x msb adc,x clc, e0 adc,# msb sta,x
w3 lda, lsb 1+ sta,x
0 lda,# msb 1+ sta,x
rts,

: doplot ( x y -- )
blitloc tuck c@
[ here 1+ ' blitop ! ] or
swap c! ;

: chkplot ( x y -- )
over 13f > over c7 > or
if 2drop else doplot then ;

: plot ( x y -- )
kernal-out
2dup peny ! penx ! chkplot
kernal-in ;

: peek ( x y -- b )
blitloc c@ and ;

variable dy
variable sy variable sx
variable err variable 2err

variable mask variable addr

create lineplot ( -- )

\ penx @ 140 <
penx lda,# w sta,
penx 100/ lda,# w 1+ sta,
1 ldy,# w lda,(y)
+branch beq,
1 cmp,# 1 beq, rts,
dey, w lda,(y)
sec, 40 sbc,#
1 bcc, rts,
:+

\ peny @ c8 <
peny lda,# w sta,
peny 100/ lda,# w 1+ sta,
1 ldy,# w lda,(y)
1 beq, rts,
dey, w lda,(y)
sec, c8 sbc,#
1 bcc, rts,

\ addr
addr 100/
lda,# w 1+ sta,
addr lda,# w sta,

\ @
0 ldy,#
w lda,(y) w2 sta, iny,
w lda,(y) w2 1+ sta, dey,

\ c@ mask c@ or
w2 lda,(y)
here ' blitop 2+ !
mask ora,

\ addr @ c!
w2 sta,(y) rts,

variable dx2 variable dy2

create stepx
\ 2err @ dx2 @ < if
sec, 2err lda, dx2 sbc,
2err 1+ lda, dx2 1+ sbc,
3 bmi, lineplot jmp,

\ dx2 @ err +!
clc, dx2 lda, err adc, err sta,
dx2 1+ lda, err 1+ adc, err 1+ sta,
\ sy @ peny +!
clc, sy lda, peny adc, peny sta,
sy 1+ lda, peny 1+ adc, peny 1+ sta,

\ sy @ 1 = if down else up then
sy lda, 1 cmp,# +branch beq,
\ up
addr lda, 7 and,# +branch bne,
sec, addr lda, 38 sbc,# addr sta,
addr 1+ lda, 1 sbc,# addr 1+ sta,
:+
addr lda, 3 bne, addr 1+ dec, addr dec,
lineplot jmp,
:+ \ down
addr inc, 3 bne, addr 1+ inc,
addr lda, 7 and,# 3 beq, lineplot jmp,
clc, addr lda, 38 adc,# addr sta,
addr 1+ lda, 1 adc,# addr 1+ sta,
lineplot jmp,

create step ( 2err -- 2err )
\ err @ 2* 2err !
err lda, 2err sta,
err 1+ lda, 2err 1+ sta,
2err asl, 2err 1+ rol,

\ step up/down

\ 2err @ dy2 @ > if
sec, dy2 lda, 2err sbc,
dy2 1+ lda, 2err 1+ sbc,
3 bmi, stepx jmp,

\ dy2 @ err +!
clc, dy2 lda, err adc, err sta,
dy2 1+ lda, err 1+ adc, err 1+ sta,
\ sx @ penx +!
clc, sx lda, penx adc, penx sta,
sx 1+ lda, penx 1+ adc, penx 1+ sta,

\ sx @ 1 = if maskror else maskrol then
sx lda, 1 cmp,# +branch bne,
\ right
\ maskror.mask>>1,addr+8?
mask lsr, 3 bcs, stepx jmp,
80 lda,# mask sta,
clc, addr lda, 8 adc,# addr sta,
3 bcc, addr 1+ inc, stepx jmp,
:+ \ left
\ mask<<1,addr-8?
mask asl, 3 bcs, stepx jmp,
1 lda,# mask sta,
sec, addr lda, 8 sbc,# addr sta,
3 bcs, addr 1+ dec, stepx jmp,

code doline
1 @: step jsr,
peny lda, lsb cmp,x 1 @@ bne,
penx lda, lsb 1+ cmp,x 1 @@ bne,
peny 1+ lda, msb cmp,x 1 @@ bne,
penx 1+ lda, msb 1+ cmp,x 1 @@ bne,
inx, inx, ;code

: line ( x y -- )
kernal-out
2dup peny @ - abs dy2 !
penx @ - abs dx2 !
2dup
peny @ swap < if 1 else ffff then sy !
penx @ swap < if 1 else ffff then sx !
dx2 @ dy2 @ - err !
dy2 @ negate dy2 !

penx @ peny @ blitloc addr ! mask !

doline kernal-in ;

\ --- circle

0 value cx 0 value cy

: plot4 ( x y -- x y )
over cx + over cy + chkplot
over if \ x?
over cx swap - over cy + chkplot
then
dup if \ y?
over cx + over cy swap - chkplot
then
over 0<> over 0<> and if
over cx swap - over cy swap - chkplot
then ;

: plot8 ( x y -- x y )
plot4
2dup <> if
swap plot4 swap
then ;

: circle ( cx cy r -- )
kernal-out
dup negate err !
swap to cy
swap to cx
0 \ x y
begin 2dup < 0= while
plot8
dup err +!
1+
dup err +!
err @ 0< 0= if
over negate err +!
swap 1- swap
over negate err +!
then
repeat 2drop kernal-in ;

: erase if
4d ['] xor else
d ['] or then ['] blitop @ !
['] blitop 2+ @ c! ;

\ --------------------------

\ paul heckbert seed fill
\ from graphics gems
variable stk
create dopush
stk lda, w sta,
stk 1+ lda, w 1+ sta,

\ dy
0 ldy,# lsb lda,x w sta,(y)
\ xr
iny, lsb 1+ lda,x w sta,(y)
iny, msb 1+ lda,x w sta,(y)
\ xl
iny, lsb 2 + lda,x w sta,(y)
iny, msb 2 + lda,x w sta,(y)
\ y
iny, lsb 3 + lda,x w sta,(y)

clc, stk lda, 6 adc,# stk sta,
3 bcc, stk 1+ inc, rts,

code spush ( y xl xr dy -- )
\ y out of bounds?
clc, lsb lda,x lsb 3 + adc,x tay,
msb lda,x msb 3 + adc,x +branch bne,
tya, sec, c8 cmp,# 3 bcs, dopush jsr,
:+
inx, inx, inx, inx, ;code

variable x1 variable x2

code spop ( -- y )
stk lda,
sec, 6 sbc,# w sta, stk sta,
3 bcs, stk 1+ dec,
stk 1+ lda, w 1+ sta,

\ ff = if ffff else 1 then dy !
0 ldy,# w lda,(y)
dy sta, dy 1+ sta,
1 cmp,# 3 bne, dy 1+ sty,

dex,
msb sty,x \ msb y=0
iny, w lda,(y) x2 sta,
iny, w lda,(y) x2 1+ sta,
iny, w lda,(y) x1 sta,
iny, w lda,(y) x1 1+ sta,
iny, w lda,(y) lsb sta,x
;code

variable l

\ ---

create .bitblt ( mask addr --
                  mask addr )
lsb lda,x w sta,
msb lda,x w 1+ sta,
0 ldy,# w lda,(y)
lsb 1+ ora,x w sta,(y)
\ 1 penx +! swap 2/ swap
penx inc, 3 bne, penx 1+ inc,
lsb 1+ lsr,x rts,

create rightend
\ nip 80 swap \ mask
80 lda,# lsb 1+ sta,x
0 lda,# msb 1+ sta,x

:-
lsb 1+ lda,x 1 bne, rts,
lsb lda,x w sta,
msb lda,x w 1+ sta,
0 ldy,# w lda,(y)
lsb 1+ and,x 1 beq, rts,
.bitblt jsr, jmp, \ recurse

create bytewise
\ penx @ 140 < if
penx 1+ lda, 0 cmp,# +branch beq,
3f lda,# penx cmp, 1 bcs, rts,
:+

:- \ 8 +
clc, lsb lda,x 8 adc,# lsb sta,x
2 bcc, msb inc,x
\ penx=140?
penx lda, 40 cmp,# +branch bne,
penx 1+ lda, 1 cmp,# +branch bne,
rts,
:+ :+
lsb lda,x w sta,
msb lda,x w 1+ sta,
0 ldy,# w lda,(y)
rightend -branch bne,

\ ff over c!
ff lda,# w sta,(y)
\ 8 penx +!
clc, penx lda, 8 adc,# penx sta,
3 bcc, penx 1+ inc,
jmp, \ recurse

create leavel
\ 2drop nip penx @ swap
inx, inx,
penx lda, lsb 1+ sta,x
penx 1+ lda, msb 1+ sta,x rts,

\ this one must be fast
code fillr ( x y -- newx y )
\ over 140 >= if exit then
msb 1+ lda,x 0 cmp,# +branch beq,
3f lda,# lsb 1+ cmp,x 1 bcs, rts,
:+

\ over penx !
lsb 1+ lda,x penx sta,
msb 1+ lda,x penx 1+ sta,
\ 2dup blitloc \ x y mask addr
dex, dex,
lsb 2 + lda,x lsb sta,x
msb 2 + lda,x msb sta,x
lsb 3 + lda,x lsb 1+ sta,x
msb 3 + lda,x msb 1+ sta,x
' blitloc jsr,

\ leftend ( x y mask addr --
\           x y mask addr more? )
:-
lsb 1+ lda,x +branch bne,
\ continue bytewise
bytewise jsr, leavel jsr, ;code
:+
lsb lda,x w sta,
msb lda,x w 1+ sta,
0 ldy,# w lda,(y)
lsb 1+ and,x +branch beq,
\ done
leavel jsr, ;code
:+
.bitblt jsr, jmp, \ recurse

code scanl
:-
\ x<0?
msb 1+ lda,x 1 bpl, rts,

addr lda, w sta,
addr 1+ lda, w 1+ sta,
0 ldy,# w lda,(y)
mask and, 1 beq, rts,

w lda,(y)
mask ora, w sta,(y)

mask asl, +branch bcc,
1 lda,# mask sta,
addr lda, sec, 8 sbc,# addr sta,
3 bcs, addr 1+ dec,

:+ \ 1-
lsb 1+ lda,x 2 bne, msb 1+ dec,x
lsb 1+ dec,x
jmp, \ recurse

create .scanr
\ over l ! \ l=x
lsb 1+ lda,x l sta,
msb 1+ lda,x l 1+ sta,
;code

code scanr ( x y mask addr -- newx y )
lsb lda,x addr sta,
msb lda,x addr 1+ sta,
lsb 1+ lda,x mask sta,
inx, inx,

:-
\ addr @ c@ mask c@ and
addr lda, w sta,
addr 1+ lda, w 1+ sta,
0 ldy,# w lda,(y)
mask and, .scanr -branch beq,

\ x<=x2?
x2 1+ lda, msb 1+ cmp,x .scanr -branch bcc,
+branch bne,
x2 lda, lsb 1+ cmp,x .scanr -branch bcc,
:+

mask lsr, +branch bne,
80 lda,# mask sta,
clc, addr lda, 8 adc,# addr sta,
3 bcc, addr 1+ inc,

:+ \ x++
lsb 1+ inc,x 2 bne, msb 1+ inc,x
jmp, \ recurse

: paint ( x y -- )
2dup c8 < 0= swap 140 < 0= or
if 2drop exit then
kernal-out
2dup peek if 2drop kernal-in exit then

here stk !
\ push y x x 1
2dup swap dup 1 spush
\ push y+1 x x -1
1+ swap dup ffff spush

begin here stk @ < while
spop dy @ + \ y

\ left line
x1 @ over \ y x y
2dup blitloc addr ! mask !
scanl
over x1 @ \ y x y x x1
< 0= if
branch [ here dy ! 0 , ] \ goto skip
then
\ y x y ...
over 1+ dup l !
\ y x y l
x1 @ < if \ l < x1?
\ push y,l,x1-1,-dy
dup l @ x1 @ 1- dy @ negate spush
then
\ y x y
nip x1 @ 1+ swap \ x=x1+1

begin
fillr
\ push y,l,x-1,dy
dup l @ 3 pick 1- dy @ spush

\ leak on right?
over x2 @ 1+ > if
\ push y,x2+1,x-1,-dy
dup x2 @ 1+ 3 pick 1- dy @ negate spush
then

\ skip: y x y
[ here dy @ ! ]

swap 1+ swap
2dup blitloc scanr

\ y x y
over x2 @ > until

2drop drop repeat kernal-in ;

here
80 c, 81 c, 82 c, 83 c, 84 c, 85 c, 86 c, 87 c, \ 0
88 c, 89 c, 8a c, 8b c, 8c c, 8d c, 8e c, 8f c,
90 c, 91 c, 92 c, 93 c, 94 c, 95 c, 96 c, 97 c, \ 1
98 c, 99 c, 9a c, 9b c, 9c c, 9d c, 9e c, 9f c,
20 c, 21 c, 22 c, 23 c, 24 c, 25 c, 26 c, 27 c, \ 2
28 c, 29 c, 2a c, 2b c, 2c c, 2d c, 2e c, 2f c,
30 c, 31 c, 32 c, 33 c, 34 c, 35 c, 36 c, 37 c, \ 3
38 c, 39 c, 3a c, 3b c, 3c c, 3d c, 3e c, 3f c,
00 c, 01 c, 02 c, 03 c, 04 c, 05 c, 06 c, 07 c, \ 4
08 c, 09 c, 0a c, 0b c, 0c c, 0d c, 0e c, 0f c,
10 c, 11 c, 12 c, 13 c, 14 c, 15 c, 16 c, 17 c, \ 5
18 c, 19 c, 1a c, 1b c, 1c c, 1d c, 1e c, 1f c,
40 c, 41 c, 42 c, 43 c, 44 c, 45 c, 46 c, 47 c, \ 6
48 c, 49 c, 4a c, 4b c, 4c c, 4d c, 4e c, 4f c,
50 c, 51 c, 52 c, 53 c, 54 c, 55 c, 56 c, 57 c, \ 7
58 c, 59 c, 5a c, 5b c, 5c c, 5d c, 5e c, 5f c,
c0 c, c1 c, c2 c, c3 c, c4 c, c5 c, c6 c, c7 c, \ 8
c8 c, c9 c, ca c, cb c, cc c, cd c, ce c, cf c,
d0 c, d1 c, d2 c, d3 c, d4 c, d5 c, d6 c, d7 c, \ 9
d8 c, d9 c, da c, db c, dc c, dd c, de c, df c,
60 c, 61 c, 62 c, 63 c, 64 c, 65 c, 66 c, 67 c, \ a
68 c, 69 c, 6a c, 6b c, 6c c, 6d c, 6e c, 6f c,
70 c, 71 c, 72 c, 73 c, 74 c, 75 c, 76 c, 77 c, \ b
78 c, 79 c, 7a c, 7b c, 7c c, 7d c, 7e c, 7f c,
40 c, 41 c, 42 c, 43 c, 44 c, 45 c, 46 c, 47 c, \ c
48 c, 49 c, 4a c, 4b c, 4c c, 4d c, 4e c, 4f c,
50 c, 51 c, 52 c, 53 c, 54 c, 55 c, 56 c, 57 c, \ d
58 c, 59 c, 5a c, 5b c, 5c c, 5d c, 5e c, 5f c,
60 c, 61 c, 62 c, 63 c, 64 c, 65 c, 66 c, 67 c, \ e
68 c, 69 c, 6a c, 6b c, 6c c, 6d c, 6e c, 6f c,
70 c, 71 c, 72 c, 73 c, 74 c, 75 c, 76 c, 77 c, \ f
78 c, 79 c, 7a c, 7b c, 7c c, 7d c, 7e c, 5e c,
: pet>scr [ swap ] literal + c@ ;

: text ( col row str strlen -- )
kernal-out
\ addr=dst
rot 140 * addr !
rot 8 * bmpbase + addr +!
\ disable interrupt,enable char rom
1 c@ dup >r fb and 1 c!
begin ?dup while
swap dup c@ pet>scr 8 * d800 +
addr @ 8 move
1+ swap 8 addr +! 1- repeat
r> 1 c! drop kernal-in ;

: drawchar ( col row srcaddr -- )
kernal-out
swap 140 * rot 8 * + bmpbase +
8 move kernal-in ;
base !
