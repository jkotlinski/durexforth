

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
# angle step (0-ff)
." a? " interpret
# init distance
." d? " interpret 
hires 7 clrcol
5 d020 c!
init
1 begin
over forward
2 pick right
swap 3 + swap
1+ dup 200 = until drop ;
