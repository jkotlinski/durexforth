

:asm dodoes
# r> zptmp !
pla, zptmp sta, pla, zptmp 1+ sta, tay,

# push data pointer to param stack
dex, dex,
clc, zptmp lda, 3 adc,# 0 sta,x
1 bcc, iny, 1 sty,x

# behavior pointer => IP
ip lda, pha, ip 1+ lda, pha,
2 ldy,# zptmp lda,(y) ip 1+ sta,
dey,    zptmp lda,(y) ip sta,
;asm

here @ [compile] exit
: create
# default behavior = exit
header 20 c, ['] dodoes , literal , ;

: does> r> latest @ >dfa ! ;
