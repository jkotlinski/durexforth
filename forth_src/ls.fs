\ submitted by kevin reno

: ls ( addr -- )
begin ?dup while
2+ dup @ . 2+
begin dup c@ ?dup while
emit 1+ repeat 1+ cr
dup c@ 0= if drop 0 then
more repeat ;

\ sample code
\ $c000 try $0:*=s
\ : try ( addr -- ) 
\ dup parse-name rot loadb drop ls ;























