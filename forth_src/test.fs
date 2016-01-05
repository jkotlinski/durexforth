: fakekeys ( n -- )
c6 c! ;

here latest @

.( gfxdemo )
a fakekeys \ skips demos
s" gfxdemo" load

.( fractals )
4 fakekeys \ skips demos
s" fractals" load

.( mmldemo )
s" mmldemo" load

.( spritedemo )
1 fakekeys \ exits demo 
s" spritedemo" load

latest ! to here
depth 0= assert

s" tester" load
s" testcore" load
s" testcoreplus" load

hex
: push ( ch -- )
c6 c@ 277 + c!
1 c6 +! ;

.( vi )
'i' push 
'.' push '(' push bl push
'o' push 'k' push ')' push
5f push \ leftarrow
88 push \ f7
vi
