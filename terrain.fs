decimal
17 constant w
: w* dup 2* 2* 2* 2* + ;
w dup * constant mapsize
here mapsize allot constant m \ map

code c>d lsb lda,x $80 and,# 1 @@ beq,
$ff lda,# msb sta,x 1 @: ;code

0 value x
0 value y
: coord ( y x -- y x )
to x to y
x 15 * 64 + y -3 * +
y 2* 2* 2* 50 +
m y w* x + + c@ c>d 2/ 2/ + ;

: tri-nw ( y x -- )
2dup coord plot 2dup 1+ coord line 2dup
swap 1+ swap coord line coord line ;
: tri-sw ( y x -- )
2dup 1+ coord plot
2dup 1+ swap 1+ swap coord line
2dup swap 1+ swap coord line 1+
coord line ;
: wireframe w 1- 0 do w 1- 0 do
i j 2dup tri-nw tri-sw loop loop ;

: create-map s" map" m loadb if
s" mapcreate" included then ;
create-map

hex 52 clrcol hires
wireframe
key drop lores
