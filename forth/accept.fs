0 value addr
: accept ( addr avail -- len )
swap to addr 0 ( avail len )
0 $cc c! \ cursor on
begin key case
$d of \ cr
 1 $cc c! nip space exit endof
$14 of \ del
 dup if 1- $14 emit then endof
( avail len char ) \ add to buffer?
>r 2dup > r@ bl $7f within and if
 r@ over addr + c! 1+ r@ emit then r>
endcase again ;
hide addr
