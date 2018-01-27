decimal
33 constant w
w dup * constant mapsize
here mapsize allot constant map
map mapsize 0 fill

\ init endpoints
rnd map c! \ nw
rnd map w 1- + c! \ ne
rnd map w dup * + 1- c! \ se
rnd map w dup 1- * + c! \ sw

: plot-2d
w 0 do w 0 do
j w * i + map + c@ if
i j plot then
loop loop ;

hex 52 clrcol hires
plot-2d
key quit
