\ examples from c64 step by step
\ programming, gfx book 3, phil cornes

require gfx
require rnd
require sin
hex

d020 c@

: blkcol rot 2/ 2/ 2/
rot 2/ 2/ 2/ rot blkcol ;

.( lineweb..)
: lineweb
7 clrcol
5 begin
dup 140 < while
dup 0 plot 96 c8 line
dup c7 plot 96 0 line
a + repeat drop ;
hires lineweb key drop lores

.( rndline..)
: rndline
hires 10 clrcol
80 begin ?dup while
rnd 0 ab um/mod nip 20 -
rnd 0 f8 um/mod nip 20 - line
1- repeat ;
hires rndline key drop lores

.( radiant..)
: radiant
hires d0 clrcol
168 begin
?dup while
32 64 plot
12c over *cos 32 +
over 12c swap *sin 64 +
line
1- repeat ;
hires radiant key drop lores

.( diamond..)
: diamond
hires 12 clrcol
2 d020 c!
0 64 plot
0 begin
dup c8 < while
a0 over line
13f 64 line
a0 over c7 swap - line
0 64 line
5 + repeat drop ;
hires diamond key drop lores

.( reccirc..)
: reccircgo ( x r -- )
dup if
2dup 64 swap circle
2dup + over 2/ recurse
2dup - over 2/ recurse
then 2drop ;

: reccirc
hires 7 clrcol
a0 50 reccircgo ;
hires reccirc key drop lores

.( 2reccirc..)
variable yd

: 2reccircgo ( x r -- )
dup if
2dup yd @ swap circle
2dup c7 yd @ - swap circle
d yd +!
2dup + over 2/ recurse
2dup - over 2/ recurse
d negate yd +!
then 2drop ;

: 2reccirc
hires 7 clrcol
64 yd !
a0 50 2reccircgo ;
hires 2reccirc key drop lores

.( erasecirc..)
: erasecirc
hires 7 clrcol
0 begin dup 140 < while
dup 0 plot dup c7 line
14 + repeat drop
0 begin dup c7 < while
dup 0 swap plot dup 13f swap line
14 + repeat drop
2 0 do 168 begin ?dup while
a0 64 plot
dup 64 swap *cos a0 +
over 64 swap *sin 64 + line
1- repeat 1 erase loop
0 erase ;
hires erasecirc key drop lores

.( rotsqr..)
: rotsqr
hires 16 clrcol
8 d020 c! 1 erase
2 0 do fa begin ?dup while
dup dup *cos a0 +
over dup *sin 64 + 2dup plot plot
dup dup *sin a0 swap -
over dup *cos 64 + line
dup dup *cos a0 swap -
over dup *sin 64 swap - line
dup dup *sin a0 +
over dup *cos 64 swap - line
dup dup *cos a0 +
over dup *sin 64 + line
5 - repeat loop 0 erase ;
hires rotsqr key drop lores

.( seascape..)
: seascape
0 d020 c!
hires e clrcol
0 38 plot 10 28 line 30 48 line
40 40 line 70 98 line 78 8c line
88 a0 line a0 90 line d0 a8 line
f8 90 line 13f b0 line
68 88 plot 88 60 line a0 70 line
a8 68 line c8 80 line d8 78 line
e8 88 line
121 87 plot 138 60 line 13f 68 line
30 a0 paint
69 88 plot 13f 88 line
88 70 paint 138 70 paint
104 6c f circle 104 6c paint

139 68 do
88 20 do j i be blkcol 8 +loop
c4 88 do j i 6 blkcol 8 +loop
8 +loop

119 e8 do 81 50 do
j i 7e blkcol
8 +loop 8 +loop ;
hires seascape key drop lores

.( jungle..)
header jungledata
c , a8 , ffff , 1 , e , 5f ,
2f , 5f , f , 57 , f , 18 ,
1c , 38 , 1c , 26 , 20 , 30 ,
24 , 10 , 30 , 3f , 54 , a8 ,
50 , ac , 44 , a4 , 40 , a8 ,
3d , 9c , 3a , a0 , 35 , 96 ,
30 , a8 , 23 , 9a , 20 , a2 ,
16 , 97 , c , a8 , ffff , 0 ,
64 , a0 , ffff , 1 , 4f , 60 ,
35 , 14 , 38 , 14 , 45 , 21 ,
48 , 14 , 54 , 38 , 58 , 30 ,
5b , 3c , 68 , 2b , 70 , 3c ,
70 , 2c , 78 , 47 , 84 , 42 ,
94 , 47 , 94 , 2c , b8 , 47 ,
c0 , 50 , b8 , 60 , 96 , a0 ,
8c , a0 , 88 , 9a , 7e , 94 ,
78 , a0 , 72 , 96 , 72 , 7e ,
6c , 96 , 64 , a0 , ffff , 0 ,
a8 , a0 , ffff , 1 , cc , 60 ,
e3 , 80 , f4 , a6 , e4 , ac ,
d8 , a0 , d0 , a4 , c4 , 94 ,
be , a0 , aa , a6 , a8 , a0 ,
ffff , 0 , cc , 47 , ffff , 1 ,
a0 , 22 ,
ac , 11 , b2 , 14 , c4 , b ,
cc , 18 , d4 , 18 , d6 , 1e ,
e0 , 18 , e4 , 1e , f4 , e ,
fa , 16 , cc , 47 , ffff , 0 ,
d8 , 54 , ffff , 1 , e7 , 3f ,
10a , 1c ,
10c , 2c , 116 , 20 , 119 , 38 ,
120 , 30 , 124 , 38 , 12a , 28 ,
134 , 22 , 131 , 40 , 110 , 40 ,
e8 , 47 , 12f , 47 , 11e , a8 ,
118 , a8 , 110 , a0 , 108 , a8 ,
d8 , 54 , ffff , 0 , b7 , 5f ,
ffff , 1 ,
50 , 5f , 64 , 58 , 78 , 50 ,
90 , 4a , b7 , 48 , b7 , 5f ,
fffe , 0 ,
0 , 138 , 0 , 38 , 6 ,
0 , 138 , 60 , c0 , d ,
0 , 8 , 40 , 58 , 0 ,
10 , 28 , 40 , 58 , d6 ,
30 , 48 , 40 , 58 , 6 ,
50 , b0 , 48 , 58 , d6 ,
50 , e0 , 40 , 40 , 6 ,
b8 , 128 , 48 , 58 , d ,
e8 , 128 , 40 , 40 , d6 ,
130 , 138 , 40 , 58 , 0 ,

variable line? variable data

: jcol
data @ @ 2 data +!
data @ @ 2 data +!
data @ @ 2 data +!
data @ @ 2 data +!
data @ @ 2 data +! \ lx ux ly uy c
4 pick \ lx ux ly uy c x
begin dup 5 pick > 0= while
3 pick \ lx ux ly uy c x y
begin dup 4 pick > 0= while
2dup 4 pick blkcol
8 + repeat drop 8 + repeat
2drop 2drop 2drop ;

: jungle
hires 10 clrcol 0 d020 c!
0 line? ! ['] jungledata data !
begin
data @ @ 2 data +!
data @ @ 2 data +! \ x y
over ffff = if line? ! drop
else over fffe = if
2drop a0 50 paint a0 b4 paint
jcol jcol jcol jcol jcol
jcol jcol jcol jcol jcol
exit then
line? @ if line else plot then
then again ;
hires jungle key drop lores

.( colorchart..)

create sqr
%00000000 c,
%00000000 c,
%00111100 c,
%00111100 c,
%00111100 c,
%00111100 c,
%00000000 c,
%00000000 c,

: colorchart
hires 9c clrcol c d020 c!
80 0 plot 7c 17 line
94 17 line 96 c line
aa c line ac 17 line
c4 17 line c0 0 line
60 c7 plot 64 a8 line
7c a8 line 78 c8 line
c8 c8 line c4 a8 line
dc a8 line e0 c7 line
78 b4 paint d0 b4 paint a0 1 paint

\ black board
f9 40 do a1 18 do
j i 10 blkcol
8 +loop 8 +loop

b 4 s" 0" text
b 5 s" 1" text
b 6 s" 2" text
b 7 s" 3" text
b 8 s" 4" text
b 9 s" 5" text
b a s" 6" text
b b s" 7" text
b c s" 8" text
b d s" 9" text
a e s" 10" text
a f s" 11" text
a 10 s" 12" text
a 11 s" 13" text
a 12 s" 14" text
a 13 s" 15" text

0 begin dup 10 < while
0 begin dup 10 < while
2dup 2dup swap 2* 2* 2* 2* or
rot d + 8 * rot 4 + 8 * rot blkcol
2dup swap d + swap 4 + sqr drawchar
1+ repeat drop
1+ repeat drop ;
hires colorchart key drop lores

d020 c!
