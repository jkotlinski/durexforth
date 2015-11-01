code (do)
pla, zptmp sta,
pla, tay,

sp1 1+ lda,x pha, sp0 1+ lda,x pha,
sp1 lda,x pha, sp0 lda,x pha,
inx, inx, 

tya, pha,
zptmp lda, pha,
;code

: do ( limit first -- ) immediate
postpone (do) here ;

code (loop)
zptmp stx, tsx, \ x = stack pointer
103 inc,x 3 bne, 104 inc,x \ i++
103 lda,x 105 cmp,x 1 @@ beq, \ lsb
2 @:
\ not done, branch back
zptmp ldx, \ restore x
loc branch dup assert >cfa jmp,
1 @:
104 lda,x 106 cmp,x 2 @@ bne, \ msb
\ loop done
\ skip branch addr
pla, clc, 3 adc,# zptmp2 sta,
pla, 0 adc,# zptmp2 1+ sta,
txa, clc, 6 adc,# tax, txs, \ sp += 6
zptmp ldx, \ restore x
zptmp2 (jmp),

: loop immediate no-tce
postpone (loop) , ; \ store branch address

variable old
variable new
variable limit
: (+loop) ( inc -- )
new !
r>
r> dup old ! new +!
r> limit !
old @ limit @ s< new @ limit @ s>= and
new @ limit @ s< old @ limit @ s>= and 
or if 2+ >r exit then
limit @ >r new @ >r
>r [ ' branch jmp, ] ;
hide old
hide new
hide limit

: +loop immediate no-tce
postpone (+loop) , ;
hide (+loop)

: i immediate postpone r@ ;
code j txa, tsx,
107 ldy,x zptmp sty, 108 ldy,x
tax, dex, 
sp1 sty,x zptmp lda, sp0 sta,x ;code
