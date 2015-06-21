:asm (do)
sp0 1+ lda,x pha, sp1 1+ lda,x pha,
sp0 lda,x pha, sp1 lda,x pha,
inx, inx, ;asm

: do ( limit first -- ) immed
['] (do) , here ;

:asm (loop)
zptmp stx, tsx, # x = stack pointer
102 inc,x 3 bne, 101 inc,x # i++
102 lda,x 104 cmp,x 1 @@ beq, # lsb
2 @:
# not done, branch back
zptmp ldx,
loc branch >cfa jmp,
1 @:
101 lda,x 103 cmp,x 2 @@ bne, # msb
# loop done
inx, inx, inx, inx, txs,
zptmp ldx,
# skip branch addr
ip inc, 2 bne, ip 1+ inc,
ip inc, 2 bne, ip 1+ inc,
;asm

: loop immed
['] (loop) , , ; # store branch address

: +loop immed
['] r> , ['] + , ['] r> , ['] 2dup , ['] < ,
[compile] while ['] >r , ['] >r ,
[compile] repeat ['] 2drop , ;

: i immed ['] r@ , ;
:asm j txa, tsx,
106 ldy,x zptmp sty, 105 ldy,x
tax, dex, 
sp1 sty,x zptmp lda, sp0 sta,x ;asm
