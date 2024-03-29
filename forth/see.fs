( decompiles colon definitions
to screen. try "see see". )

marker ---see---

header see latest

( points to a list with format
 src, dst, code )
variable branchptr

variable my-xt

\ branch types
0 constant #if
1 constant #else
2 constant #while
3 constant #leave
4 constant #repeat
5 constant #again
6 constant #until

: ,branch ( val -- )
branchptr @ ! 2 branchptr +! ;
: branch! ( src dst -- src )
over ,branch ,branch
0 branchptr @ c! 1 branchptr +! ;
: type! ( u -- )
branchptr @ 1 - c! ;

: reached-end ( addr -- addr flag )
1 branchptr @ here ?do
over i 2+ @ u< if drop 0 leave then
5 +loop ;

:noname ( 0 xt nt -- nt? xt flag )
2dup >xt = if swap rot then ;
: xt>nt ( xt -- nt | 0 )
0 swap literal dowords drop ;

: 3+ 3 + ; : 4+ 4 + ; : 5+ 5 + ;

: scan-0branch ( addr -- addr+5 )
dup 3+ @ 2dup branch! \ src dst src
u> 0= if \ back
#until type! then 5+ ;

: skip-lits ( addr -- addr )
3+ dup c@ + 1+ ;

: scan-loop ( addr -- addr+5 )
\ correct #else to #leave
5+ branchptr @ here ?do
dup i 2+ @ = if #leave i 4+ c! then
5 +loop ;

: scan-jsr ( addr -- addr )
dup 1+ @ case
['] litc of 4+ endof
['] lit of 5+ endof
['] lits of skip-lits endof
['] (?do) of 5+ endof
['] (loop) of scan-loop endof
['] (+loop) of scan-loop endof
['] (of) of 5+ endof
['] 0branch of scan-0branch endof
drop 3+ dup endcase ;

\ it's a while-repeat if...
\ 1) it's a backjump (repeat)
\ 2) followed by dst of a fwd 0branch
\    (while target)
: while? ( jmpaddr -- jmpaddr flag )
\ backjump?
dup dup 1+ @ u> 0= if 0 exit then
\ 0branch fwd dst?
0 branchptr @ here ?do
over 3+ i 2+ @ = if \ dst?
#while i 4+ c!
drop 1 leave then 5 +loop ;

: scan-jmp ( addr -- addr )
dup 1+ @ dup my-xt @ u< if drop else
2dup branch! u> if #else else
while? if #repeat else #again then
then type! then ;

: scan ( nt -- )
here branchptr !
>xt dup my-xt ! begin dup c@ case
$20 of scan-jsr endof
$4c of scan-jmp reached-end if
drop exit then 3+ endof
$e8 of 1+ endof \ inx
$60 of reached-end if \ rts
drop exit else 1+ then endof
abort endcase again ;

: print-xt ( xt -- )
dup xt>nt ?dup if name>string type drop
else u. ." execute" then space ;

: print-0branch ( addr -- addr+5 )
branchptr @ here do
i @ over = if i 4+ c@ case
#if of ." if " endof
#while of ." while " endof
#until of ." until " endof
endcase leave then
5 +loop 5+ ;

: print-lits ( addr -- addr )
's' emit '"' emit space
4+ dup 1- c@ begin ?dup while
over c@ emit 1 /string repeat
'"' emit space ;

: print-unloop ( addr -- addr+3 )
\ if followed by a leave, skip
3+ branchptr @ here do
dup i @ = i 4+ c@ #leave = and if
unloop exit then 5 +loop ." unloop " ;

: print-of ( addr -- addr+5 )
5+ ." over = if drop " ;

: print-jsr ( addr -- addr )
dup 1 + @ case
['] lit of 3+ dup @ . 2+ endof
['] litc of 3+ dup c@ . 1+ endof
['] lits of print-lits endof
['] (do) of 3+ ." do " endof
['] (?do) of 5+ ." ?do " endof
['] (loop) of 5+ ." loop " endof
['] (+loop) of 5+ ." +loop " endof
['] (of) of print-of endof
['] 0branch of print-0branch endof
['] unloop of print-unloop endof
print-xt 3+ dup
endcase ;

: remove-then ( addr -- )
branchptr @ here do i 2+ @ over =
i 4+ c@ #if = and if
0 i 2+ ! then 5 +loop drop ;

: print-jmp ( addr -- addr )
dup 1+ @ dup my-xt @ u< if print-xt
else drop branchptr @ here ?do
i @ over = if i 4+ c@ case
#else of ." else "
dup 3+ remove-then endof
#leave of ." leave " endof
#repeat of ." repeat " endof
#again of ." again " endof
abort endcase then 5 +loop then ;

: .then ." then " ;
: .begin ." begin " ;

: print-to-branch ( addr -- addr )
branchptr @ here ?do
dup i 2+ @ = if
i 4+ c@ case
#if of .then endof
#else of .then endof
#while of endof
#leave of endof
#repeat of .begin endof
#again of .begin endof
#until of .begin endof
abort endcase then 5 +loop ;

: print ( nt -- )
':' emit space
dup name>string type space
dup c@ $80 and if ." immediate " then
>xt begin
print-to-branch dup c@ case
$20 of print-jsr endof
$4c of print-jmp reached-end if
drop ';' emit cr exit then 3+ endof
$e8 of ." drop " 1+ endof \ inx
$60 of \ rts
reached-end if
drop ';' emit cr exit else
." exit " 1+ then endof
endcase again ;

define see
parse-name 2dup find-name \ c-addr u nt
?dup 0= if notfound then nip nip \ nt
dup scan print ;

to latest
