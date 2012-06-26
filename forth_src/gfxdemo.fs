

# examples from c64 step by step
# programming, gfx book 3, phil cornes

s" rnd" load
s" sin" load

: lineweb
hires 7 clrcol
5 begin
dup 140 < while
dup 0 plot 96 c8 line
dup c7 plot 96 0 line
a + repeat drop ;

: rndline
hires 10 clrcol
80 begin ?dup while
rnd ab / 20 -
rnd f8 / 20 - line
1- repeat ;

: radiant
hires d0 clrcol
100 begin
?dup while
32 64 plot
dup 12c *cos 32 +
over 12c *sin 64 +
line
1- repeat ;

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
5 + repeat ;

: reccircgo ( x r -- )
dup if
2dup 64 swap circle
2dup + over 2/ recurse
2dup - over 2/ recurse
then 2drop ;

: reccirc
hires 7 clrcol
a0 50 reccircgo ;

var yd

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

: erasecirc
hires 7 clrcol
0 begin dup 140 < while
dup 0 plot dup c7 line
14 + repeat drop
0 begin dup c7 < while
dup 0 swap plot dup 13f swap line
14 + repeat drop
begin 100 begin ?dup while
a0 64 plot
dup 64 swap *cos a0 +
over 64 swap *sin 64 + line
1- repeat 1 erase again ;

: rotsqr
hires 16 clrcol
8 d020 c! 1 erase
begin fa begin ?dup while
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
5 - repeat again ;

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
68 begin dup 138 <= while
20 begin dup 88 < while
2dup be blkcol
8 + repeat begin dup c4 < while
2dup 6 blkcol
8 + repeat drop 8 + repeat drop
e8 begin dup 118 <= while
50 begin dup 80 <= while
2dup 7e blkcol
8 + repeat drop 8 + repeat drop
begin again ;

# seascape
