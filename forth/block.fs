( three block buffers at $c000-$cbff )

\ buffer block id's
create bbi 0 , 0 c,

\ last-used timestamps
variable time
create lu 0 , 0 , 0 ,

create path 'b' c, 0 ,

\ updates last-used timestamp
: touch ( buf -- buf )
time @ over 2* lu + ! 1 time +! ;

: >addr $400 * $c000 + ;

: >path ( buf -- ) 10 /mod
'0' + path 1+ c! '0' + path 2+ c! ;

: doload ( blk buf -- addr )
2dup bbi + c! >addr >r >path path 3
r@ loadb 0= if r@ $400 erase then r> ;

: already-loaded ( blk -- addr|blk )
3 0 do dup bbi i + c@ = if drop
i touch >addr unloop exit then loop ;

: load-to-unused ( blk -- addr|blk )
3 0 do bbi i + c@ 0= if
i touch doload unloop exit then loop ;

: pick-unused ( blk -- addr|blk )
3 0 do bbi i + c@ 0= if bbi i + c!
i touch >addr unloop exit then loop ;

: drop-lru ( -- )
0 lu @ lu 2+ @ < lu @ lu 4 + @ < and
if 0 else lu 2+ @ lu 4 + @ < if
1 else 2 then then bbi + c! ;

: block ( blk -- addr )
already-loaded dup 0< if exit then
load-to-unused dup 0< if exit then
drop-lru load-to-unused ;

: buffer ( blk -- addr )
already-loaded dup 0< if exit then
pick-unused dup 0< if exit then
drop-lru pick-unused ;
