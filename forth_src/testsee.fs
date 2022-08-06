marker ---testsee---

include see

: verify page see refill
source tuck type cr 0 do
$400 i + c@ $428 i + c@ <>
abort" ko" loop ." ok"
; immediate

: x ; : y ;
: test 0 if x then y ;
verify test
: test 0 if x then y ;

---testsee---
