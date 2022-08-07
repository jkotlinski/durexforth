( decompiles colon definitions
to screen. try "see see". )

( points to a list with format
 src, dst, code )
variable branchptr

variable my-xt

\ branch types
1 constant #else
2 constant #while
3 constant #leave
\ codes > 10 match begin
11 constant #repeat
12 constant #again
13 constant #until

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

:noname ( 0 xt nt -- nt? xt flag )
2dup dup c@ 1f and 1+ + @ = if
swap rot then ;
: xt>nt ( xt -- nt | 0 )
0 swap literal dowords drop ;

: scan-0branch ( addr -- addr+5 )
dup 3 + @ 2dup branch! \ src dst src
u> 0= if \ back
#until type! then 5 + ;

: skip-lits ( addr -- addr )
3 + dup @ + 2+ ;

: scan-jsr ( addr -- addr )
dup 1+ @ case
['] litc of 4 + endof
['] lit of 5 + endof
['] lits of skip-lits endof
['] 0branch of scan-0branch endof
drop 3 + dup endcase ;

: scan-jmp ( addr -- addr+3 )
dup 1 + @ dup my-xt @ < if drop else
branch! #again type! then 3 + ;

: scan ( nt -- )
here branchptr !
>xt begin dup c@ case
$20 of scan-jsr endof
$4c of scan-jmp reached-end if
drop exit then endof
$e8 of 1+ endof \ inx
$60 of \ rts
reached-end if
drop exit then endof
endcase again ;

: print-xt ( xt -- )
dup my-xt @ < if
xt>nt count 1f and type space
else ." again " drop then ;

: print-0branch ( addr -- addr+5 )
\ todo while, until
branchptr @ here do
i @ over = if
i 4 + @ 10 > if ." until "
else ." if " then
unloop 5 + exit then
6 +loop abort ;

: print-lits ( addr -- addr )
's' emit '"' emit space
5 + dup 2 - @ begin ?dup while
over c@ emit 1 /string repeat
'"' emit space ;

: print-jsr ( addr -- addr )
dup 1 + @
case
['] lit of 3 + dup @ . 2+ endof
['] litc of 3 + dup c@ . 1+ endof
['] lits of print-lits endof
['] 0branch of print-0branch endof
print-xt 3 + dup
endcase ;

: print-jmp ( addr -- addr+3 )
1+ dup @ print-xt 2 + ;

: print-to-branch ( addr -- addr )
\ todo begin
branchptr @ here ?do
dup i 2 + @ = if
i 4 + @ 10 > if ." begin " else
." then " then then 6 +loop ;

: print ( nt -- )
':' emit space
dup name>string type space
dup c@ $80 and if ." immediate " then
>xt dup my-xt ! begin
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
