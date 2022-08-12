: fakekeys ( n -- ) $c6 c! ;

marker ---test---

.( gfxdemo )
$b fakekeys \ skips demos
parse-name gfx included
parse-name gfxdemo included

.( fractals )
4 fakekeys \ skips demos
parse-name fractals included

.( mmldemo )
parse-name mmldemo included

.( siddemo )
parse-name sid included sid-demo

.( spritedemo )
1 fakekeys \ exits demo
parse-name spritedemo included

.( see )
parse-name testsee included

---test---

: x depth abort" depth" ; x

parse-name compat included
parse-name tester included
parse-name testcore included
parse-name testcoreplus included

: push ( ch -- )
$c6 c@ $277 + c!
1 $c6 +! ;

.( v )
\ The FIFO is only 10 bytes.
\ Don't add more.
'i' push $d push
'.' push '(' push bl push
'O' push 'K' push ')' push
$5f push \ leftarrow
$88 push \ f7
v
