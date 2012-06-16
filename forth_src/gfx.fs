

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
0 penx ! 0 peny !

: blitop 0 ;

: blitloc ( x y -- mask addr )
dup fff8 and 28 *
swap 7 and + swap # y x
dup 7 and ['] mask + c@ # y x bit
-rot # bit y x
fff8 and + bmpbase + ;

: plot ( x y -- )
2dup peny ! penx !
2dup c8 >= swap 140 >= or
if 2drop exit then
blitloc swap over c@
[ here @ to blitop ] or swap c! ;

: peek ( x y -- b )
blitloc c@ and ;

: dx 0 ; : dy 0 ;
: sx 0 ; : sy 0 ;
var err

: line ( x y -- )
2dup peny @ - abs to dy
penx @ - abs to dx
2dup
peny @ swap s< if 1 else ffff then to sy
penx @ swap s< if 1 else ffff then to sx
dx dy - err !
dy negate to dy

begin
 err @ 2* dup
 dy s> if
  dy err +!
  sx penx +! 
 then
 dx s< if
  dx err +!
  sy peny +!
 then
 penx @ peny @ plot
 2dup peny @ = swap penx @ = and if
  2drop exit
 then
again ;

: cx 0 ;
: cy 0 ;

: plot4 ( x y -- x y )
over cx + over cy + plot
over if # x?
over cx swap - over cy + plot
then
dup if # y?
over cx + over cy swap - plot
then
over not not over not not and if
over cx swap - over cy swap - plot
then ;

: plot8 ( x y -- x y )
plot4
2dup <> if
swap plot4 swap
then ;

: circle ( cx cy r -- )
dup negate err !
swap to cy
swap to cx
0 # x y
begin 2dup s< not while
plot8
dup err +!
1+
dup err +!
err @ 0< not if
over negate err +!
swap 1- swap
over negate err +!
then
repeat 2drop ;

: erase if
['] xor else
['] or then blitop ! ;

: flood ( x y -- )
2dup peek if 2drop exit then

# line to right, find maxX
over # x y x1
begin 2dup swap peek not # x y x1 set
over 140 < and while
2dup swap plot 1+ repeat
1- >r # push maxX

# x y

# line left, find minX
swap 1- # y x2
begin 2dup swap peek not
over ffff s> and while
2dup swap plot 1- repeat
1+ # y minX

# y minX
begin dup r@ < while
swap # minX y
# recurse up
dup if 2dup 1- peek not if
2dup 1- recurse then then
# recurse down
dup c7 < if 2dup 1+ peek not if
2dup 1+ recurse then then
# minX y
swap 1+ repeat r> drop 2drop ;

hires 5 clrcol
10 10 plot
20 10 line
20 40 line
10 40 line
10 10 line
18 18 flood
