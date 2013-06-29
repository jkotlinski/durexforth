

: do ( limit first -- ) immed
' swap , [compile] begin ' >r , ' >r , ;

: loop immed
' r> , ' 1+ , ' r> , ' 2dup , ' = ,
[compile] until ' 2drop , ;

: +loop immed
' r> , ' + , ' r> , ' 2dup , ' >= ,
[compile] until ' 2drop , ;

: i immed ' r@ , ;

(
: test 10 0 do i . loop ;

test
)
