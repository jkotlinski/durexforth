: fakekeys ( n -- ) $c6 c! ;

marker ---test---

.( gfxdemo )
$b fakekeys \ skips demos
parse-name gfx included
parse-name gfxdemo included

.( fractals )
4 fakekeys \ skips demos
parse-name fractals included
demo

.( mmldemo )
parse-name mmldemo included

.( siddemo )
parse-name sid included sid-demo

.( spritedemo )
1 fakekeys \ exits demo
parse-name spritedemo included

.( see )
parse-name testsee included

.( include )
:noname s" include 1 2" evaluate
2 <> abort" not 2"
1 <> abort" not 1" ; execute

---test---

: x depth abort" depth" ; x

parse-name compat included
parse-name tester included
parse-name testcore included
parse-name testcoreplus included
parse-name testcoreext included
parse-name testexception included

\ -----

( Finally: Using v F7 compile & run,
write an "ok" dummy file to indicate
that tests passed, then exit Vice. )

: push ( ch -- )
$c6 c@ $277 + c!
1 $c6 +! ;

: x
0 1 s" ok" saveb
0 $d7ff c! ; \ exit vice

.( v )
\ The FIFO is only 10 bytes.
\ Don't add more.
'i' push 'x' push
$5f push \ leftarrow
$88 push \ f7
v
