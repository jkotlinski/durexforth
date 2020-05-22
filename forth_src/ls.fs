\ submitted by kevin reno

: rdir ( addr -- )
begin ?dup while        \ abort if addr = 0
more 2+ dup @ . 2+      \ skip line link, print blocks
begin dup c@ ?dup while \ eol?
emit 1+ repeat 1+ cr    \ print line, eol
dup c@ 0= if c@ then    \ if eof then addr = 0
repeat ;                

: ls parse-name ?dup if
else drop s" $"
then here loadb if
here rdir then ;
