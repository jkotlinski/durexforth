

:asm (do)
2 lda,x pha, 3 lda,x pha,
0 lda,x pha, 1 lda,x pha,
inx, inx, inx, inx, ;asm

: do ( limit first -- ) immed
' (do) , here ;

: loop immed
' r> , ' 1+ , ' r> , ' 2dup , ' < ,
[compile] while ' >r , ' >r ,
[compile] repeat ' 2drop , ;

: +loop immed
' r> , ' + , ' r> , ' 2dup , ' < ,
[compile] while ' >r , ' >r ,
[compile] repeat ' 2drop , ;

: i immed ' r@ , ;
:asm j txa, 5 sbx,#
pla, tay, pla, 0 sta,x
pla, 1 sta,x pla, 2 sta,x
pla, 4 sta,x pla, 3 sta,x pha,
4 lda,x pha, 2 lda,x pha,
1 lda,x pha, 0 lda,x pha,
tya, pha, inx, inx, inx, ;asm

( : test
sp@
2 0 do 2 0 do loop loop
sp@ 2+ = if exit then
begin 1 d020 +! again ; test )
