

s" gfx" load
s" sin" load

var tx var ty var ta var tp

: pendown 1 tp ! tx @ ty @ plot ;
: penup 0 tp ! ;

: init 10e ta ! a0 tx ! 64 ty ! pendown ;

: right ( a -- )
ta +! ;
: left ( a -- )
negate right ;
: forward ( px -- )
dup ta @ *cos tx +!
ta @ *sin ty +!
tp @ if tx @ ty @ line then ;
: back ( px -- )
80 right forward 80 right ;

# --- demo

: polyspiral
# init distance
." d? " interpret 
# angle step (degrees)
." a? " interpret
# distance step
." c? " interpret 
hires 7 clrcol
init
1 >r begin
2 pick forward
over right
rot over + -rot
r> 1+ dup >r 64 = until r> drop
5 d020 c! key lores 0 d020 c! ;

: inward
# distance
." d? " interpret 
# init angle (deg)
." a? " interpret
# angle step (deg)
." c? " interpret 
hires 7 clrcol init
begin 2 pick forward
over right
swap over + 168 mod swap again ;
