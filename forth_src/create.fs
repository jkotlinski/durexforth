

# create/does>

:asm dodoes
# r> zptmp !
pla, zptmp sta,
pla, zptmp 1+ sta, tay,

# Pushes data pointer to param stack.
dex, dex, clc, zptmp lda, 3 adc,#
0 sta,x 1 bcc, iny, 1 sty,x

# Is behavior pointer null?
zptmp inc, 2 bne, zptmp 1+ inc,
0 ldy,# zptmp lda,(y)
iny,    zptmp ora,(y)
+branch beq, # Yes: done.

# No: behavior pointer => IP
ip lda, pha, ip 1+ lda, pha,
zptmp lda,(y) ip 1+ sta, dey,
zptmp lda,(y) ip sta,

:+ ;asm
    
: create
header 20 c, ['] dodoes , 0 , ;
: does> r> latest @ >dfa ! ;
