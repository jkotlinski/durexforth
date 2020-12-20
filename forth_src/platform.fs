marker ---get-platform---

\ use IRQ vector to identify which
\ system we are running on
: get-platform 
$fffe @ case
$ff48 of 64 endof
$ff17 of 128 endof
abort" cannot identify platform"
endcase ;

get-platform
---get-platform---
constant platform
