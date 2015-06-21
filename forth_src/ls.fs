# submitted by kevin reno

: ls base decimal
s" $" here loadb drop
ae @ 2 - here 
begin 2dup <> while 
2+ dup @ . space 2+
begin dup c@ ?dup while
emit 1+ repeat 1+ cr repeat
2drop to base ;
