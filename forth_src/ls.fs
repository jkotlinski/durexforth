

# submitted by kevin reno

: to0 begin
dup c@ dup if
emit 1+ else drop 1+ cr exit
then again ;

: ls base decimal
s" $" here loadb drop
ae @ 2 - here 
begin
2dup <> while 
2+ dup @ . space 2+
to0 repeat
2drop to base ;
