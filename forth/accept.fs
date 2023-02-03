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
hide curr
