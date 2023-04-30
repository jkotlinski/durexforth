require io

( three block buffers at $c000-$cbff )

create bbi 0 , 0 c, \ buf block id's
create dirty 0 , 0 c,
create curr-buf 0 c,

variable map 0 map !
: path s" blocks" ;

: get## ( -- n )
chrin '0' - #10 * chrin '0' - + ;

variable t variable s

: b-a ( -- ) \ allocate sector
decimal <# s @ 0 #s bl hold 2drop
t @ 0 #s bl hold '0' hold bl hold
'a' hold '-' hold 'b' hold #>
$f $f open ioabort $f chkin ioabort
get## case #65 of \ no block
#10 0 do chrin drop loop
get## t ! chrin drop get## s !
clrchn $f close recurse endof
#66 of \ illegal track/sector
s @ 0= abort" full"
0 s ! 1 t +! clrchn $f close recurse
endof endcase clrchn $f close ;

\ Usage: "20 create-blocks" allocates
\ 20 Forth blocks = 80 sectors and
\ writes a map file named "blocks".
: create-blocks ( n -- )
1 t ! 0 s ! here map !
4 * 0 do b-a t @ c, s @ c, 1 s +!
loop map @ here path saveb ;

: load-map map @ if exit then
here path here loadb
0= abort" no blocks" map ! ;

: >addr ( buf -- addr )
$400 * $c000 + ;

: write-sector ( t s src -- ) decimal
s" #" 5 5 open ioabort
s" b-p 5 0" $f $f open ioabort
5 chkout ioabort
dup $100 + swap do i c@ emit loop
$f chkout ioabort
<# $d hold 0 #s 2drop bl hold 0 #s
bl hold '0' hold bl hold '5' hold
bl hold '2' hold 'u' hold #> type
clrchn $f close 5 close ;

: save-buf ( buf -- )
dup dirty + c@ 0= if drop exit then
load-map 0 over dirty + c!
dup bbi + c@ 1- 8 * map @ +
swap >addr dup $400 + swap do
dup @ split i write-sector
2+ $100 +loop drop ;

: >buf ( blk -- buf ) 3 mod ;

: read-sector ( dst t s -- ) decimal
s" #" 5 5 open ioabort <# 0 #s bl hold
2drop 0 #s bl hold '0' hold bl hold
'5' hold bl hold '1' hold 'u' hold #>
$f $f open ioabort 5 chkin ioabort
dup $100 + swap do chrin i c! loop
$f close 5 close clrchn ;

: load-blk ( blk -- )
load-map dup 1- 8 * map @ + swap >buf
>addr dup $400 + swap do i over @
split read-sector 2+ $100 +loop drop ;

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

: empty-buffers ( -- )
bbi 3 erase dirty 3 erase ;

: update ( -- )
1 dirty curr-buf c@ + c! ;

: save-buffers ( -- )
0 save-buf 1 save-buf 2 save-buf ;

: flush save-buffers empty-buffers ;
