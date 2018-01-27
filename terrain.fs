decimal
17 constant w
w dup * constant mapsize
here mapsize allot constant map
map mapsize 0 fill \ debug only

variable r 256 r ! \ range
16 value s \ step size

: crnd rnd r @ / ;
: c>d dup $80 and if $ff00 or then ;

\ init endpoints
crnd map c! \ nw
crnd map w 1- + c! \ ne
crnd map w dup * + 1- c! \ se
crnd map w dup 1- * + c! \ sw

: diamond  ( x y -- )
w * + map + \ nw
dup s + \ nw ne
dup s w * + \ nw ne se
dup s - \ nw ne se sw
c@ c>d swap c@ c>d + swap
c@ c>d + swap c@ c>d + 4 / \ avg
drop \ todo
;

: diamonds
w 1- 0 do w 1- 0 do
i j diamond
s +loop
s +loop ;
: squares ;
: diamond-square
begin
s 1 > while
diamonds squares
r @ 2* r !
s 2/ to s
repeat ;
diamond-square

: plot-3d
w 0 do w 0 do
j 15 * 64 + i -3 * + \ x
i 8 * 50 + \ y
map i w * j + + c@ c>d 2/ 2/ + \ yd
plot loop loop ;

hex 52 clrcol hires
plot-3d
key drop lores
