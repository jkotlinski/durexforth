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

: reached-end
branchptr @ here ?do
i 2+ @ here > if 0 unloop exit then
6 +loop 1 ;

variable nt
:noname ( xt nt -- xt 1|0 )
2dup dup c@ 1f and 1+ + @ = if
nt ! 0 else drop 1 then ;
: xt>nt ( xt -- nt | 0 )
0 nt ! literal dowords drop nt @ ;

: scan-jsr ( addr -- addr+3 )
dup 1+ @
case
['] 0branch of
dup 3 + @ 2dup branch! \ src dst src
u< if \ back
UntilCode type! then 5 + endof
drop 3 + dup
endcase ;

: scan-jmp ( addr -- addr+3 )
3 + ;

: scan ( nt -- )
here branchptr !
>xt begin
dup c@ case
$20 of scan-jsr endof
$4c of scan-jmp reached-end if
drop exit then endof
$e8 of 1+ endof \ inx
$60 of \ rts
reached-end if
drop exit then endof
endcase
again ;

: print-xt ( xt -- )
xt>nt count 1f and type space ;

: print-jsr ( addr -- addr+3 )
dup 1 + @
case
['] 0branch of
\ todo while, until
." if " 5 + endof
print-xt 3 + dup
endcase ;

: print-jmp ( addr -- addr+3 )
1+ dup @ print-xt 2 + ;

: print-to-branch ( addr -- addr )
\ todo begin
branchptr @ here ?do
dup i 2 + @ = if
." then " then 6 +loop ;

: print ( nt -- )
':' emit space
dup name>string type space
dup c@ $80 and if ." immediate " then
>xt begin
print-to-branch
dup c@ case
$20 of print-jsr endof
$4c of print-jmp reached-end if
drop ';' emit cr exit then endof
$e8 of ." drop " 1+ endof \ inx
$60 of \ rts
reached-end if
drop ';' emit cr exit else
." exit " then endof
endcase
again ;

: see
parse-name 2dup find-name \ c-addr u nt
?dup 0= if notfound then nip nip \ nt
dup scan print ;
