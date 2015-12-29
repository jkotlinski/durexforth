depth 0= assert

( loop tests )
: x 10 0 do i .
i 5 = if 0 leave then loop ;
x 0= assert
: x 10 0 do i .
i 5 = if 0 unloop exit then loop ;
x 0= assert

: fakekeys ( n -- )
c6 c! ;

depth 0= assert

( sample tests )
a fakekeys \ skips demos
s" gfxdemo" load
depth 0= assert

4 fakekeys \ skips demos
s" fractals" load
depth 0= assert

s" mmldemo" load
depth 0= assert

1 fakekeys \ exits demo 
s" spritedemo" load
depth 0= assert

.( ok)
