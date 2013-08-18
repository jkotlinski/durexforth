

:asm (do)
2 lda,x pha, 3 lda,x pha,
0 lda,x pha, 1 lda,x pha,
inx, inx, inx, inx, ;asm

: do ( limit first -- ) immed
' (do) , here ;

:asm (loop)
txa, tay, tsx, # x = stack pointer
102 inc,x 3 bne, 101 inc,x # i++
102 lda,x 104 cmp,x 1 @@ bne, # lsb
101 lda,x 103 cmp,x 2 @@ beq, # msb
1 @:
# not done, branch back
tya, tax,
loc branch >cfa jmp,
2 @:
# loop done
inx, inx, inx, inx, txs,
tya, tax,
# skip branch addr
ip inc, 2 bne, ip 1+ inc,
ip inc, 2 bne, ip 1+ inc,
;asm

: loop immed
' (loop) , , ; # store branch address

: +loop immed
' r> , ' + , ' r> , ' 2dup , ' < ,
[compile] while ' >r , ' >r ,
[compile] repeat ' 2drop , ;

: i immed ' r@ , ;
:asm j txa, tsx,
106 ldy,x zptmp sty, 105 ldy,x
tax, dex, dex,
1 sty,x zptmp lda, 0 sta,x ;asm

hide (do)

( : test
sp@
2 0 do 2 0 do loop loop
sp@ 2+ = if exit then
begin 1 d020 +! again ; test )
