

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

: multishape
init
." a? "
." d? "
hires 7 clrcol ;
