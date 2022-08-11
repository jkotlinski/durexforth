code (do) ( limit first -- )
pla, w sta,
pla, tay,

msb 1+ lda,x pha, lsb 1+ lda,x pha,
msb lda,x pha, lsb lda,x pha,
inx, inx,

tya, pha,
w lda, pha,
;code

\ leave stack
variable lstk $14 allot
variable lsp lstk lsp !
: >l ( n -- ) lsp @ ! 2 lsp +! ;

: do 0
postpone (do) here dup >l ; immediate

: ?do
postpone 2dup postpone = postpone if
postpone 2drop postpone branch
here swap 0 , postpone then
postpone (do) here dup >l ; immediate

: leave
postpone unloop
here 1+ >l 0 jmp, ; immediate

: resolve-leaves ( ?dopos dopos -- )
begin -2 lsp +!
dup lsp @ @ < while
here lsp @ @ ! repeat drop
\ ?do forward branch
?dup if here swap ! then ;

code (loop)
w stx, tsx, \ x = stack pointer
$103 inc,x 3 bne, $104 inc,x \ i++
$103 lda,x $105 cmp,x 1 @@ beq, \ lsb
2 @:
\ not done, branch back
w ldx, \ restore x
' branch jmp,
1 @:
$104 lda,x $106 cmp,x 2 @@ bne, \ msb
\ loop done
\ skip branch addr
pla, clc, 3 adc,# w2 sta,
pla, 0 adc,# w2 1+ sta,
txa, clc, 6 adc,# tax, txs, \ sp += 6
w ldx, \ restore x
w2 (jmp),

: loop
postpone (loop) dup , resolve-leaves ; immediate

: (+loop) ( inc -- )
r> swap r> 2dup +
rot 0< if tuck swap else tuck then
r@ 1- -rot within 0= if
>r >r [ ' branch jmp, ] then
r> 2drop 2+ >r ;

: +loop
postpone (+loop) dup , resolve-leaves ; immediate

: i postpone r@ ; immediate
code j txa, tsx,
$107 ldy,x w sty, $108 ldy,x
tax, dex,
msb sty,x w lda, lsb sta,x ;code

hide lstk
hide lsp
hide >l
hide resolve-leaves
