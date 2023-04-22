( three block buffers at $c000-$cbff )

\ buffer block id's
create bbi 0 , 0 c,

create path 'b' c, 0 ,

: >addr ( slot -- addr )
$400 * $c000 + ;

: doload ( n slot -- addr )
2dup bbi + c! >addr >r 10 /mod
'0' + path 1+ c!
'0' + path 2+ c!
path 3 r@ loadb drop r> ;

: block ( n -- addr )
\ if already loaded, return buffer addr
3 0 do dup bbi i + c@ = if
drop i >addr unloop exit
then loop

\ load to an unused buffer, if possible
3 0 do bbi i + c@ 0= if
i doload unloop exit
then loop

\ discard a buffer and load it there
;
