

: do ( limit first -- ) immed
' swap , [compile] begin ' >r , ' >r , ;

: loop immed
' r> , ' 1+ , ' r> , ' 2dup , ' = ,
[compile] until ' 2drop , ;

: +loop immed
' r> , ' + , ' r> , ' 2dup , ' >= ,
[compile] until ' 2drop , ;

: i immed ' r@ , ;
: j immed ' r> , ' r> , ' r@ ,
' -rot , ' >r , ' >r , ;

( : test
sp@
2 0 do 2 0 do loop loop
sp@ 2+ = if exit then
begin 1 d020 +! again ; test )
