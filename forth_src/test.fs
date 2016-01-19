: fakekeys ( n -- )
c6 c! ;

here latest @

.( gfxdemo )
a fakekeys \ skips demos
s" gfxdemo" included

.( fractals )
4 fakekeys \ skips demos
s" fractals" included

.( mmldemo )
s" mmldemo" included

.( spritedemo )
1 fakekeys \ exits demo 
s" spritedemo" included

latest ! to here
depth 0= assert

s" tester" included
s" testcore" included
s" testcoreplus" included

hex
: push ( ch -- )
c6 c@ 277 + c!
1 c6 +! ;

.( vi )
\ The FIFO is only 10 bytes.
\ Don't add more.
'i' push d push
'.' push '(' push bl push
'O' push 'K' push ')' push
5f push \ leftarrow
88 push \ f7
vi
