\ submitted by kevin reno

: rdir ( addr -- )
begin ?dup while
more 2+ dup @ . 2+
begin dup c@ ?dup while 
emit 1+ repeat 1+ cr
dup c@ 0= if c@ then
repeat ;

: ls parse-name ?dup if
else drop s" $"
then here 34 + loadb if
page here 34 + rdir then ;
