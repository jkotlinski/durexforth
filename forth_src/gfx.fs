

: bmpbase a000 ;
: colbase 8c00 ;

:asm hires 
bb lda,# d011 sta, # enable
15 lda,# dd00 sta, # vic bank 2
38 lda,# d018 sta,
56 lda,# 1 sta, # no basic
;asm

:asm lores
9b lda,# d011 sta,
17 lda,# dd00 sta,
17 lda,# d018 sta,
;asm

: clrcol ( fgbgcol -- )
colbase 3e8 fill
0 bmpbase 1f40 fill ;

: blkcol ( x y c -- )
rot 8 / rot 8 / 28 *
+ colbase + c! ;

create mask
80 c, 40 c, 20 c, 10 c,
8 c, 4 c, 2 c, 1 c,

var penx var peny

: plot ( x y -- )
2dup peny ! penx !
dup fff8 and 28 *
swap 7 and + swap # y x
dup 7 and ['] mask + c@ # y x bit
-rot # bit y x
fff8 and + bmpbase +
dup c@ rot or swap c! ;

var dx var dy

: lineh
dx @ 8000 and if
# left
begin dx @ while
penx @ peny @ plot
ffff penx +! 1 dx +!
repeat
else
# right
begin dx @ while
penx @ peny @ plot
1 penx +! ffff dx +!
repeat
then ;

: linev
dy @ 8000 and if
# up
begin dy @ while
penx @ peny @ plot
ffff peny +! 1 dy +!
repeat
else
# down
begin dy @ while
penx @ peny @ plot
1 peny +! ffff dy +!
repeat
then ;

: line ( x y -- )
peny @ - dy ! penx @ - dx !
dy @ if
 dx @ if
 else
  linev
 then
else
 lineh
then ;
