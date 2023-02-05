header init     ( -- )
header forward  ( px -- )
header back     ( px -- )
header left     ( deg -- )
header right    ( deg -- )
header penup    ( -- )
header pendown  ( -- )
header turtle@  ( -- x y deg )
header turtle!  ( x y deg -- )
header moveto   ( x y deg -- )

require gfx
require sin

latest

variable tx variable ty \ 10.6 fixedpoint
variable ta 0 value tp

code ls \ left shift x6
lsb asl,x msb rol,x lsb asl,x msb rol,x
lsb asl,x msb rol,x lsb asl,x msb rol,x
lsb asl,x msb rol,x lsb asl,x msb rol,x
;code

code rs \ right shift x6 (sign extend)
msb lda,x $80 cmp,# msb ror,x lsb ror,x
msb lda,x $80 cmp,# msb ror,x lsb ror,x
msb lda,x $80 cmp,# msb ror,x lsb ror,x
msb lda,x $80 cmp,# msb ror,x lsb ror,x
msb lda,x $80 cmp,# msb ror,x lsb ror,x
msb lda,x $80 cmp,# msb ror,x lsb ror,x
;code

define pendown
1 to tp tx @ rs ty @ rs plot ;
define penup 0 to tp ;

define moveto
ta ! ls ty ! ls tx !
pendown ;
define init hires 7 clrcol
$a0 $64 #270 moveto ;

define right ta +! ;
define left negate right ;
define forward
ls dup ta @ *cos tx +!
ta @ *sin ty +!
tp if tx @ rs ty @ rs line then ;
define back
$80 right forward $80 right ;

define turtle@
tx @ ty @ ta @ ;
define turtle!
ta ! ty ! tx ! pendown ;

to latest

(
\ --- demo - not working :(

: polyspiral
." init distance? " interpret
." angle? " interpret
." distance step? " interpret
init
$64 >r begin
2 pick forward
over left
rot over + -rot
r> 1- dup >r 0= until r> 2drop 2drop
5 $d020 c! key drop lores 0 $d020 c! ;

: inward
." distance? " interpret
." init angle? " interpret
." angle step? " interpret
init
begin 2 pick forward
over right
tuck + $168 mod swap again ;
)
