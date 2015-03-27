

# submitted by kevin reno
: load$
s" $" here loadb drop
ae @ 2 - here ;

: tobl 2+ dup @ . space 2+ ;
: to0 begin
dup c@ dup if
emit 1+ else drop 1+ cr exit
then again ;

: ls base decimal
load$ begin
2dup <> while tobl to0 repeat
2drop to base ;
