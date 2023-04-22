( three block buffers at $c000-$cbff )

\ buffer block id's
create bbi 0 , 0 c,
create dirty 0 , 0 c,

\ last-used timestamps
variable time
create lu 0 , 0 , 0 ,

create path 'b' c, 0 ,

\ updates last-used timestamp
: touch ( buf -- buf )
1 time +! time @ over 2* lu + ! ;

: >addr $400 * $c000 + ;

: >path ( blk -- ) #10 /mod
'0' + path 1+ c! '0' + path 2+ c! ;

: save-buf ( buf -- )
dup dirty + c@ 0= if drop exit then
0 over dirty + c!
dup bbi + c@ >path >addr dup $400 +
path 3 saveb ;

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
lu @ lu 2+ @ < lu @ lu 4 + @ < and
if 0 else lu 2+ @ lu 4 + @ < if
1 else 2 then then dup save-buf
bbi + 0 swap c! ;

: block ( blk -- addr )
already-loaded dup 0< if exit then
load-to-unused dup 0< if exit then
drop-lru load-to-unused ;

: buffer ( blk -- addr )
already-loaded dup 0< if exit then
pick-unused dup 0< if exit then
drop-lru pick-unused ;

: list ( blk -- )
block dup $400 + swap do
i c@ emit loop ;

: empty-buffers bbi 3 erase ;

: update ( -- )
3 0 do time @ lu i 2* + @ = if
1 dirty i + c! then loop ;

: save-buffers ( -- )
0 save-buf 1 save-buf 2 save-buf ;

: flush save-buffers empty-buffers ;
