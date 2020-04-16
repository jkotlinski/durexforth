\ submitted by kevin reno

: ls ( addr -- )
begin ?dup while        \ abort if addr = 0
2+ dup @ . 2+           \ skip line link, print blocks
begin dup c@ ?dup while \ eol?
emit 1+ repeat 1+ cr    \ print line, eol
dup c@ 0= if c@ then    \ if eof then addr = 0
repeat ;                

\ sample code
\ $c000 try $0:*=s
\ : try ( addr -- ) 
\ dup parse-name rot loadb drop ls ;























