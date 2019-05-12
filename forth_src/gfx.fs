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
: pet>scr literal + c@ ;

variable addr

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
