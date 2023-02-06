: memcmp ( a1 a2 n -- diff )
0 ?do over c@ over c@ - ?dup
if  >r 2drop r> unloop exit  then
swap 1+ swap 1+ loop  2drop 0 ;

: strcmp ( a1 n1 a2 n2 -- diff )
rot swap 2dup - >r min memcmp r>
over if drop else nip then ;

: signum ( n1 -- -1/0/1 )
dup 0 > if drop 1 exit then
dup 0< if drop -1 then ;

: compare strcmp signum ;
