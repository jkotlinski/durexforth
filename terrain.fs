decimal
15 constant w
w dup * constant mapsize
here mapsize allot constant map
map mapsize 0 fill

\ init endpoints
rnd map c! \ nw
rnd map w 1- + c! \ ne
rnd map w dup * + 1- c! \ se
rnd map w dup 1- * + c! \ sw

( : plot-2d
w 0 do w 0 do
j w * i + map + c@ if
i j plot then
loop loop ; )

: plot-3d
w 0 do w 0 do
j 15 * 88 + i -5 * + \ x
i 8 * 50 + \ y
plot loop loop ;

hex 52 clrcol hires
plot-3d
key quit
