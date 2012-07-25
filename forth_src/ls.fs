

# submitted by kevin reno
: load$
s" $" here @ loadb drop
ae @ 2 - here @ ;

: blocks ar ! xr ! bdcd jsr-wrap
space ;

: getreg dup c@ swap 1+ c@ ;

: tobl 2 + dup getreg blocks 2 + ;
: to0 begin dup c@ dup if
emit 1+ else drop 1+ cr exit
then again ;

: ls load$ begin
2dup <> if
       tobl
       to0
       else 2drop exit
       then again ;
