

:asm hires 
# bmp 8000-9fff
bb lda,# d011 sta,
15 lda,# dd00 sta,
0 lda,# d018 sta,
;asm

:asm nohires
# revert
9b lda,# d011 sta,
17 lda,# dd00 sta,
17 lda,# d018 sta,
;asm
