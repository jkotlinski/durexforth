( decompile forth word and print to
screen. try "see see". watch out:
hidden words are not supported! )

( addr + target type )
0 value targets

1 constant br-then
2 constant br-begin

: branch, ( dst type -- )
swap targets !
targets 2+ to targets
targets c!
targets 1+ to targets ;

: scan-jsr
1+ dup @
case
['] 0branch of 2+ endof
endcase 2+ ;

: scan-jmp ( xt -- xt+3 )
dup 1+ @
2dup < if \ back-branch
br-begin branch,
else \ fwd-branch
br-then branch,
then
3 + ;

: scan ( nt -- )
here to targets
>xt begin
dup c@ case
$20 of scan-jsr endof
$4c of scan-jmp endof
$e8 of 1+ endof \ inx
$60 of drop exit endof \ rts
endcase
again ;

: print-jsr
1+ dup @
case
['] 0branch of 2+ endof
endcase 2+ ;
: print-jmp
3 + ;

: print ( nt -- )
':' emit space
dup name>string type space
dup c@ $80 and if ." immediate " then
>xt begin
dup c@ case
$20 of print-jsr endof
$4c of print-jmp endof
$e8 of ." drop " 1+ endof \ inx
$60 of \ rts
." exit " drop ';' emit cr exit
endof
endcase
again ;

: see
parse-name 2dup find-name \ c-addr u nt
?dup 0= if notfound then nip nip \ nt
dup scan print ;

: test if 1 then ;

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
