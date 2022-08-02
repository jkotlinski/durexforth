( decompile forth word and print to
screen. try "see see". watch out:
hidden words are not supported! )

: scan ( nt -- )
drop ;

: print ( nt -- )
':' emit space
dup name>string type space
c@ $80 and if ." immediate " then
';' emit cr
;

: see
parse-name 2dup find-name \ c-addr u nt
?dup 0= if notfound then nip nip \ nt
dup scan print ;

: test ;
: test2 ; immediate

see test
see test2
see test3

(
dup xt> dup
name>string type space

scan
print ;

begin
 2dup >
while
 dup c@ case
 $20 of see-jsr endof
 $4c of see-jmp endof
 $e8 of 1+ ." drop " endof \ inx
 $60 of 1+ ." exit " endof \ rts
 ." ? " swap 1+ swap
 endcase
repeat
';' emit cr 2drop ;
)
