\ ! required stops working after
\ 16 includes
variable req $1e allot
req $20 0 fill
: hash ( addr u -- hash )
over + swap 0 -rot
do $1f * i c@ + loop ;
: included ( addr u -- )
2dup hash -rot included
req $20 + req do 
i @ 0= if i ! leave then
dup i @ = if drop leave then
2 +loop ;
: include bl word count included ;

: required ( addr u -- )
req $20 + req do 
i @ 0= if included leave then
2dup hash i @ = if 2drop leave then
2 +loop ;
: require bl word count required ;

