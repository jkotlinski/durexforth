( three block buffers at $c000-$cbff )

create bbi 0 , 0 c, \ buffer block id's
create dirty 0 , 0 c,
create curr-buf 0 c,
create path 'b' c, 0 ,

: >addr ( buf -- addr )
$400 * $c000 + ;

: >path ( blk -- ) #10 /mod
'0' + path 1+ c! '0' + path 2+ c! ;

: save-buf ( buf -- )
dup dirty + c@ 0= if drop exit then
0 over dirty + c!
dup bbi + c@ >path >addr dup $400 +
path 3 saveb ;

: >buf ( blk -- buf ) 3 mod ;

: load-blk ( blk -- )
dup >path >buf >addr >r path 3
r@ loadb 0= if r@ $400 erase then
r> drop ;

: set-blk ( blk -- addr )
dup >buf curr-buf c!
dup dup >buf bbi + c! >buf >addr ;

: unassign ( blk -- blk )
dup >buf dup save-buf bbi + 0 swap c! ;

: loaded? ( blk -- blk flag )
dup dup >buf bbi + c@ = ;

: block ( blk -- addr )
loaded? if >buf >addr else
unassign dup load-blk set-blk then ;

: buffer ( blk -- addr )
loaded? if >buf >addr else
unassign set-blk then ;

: list ( blk -- )
block dup $400 + swap do
i c@ emit loop ;

: empty-buffers bbi 3 erase ;

: update ( -- )
1 dirty curr-buf c@ + c! ;

: save-buffers ( -- )
0 save-buf 1 save-buf 2 save-buf ;

: flush save-buffers empty-buffers ;
