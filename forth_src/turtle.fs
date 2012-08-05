

." gfx"
s" gfx" load
." sin"
s" sin" load

var tx var ty
var ta var tp

: init
hires 10 clrcol
a0 dup tx ! 64 dup ty ! plot
0 ta ! 1 tp ! ;

: right ( a -- )
ta +! ;
: left ( a -- )
negate right ;
: forward ( px -- )
dup ta @ *cos tx +!
ta @ *sin ty +!
tx @ ty @ line ;
: back ( px -- )
80 ta +! forward ff80 ta +! ;
: pendown 1 tp c!
tx @ ty @ plot ;
: penup 0 tp c! ;

# --- demo

: multishape
." a? "
." d? "
init ;

init 10 back lores
