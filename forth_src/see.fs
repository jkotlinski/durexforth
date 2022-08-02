( decompiles colon definitions
to screen. try "see see". )

( points to a list with format
 src, dst, code )
variable branchptr

\ codes (branch types)
1 constant RepeatCode
2 constant AgainCode
3 constant UntilCode
4 constant ElseCode
5 constant WhileCode
6 constant LeaveCode

: ,branch ( val -- )
branchptr @ ! 2 branchptr +! ;
: branch! ( src dst -- src )
over ,branch ,branch 0 ,branch ;
: type! ( u -- )
branchptr @ 2 - ! ;

variable nt
:noname ( xt nt -- xt 1|0 )
2dup dup c@ 1f and 1+ + @ = if
nt ! 0 else drop 1 then ;
: xt>nt ( xt -- nt | 0 )
0 nt ! literal dowords drop nt @ ;

: scan-jsr ( addr -- addr+3 )
1+ dup @
case
['] 0branch of 2+ endof
endcase 2+ ;

: scan-jmp ( addr -- addr+3 )
3 + ;

: scan ( nt -- )
here branchptr !
>xt begin
dup c@ case
$20 of scan-jsr endof
$4c of scan-jmp endof
$e8 of 1+ endof \ inx
$60 of drop exit endof \ rts
endcase
again ;

: print-jsr ( addr -- addr+3 )
1+ dup @
case
['] 0branch of 2+ endof
dup xt>nt count 1f and type space
endcase 2+ ;

: print-jmp ( addr -- addr+3 )
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
