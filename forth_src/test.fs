depth 0= assert

( loop tests )
: x 10 0 do i .
i 5 = if 0 leave then loop ;
x 0= assert
: x 10 0 do i .
i 5 = if 0 unloop exit then loop ;
x 0= assert

depth 0= assert

( sample tests )
s" gfxdemo" load
xxxxxxxxxx \ skips demos
depth 0= assert

s" fractals" load
xxxx \ skips demos
depth 0= assert

s" mmldemo" load
depth 0= assert

s" spritedemo" load
x \ exits demo 
depth 0= assert

.( ok)
