:asm (do)
pla, zptmp sta,
pla, tay,

sp0 1+ lda,x pha, sp1 1+ lda,x pha,
sp0 lda,x pha, sp1 lda,x pha,
inx, inx, 

tya, pha,
zptmp lda, pha,
;asm

: do ( limit first -- ) immed
['] (do) jsr, here ;

:asm (loop)
zptmp stx, tsx, # x = stack pointer
104 inc,x 3 bne, 103 inc,x # i++
104 lda,x 106 cmp,x 1 @@ beq, # lsb
2 @:
# not done, branch back
zptmp ldx,
loc branch >cfa jmp,
1 @:
103 lda,x 105 cmp,x 2 @@ bne, # msb
# loop done
here d020 inc, jmp,
inx, inx, inx, inx, txs,
zptmp ldx,
# skip branch addr
# ip inc, 2 bne, ip 1+ inc,
# ip inc, 2 bne, ip 1+ inc,
;asm

: loop immed
['] (loop) jsr, , ; # store branch address

: +loop immed
['] r> , ['] + , ['] r> , ['] 2dup , ['] < ,
[compile] while ['] >r , ['] >r ,
[compile] repeat ['] 2drop , ;

: i immed ['] r@ jsr, ;
:asm j txa, tsx,
106 ldy,x zptmp sty, 105 ldy,x
tax, dex, 
sp1 sty,x zptmp lda, sp0 sta,x ;asm

: x 10 0 do i . loop ;
x
