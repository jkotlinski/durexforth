decimal
17 constant w
w dup * constant mapsize
here mapsize allot constant m \ map
m mapsize 0 fill \ debug only

256 value r \ range
16 value s \ step size

: crnd rnd r / ;
: c>d dup $80 and if $ff00 or then ;

\ init endpoints
crnd m c! \ nw
crnd m w 1- + c! \ ne
crnd m w dup * + 1- c! \ se
crnd m w dup 1- * + c! \ sw

: diamond  ( x y -- )
w * + m + dup \ nw nw
dup s + \ nw nw ne
dup s w * + \ nw nw ne se
dup s - \ nw nw ne se sw
c@ c>d swap c@ c>d + swap
c@ c>d + swap c@ c>d + 4 / \ nw avg
crnd + swap \ val nw
s 2/ + s 2/ w * + c! ;
: diamonds
w 1- 0 do w 1- 0 do
i j diamond s +loop s +loop ;

variable x
variable y
variable n
: square ( x y -- )
2dup . . cr
y ! x ! 0 n ! 0 \ sum
\ sample up
y @ s 2/ - -1 > if
x @ y @ s 2/ - w * m + + c@ c>d +
1 n +! then
\ sample down
y @ s 2/ + w < if
x @ y @ s 2/ + w * m + + c@ c>d +
1 n +! then
\ sample left
x @ s 2/ - -1 > if
x @ s 2/ - y @ w * m + + c@ c>d +
1 n +! then
\ sample right
x @ s 2/ + w < if
x @ s 2/ + y @ w * m + + c@ c>d +
1 n +! then
n @ / crnd +
m x @ + y @ w * + c! ;

: squares
w 1- 0 do w 1- 0 do
i s 2/ + j square \ u
i s 2/ + j s + square \ d
i j s 2/ + square \ l
i s + j s 2/ + square \ r
s +loop s +loop ;

: diamond-square
begin
s 1 > while
diamonds squares
r 2* to r
s 2/ to s
repeat ;
diamond-square

: plot-3d
w 0 do w 0 do
j 15 * 64 + i -3 * + \ x
i 8 * 50 + \ y
m i w * j + + c@ c>d 2/ 2/ + \ yd
plot loop loop ;

hex 52 clrcol hires
plot-3d
key drop lores
