decimal
15 constant w
w dup * constant mapsize
here mapsize allot constant map
map mapsize 0 fill

: crnd rnd 256 / ;

\ init endpoints
crnd map c! \ nw
crnd map w 1- + c! \ ne
crnd map w dup * + 1- c! \ se
crnd map w dup 1- * + c! \ sw

: c>d dup $80 and if $ff00 or then ;

: plot-3d
w 0 do w 0 do
j 15 * 88 + i -5 * + \ x
i 8 * 50 + \ y
map i w * j + + c@ c>d 2/ 2/ + \ yd
plot loop loop ;

hex 52 clrcol hires
plot-3d
key drop lores
