

: do ( limit first -- ) immed
[compile] begin ' >r , ' >r , ;

: loop immed
' r> , ' r> ,
' 1+ , ' 2dup , ' = ,
[compile] until ' 2drop , ;

: i immed ' r> , ' r> , ' dup ,
' >r , ' swap , ' >r , ;

: test
10 0 do i . loop ;

cr see do
cr see loop
cr see test
test
