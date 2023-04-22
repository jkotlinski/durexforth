( three block buffers at $c000-$cbff )

\ buffer block id's
create bbi 0 , 0 c,

\ last-used timestamps
variable time
create lu 0 , 0 , 0 ,

create path 'b' c, 0 ,

\ updates last-used timestamp
: touch ( buf -- buf )
time @ over 2* lu + !
1 time +! ;

: >addr ( buf -- addr )
$400 * $c000 + ;

: doload ( n buf -- addr )
2dup bbi + c! >addr >r 10 /mod
'0' + path 1+ c! '0' + path 2+ c!
path 3 r@ loadb 0= if r@ $400 erase
then r> ;

: already-loaded ( n -- addr|0 )
3 0 do dup bbi i + c@ = if
drop i touch >addr unloop exit
then loop 0 ;

: load-to-unused ( n -- addr|0 )
3 0 do bbi i + c@ 0= if
i touch doload unloop exit
then loop 0 ;

: drop-lru ( -- )
0 lu @ lu 2+ @ <
lu @ lu 4 + @ < and if 0
else lu 2+ @ lu 4 + @ < if
1 else 2 then then bbi + c! ;

: block ( n -- addr )
already-loaded ?dup if exit then
load-to-unused ?dup if exit then
drop-lru load-to-unused ;
