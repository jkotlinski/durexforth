marker ---testsee---

include see

: gives page see
4 $d6 c@ do cr loop \ move to row 4
refill source tuck
type 0 do $400 i + c@ $4a0 i + c@
<> abort" ko" loop ; immediate

: x ; immediate gives x
: x immediate ;

: x , . ; gives x
: x , . ;

: x 12 ; gives x
: x 12 ;

: x 1234 ; gives x
: x 1234 ;

: x drop ; gives x
: x drop ;

: x s" hai" ; gives x
: x s" hai" ;

: x if then ; gives x
: x if then ;

: x if else then ; gives x
: x if else then ;

: x if else 1 then ; gives x
: x if else 1 then ;

: x if exit then ; gives x
: x if exit then ;

: x begin again ; gives x
: x begin again ;

: x begin until ; gives x
: x begin until ;

: x begin 1 while 2 repeat ; gives x
: x begin 1 while 2 repeat ;

: x do loop ; gives x
: x do loop ;

: x do +loop ; gives x
: x do +loop ;

: x ?do loop ; gives x
: x ?do loop ;

: x do leave loop ; gives x
: x do leave loop ;

: x case 1 of 2 endof 3 of 4 endof 5 endcase ; gives x
: x 1 over = if drop 2 else 3 over = if drop 4 else 5 drop then then ;

page .( see ok) cr

---testsee---
