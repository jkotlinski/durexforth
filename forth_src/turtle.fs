

." gfx"
s" gfx" load
." sin"
s" sin" load

var tx var ty var ta var tp

: pendown 1 tp ! tx @ ty @ plot ;
: penup 0 tp ! ;

: init 0 ta ! a0 tx ! 64 ty ! pendown ;

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
# angle step (0-ff)
." a? " interpret
# distance step
." c? " interpret 
hires 7 clrcol
5 d020 c!
init
1 >r begin
2 pick forward
over right
rot over + -rot
r> 1+ dup >r 100 = until r> drop ;
