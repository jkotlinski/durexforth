decimal
17 constant w
w dup * constant mapsize
here mapsize allot constant map
map mapsize 0 fill

variable range 256 range !
variable stepsize 16 stepsize !

: crnd rnd range @ / ;

\ init endpoints
crnd map c! \ nw
crnd map w 1- + c! \ ne
crnd map w dup * + 1- c! \ se
crnd map w dup 1- * + c! \ sw

: diamond . . cr ;
: diamonds
w 0 do w 0 do
j i diamond
stepsize @ +loop
stepsize @ +loop ;
: squares ;
: diamond-square
begin
stepsize @ 1 > while
diamonds squares
range @ 2* range !
stepsize @ 2/ stepsize !
repeat ;
diamond-square

: c>d dup $80 and if $ff00 or then ;

: plot-3d
w 0 do w 0 do
j 15 * 64 + i -3 * + \ x
i 8 * 50 + \ y
map i w * j + + c@ c>d 2/ 2/ + \ yd
plot loop loop ;

hex 52 clrcol hires
plot-3d
key drop lores
