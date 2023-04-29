require io

( three block buffers at $c000-$cbff )

create bbi -1 , -1 c, \ buf block id's
create dirty 0 , 0 c,
create curr-buf 0 c,

variable map 0 map !
: path s" blocks" ;

\ block-allocate.
\ returns true on success.
: b-a ( track sector -- flag )
<# 0 #s bl hold 2drop 0 #s bl hold
'0' hold bl hold 'a' hold '-' hold
'b' hold #> $f $f open ioabort
$f chkin ioabort
\ TODO: get next free track/sector
\ from the error string. as is,
\ it is way too slow.
chrin begin chrin emit
readst until clrchn $f close '0' = ;

\ Usage: "20 create-blocks" allocates
\ 20 Forth blocks = 80 sectors and
\ writes a map file named "blocks".
: create-blocks ( n -- )
4 * here map ! #36 1 do i #18 <> if
#21 0 do j i b-a if j c, i c, 1-
?dup 0= if map @ here path saveb
unloop unloop exit then then
loop then loop 1 abort" full" ;

: load-map map @ if exit then
here path here loadb
0= abort" no blocks" map ! ;

: >addr ( buf -- addr )
$400 * $c000 + ;

: save-buf ( buf -- )
dup dirty + c@ 0= if drop exit then
load-map
\ TODO
\ 0 over dirty + c!
\ dup bbi + c@ dup scratch
\ here >path >addr dup
\ $400 + here 4 saveb
;

: >buf ( blk -- buf ) 3 mod ;

: load-sector ( dst src -- )
dup c@ swap 1+ c@ \ dst track sector
s" #" 5 5 open ioabort <# 0 #s bl hold
2drop 0 #s bl hold '0' hold bl hold
'5' hold ':' hold '1' hold 'u' hold #>
$f $f open ioabort 5 chkin ioabort
dup $100 + swap do chrin i c! loop
$f close 5 close clrchn ;

: load-blk ( blk -- ) load-map
 dup >buf >addr  swap 4    * map @ +
2dup load-sector swap $100 + swap 2+
2dup load-sector swap $100 + swap 2+
2dup load-sector swap $100 + swap 2+
     load-sector ;

: set-blk ( blk -- addr )
dup >buf curr-buf c!
dup dup >buf bbi + c! >buf >addr ;

: unassign ( blk -- blk )
dup >buf dup save-buf bbi + -1 swap c! ;

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

: empty-buffers ( -- )
bbi 3 -1 fill dirty 3 erase ;

: update ( -- )
1 dirty curr-buf c@ + c! ;

: save-buffers ( -- )
0 save-buf 1 save-buf 2 save-buf ;

: flush save-buffers empty-buffers ;
