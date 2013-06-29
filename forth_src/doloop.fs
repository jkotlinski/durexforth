

: do ( limit first -- ) immed
' swap , [compile] begin ' >r , ' >r , ;

: loop immed
' r> , ' 1+ , ' r> , ' 2dup , ' = ,
[compile] until ' 2drop , ;

: i immed [compile] r@ ;

(
: test 10 0 do i . loop ;

cr see do
cr see loop
cr see test
test
)
