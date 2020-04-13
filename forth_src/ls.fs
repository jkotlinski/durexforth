\ submitted by kevin reno

: ls
$f word count dup 0<> if
tuck here 3 + swap move
3 + here
'$' over c!
'0' over 1+ c!
':' over 2+ c!
tuck
else 2drop s" $" here then
loadb 2 - here
begin 2dup <> while
2+ dup @ . space 2+
begin dup c@ ?dup while
emit 1+ repeat 1+ cr
more repeat 2drop ;









