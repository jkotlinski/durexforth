marker ---testsee---

include see

: verify page see refill
source tuck type 0 do
$400 i + c@ $428 i + c@ <>
abort" ko" loop ; immediate

\ jsr+jmp
: test , . ;
verify test
: test , . ;

\ litc
: test 1234 ;
verify test
: test 1234 ;

\ lit
: test 1234 ;
verify test
: test 1234 ;

\ if .. then
: test 0 if 1 then 2 ;
verify test
: test 0 if 1 then 2 ;

\ begin .. again
: test 0 begin 1 again 2 ;
verify test
: test 0 begin 1 again 2 ;

page .( see ok) cr

---testsee---
