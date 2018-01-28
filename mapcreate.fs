256 value r \ range
16 value s \ step size

: crnd rnd r / ;

: diamond  ( x y -- )
w* + m + dup \ nw nw
dup s + \ nw nw ne
dup s w* + \ nw nw ne se
dup s - \ nw nw ne se sw
c@ c>d swap c@ c>d + swap
c@ c>d + swap c@ c>d + 4 / \ nw avg
crnd + swap \ val nw
s 2/ + s 2/ w* + c! ;
: diamonds
w 1- 0 do w 1- 0 do
i j diamond s +loop s +loop ;

: get x y w* m + + + c@ c>d +
swap 1+ swap ;
: square ( x y -- )
to y to x 0 0 \ n sum
\ sample up
y s 2/ - -1 > if
s 2/ w* negate get then
\ sample down
y s 2/ + w < if s 2/ w* get then
\ sample left
x s 2/ - -1 > if s 2/ negate get then
\ sample right
x s 2/ + w < if s 2/ get then
swap / crnd +
m x + y w* + c! ;

: squares
w 0 do w s 2/ do
i j square s +loop s +loop
w s 2/ do w 0 do
i j square s +loop s +loop ;

: init-corners
crnd m c! \ nw
crnd m w 1- + c! \ ne
crnd m w dup * + 1- c! \ se
crnd m w dup 1- * + c! ; \ sw

: diamond-square
init-corners begin
s 1 > while s . diamonds squares
r 2* to r s 2/ to s repeat ;

diamond-square
m m mapsize + s" map" saveb
