

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

(
: test
2 0 do 3 0 do
i . j . cr
loop loop ;

test
)
