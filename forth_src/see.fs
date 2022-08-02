( decompile forth word and print to
screen. try "see see". watch out:
hidden words are not supported! )

: scan-jsr
3 + ;
: scan-jmp
3 + ;

: scan ( nt -- )
>xt begin
dup c@ case
$20 of scan-jsr endof
$4c of scan-jmp endof
$e8 of 1+ endof \ inx
$60 of drop exit endof \ rts
endcase
again ;

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

: test begin 1 again ;

see test

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
