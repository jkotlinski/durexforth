

s" gfx" load
s" sin" load

var tx var ty # 9.7 fixedpoint
var ta 
: tp 0 ;

: s2/ ( signed 2/ )
2/ dup 4000 and if 8000 or then ;
: ls 2* 2* 2* 2* 2* 2* 2* ;
: rs s2/ s2/ s2/ s2/ s2/ s2/ s2/ ;
: pendown 1 to tp tx @ rs ty @ rs plot ;
: penup 0 to tp ;

: init a0 ls tx ! 64 ls ty ! pendown 
10e ta ! ( north ) ;

: right ( a -- )
ta @ +
dup 8000 and if 168 + then
168 mod ta ! ;
: left ( a -- )
negate right ;
: forward ( px -- )
ls dup ta @ *cos tx +!
ta @ *sin ty +!
tp if tx @ rs ty @ rs line then ;
: back ( px -- )
80 right forward 80 right ;

# --- demo

: polyspiral
." init distance? " interpret 
." angle?" interpret
." distance step? " interpret 
hires 7 clrcol
init
1 >r begin
2 pick forward
over left
rot over + -rot
r> 1+ dup >r 64 = until r> drop
5 d020 c! key lores 0 d020 c! ;

: inward
." distance? " interpret 
." init angle? " interpret
." angle step? " interpret 
hires 7 clrcol init
begin 2 pick forward
over right
swap over + 168 mod swap again ;
