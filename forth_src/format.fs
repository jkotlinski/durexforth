variable end
create buf #34 allot
: <# buf end ! ;
: #> 2drop buf end @ over - ;
: hold
\ reserve space for char at start
buf dup 1+ end @ buf - move
1 end +!
buf c! ;
: sign 0< if '-' hold then ;
: # base @ ud/mod rot
dup a < if 7 - then $37 + hold ;
: #s # begin 2dup or while # repeat ;

: u. 0 <# #s #> type space ;

\ this is slow :(
: . dup abs 0 <# #s rot sign #>
type space ;

variable curr
: accept ( addr u -- u )
$cc >r 0 $cc c! \ enable cursor
swap dup >r curr !
begin
 key
 dup $d = if \ cr
  2drop curr @ r> -
  space r> $cc c! \ reset cursor
  exit
 else dup $14 = if \ del
  curr @ r@ > if
   emit -1 curr +! 1+
  else drop then
 else dup $7f and $20 < if
  drop \ ignore
 else
  \ process character
  over if dup curr @ c!
   emit 1- 1 curr +!
  else drop then
 then then then
again ;
