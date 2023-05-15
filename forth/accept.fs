0 value addr
: accept ( addr avail -- len )
0 $cc ( enable cursor-blink ) c!
swap to addr 0 ( avail len )
begin key case
\ return and delete:
$0d of nip space 1 $cc c! exit endof
$14 of dup if 1- $14 emit then endof
\ ( avail len char ) add to buffer?
>r 2dup > r@ $7f and $1f > and if
 r@ over addr + c! 1+ r@ emit then r>
endcase again ;
hide addr
