decimal
17 constant w
w dup * constant mapsize
here mapsize allot constant m \ map
m mapsize 0 fill \ debug only

256 value r \ range
16 value s \ step size

: crnd rnd r / ;
: c>d dup $80 and if $ff00 or then ;

variable x
variable y
: plot ( y x -- )
x ! y !
x @ 15 * 64 + y @ -3 * + \ x
y @ 8 * 50 + \ y
m y @ w * x @ + + c@ c>d 2/ 2/ + \ yd
plot ;

\ init endpoints
crnd m c! \ nw
crnd m w 1- + c! \ ne
crnd m w dup * + 1- c! \ se
crnd m w dup 1- * + c! \ sw

: diamond  ( x y -- )
2dup
w * + m + dup \ nw nw
dup s + \ nw nw ne
dup s w * + \ nw nw ne se
dup s - \ nw nw ne se sw
c@ c>d swap c@ c>d + swap
c@ c>d + swap c@ c>d + 4 / \ nw avg
crnd + swap \ val nw
s 2/ + s 2/ w * +
dup c@ 0<> abort" d"
c!
s 2/ + swap s 2/ + plot ;
: diamonds
w 1- 0 do w 1- 0 do
i j diamond s +loop s +loop ;

: get x @ y @ w * m + + + c@ c>d +
swap 1+ swap ;
: square ( x y -- )
y ! x ! 0 0 \ n sum
\ sample up
y @ s 2/ - -1 > if
s 2/ w * negate get then
\ sample down
y @ s 2/ + w < if s 2/ w * get then
\ sample left
x @ s 2/ - -1 > if s 2/ negate get then
\ sample right
x @ s 2/ + w < if s 2/ get then
swap / crnd +
m x @ + y @ w * +
dup c@ 0<> abort" s"
c!
y @ x @ plot ;

: squares
w 0 do w s 2/ do
i j square s +loop s +loop
w s 2/ do w 0 do
i j square s +loop s +loop ;

: diamond-square
begin
s 1 > while
diamonds squares
r 2* to r
s 2/ to s
repeat ;

hex 52 clrcol hires
0 0 plot 0 w 1- plot
w 1- 0 plot w 1- dup plot
diamond-square
key drop lores
