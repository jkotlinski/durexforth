marker ---testsee---

include see

: gives page see refill source tuck
type 0 do $400 i + c@ $428 i + c@
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

: x if exit then ; gives x
: x if exit then ;

: x begin again ; gives x
: x begin again ;

: x begin until ; gives x
: x begin until ;

: x do loop ; gives x
: x do loop ;

page .( see ok) cr

---testsee---
