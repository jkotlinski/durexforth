( three block buffers at $c000-$cbff )

create bbi 0 , 0 c, \ buffer block id's
create dirty 0 , 0 c,
create curr-buf 0 c,

: >addr ( buf -- addr )
$400 * $c000 + ;

: >path ( blk -- )
'b' here c! #10 /mod #10 /mod
4 1 do '0' + here i + c! loop ;

: save-buf ( buf -- )
dup dirty + c@ 0= if drop exit then
0 over dirty + c!
dup bbi + c@ >path >addr dup $400 +
here 4 saveb ;

: >buf ( blk -- buf ) 3 mod ;

: load-blk ( blk -- )
dup >path >buf >addr >r here 4
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
loaded? 0= if unassign dup load-blk
then set-blk ;

: buffer ( blk -- addr )
loaded? 0= if unassign then set-blk ;

: list ( blk -- )
block dup $400 + swap do
i c@ emit loop ;

: empty-buffers bbi 3 erase ;

: update ( -- )
1 dirty curr-buf c@ + c! ;

: save-buffers ( -- )
0 save-buf 1 save-buf 2 save-buf ;

: flush save-buffers empty-buffers ;
