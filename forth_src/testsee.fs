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
: test if then ;
verify test
: test if then ;

\ begin .. again
: test begin again ;
verify test
: test begin again ;

page .( see ok) cr

---testsee---
