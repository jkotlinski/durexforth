

:asm dodoes
# r> zptmp !
pla, zptmp sta, pla, zptmp 1+ sta, tay,

# Pushes data pointer to param stack.
dex, dex, clc, zptmp lda, 3 adc,#
0 sta,x 1 bcc, iny, 1 sty,x

# Is behavior pointer null?
2 ldy,# zptmp lda,(y)
+branch beq, # Yes: done.

# No: behavior pointer => IP
ip lda, pha, ip 1+ lda, pha,
zptmp lda,(y) ip 1+ sta, dey,
zptmp lda,(y) ip sta,
:+ ;asm
    
: create # 0 = behavior pointer
header 20 c, ['] dodoes , 0 , ;
: does> r> latest @ >dfa ! ;
