: fakekeys ( n -- )
c6 c! ;

.( gfxdemo )
a fakekeys \ skips demos
s" gfxdemo" load
depth 0= assert

.( fractals )
4 fakekeys \ skips demos
s" fractals" load
depth 0= assert

.( mmldemo )
s" mmldemo" load
depth 0= assert

.( spritedemo )
1 fakekeys \ exits demo 
s" spritedemo" load
depth 0= assert

s" tester" load
s" core" load
s" coreplus" load

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
