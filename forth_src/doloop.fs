:asm (do)
pla, zptmp sta,
pla, tay,

sp1 1+ lda,x pha, sp0 1+ lda,x pha,
sp1 lda,x pha, sp0 lda,x pha,
inx, inx, 

tya, pha,
zptmp lda, pha,
;asm

: do ( limit first -- ) immed
['] (do) compile, here ;

:asm (loop)
zptmp stx, tsx, # x = stack pointer
103 inc,x 3 bne, 104 inc,x # i++
104 lda,x 106 cmp,x 1 @@ beq, # lsb
2 @:
# not done, branch back
zptmp ldx,
loc branch >cfa jmp,
1 @:
103 lda,x 105 cmp,x 2 @@ bne, # msb
# loop done
zptmp ldx,
# skip branch addr
pla, clc, 2 adc,# zptmp sta,
pla, 0 adc,# zptmp 1+ sta,
pla, pla, pla, pla,
zptmp 1+ lda, pha,
zptmp lda, pha,
;asm

: loop immed
['] (loop) compile, , ; # store branch address

: +loop immed
['] r> compile, 
['] + compile, 
['] r> compile, 
['] 2dup compile, 
['] < compile,
[compile] while 
['] >r compile, 
['] >r compile,
[compile] repeat 
['] 2drop compile, ;

: i immed ['] r@ compile, ;
:asm j txa, tsx,
107 ldy,x zptmp sty, 108 ldy,x
tax, dex, 
sp1 sty,x zptmp lda, sp0 sta,x ;asm
