

:asm (do)
2 lda,x pha, 3 lda,x pha,
0 lda,x pha, 1 lda,x pha,
inx, inx, inx, inx, ;asm

: do ( limit first -- ) immed
' (do) , [compile] begin ;

: loop immed
' r> , ' 1+ , ' r> , ' 2dup , ' < ,
[compile] while ' >r , ' >r ,
[compile] repeat ' 2drop , ;

: +loop immed
' r> , ' + , ' r> , ' 2dup , ' < ,
[compile] while ' >r , ' >r ,
[compile] repeat ' 2drop , ;

: i immed ' r@ , ;
: j immed ' r> , ' r> , ' r@ ,
' -rot , ' >r , ' >r , ;

( : test
sp@
2 0 do 2 0 do loop loop
sp@ 2+ = if exit then
begin 1 d020 +! again ; test )
