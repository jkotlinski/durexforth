\ ! required stops working after
\ 16 includes
: hash ( addr u -- hash )
over + swap 0 -rot
do $1f * i c@ + loop ;
: included ( addr u -- )
2dup hash >r included r>
(includes) $20 + (includes) do
i @ 0= if i ! leave then
dup i @ = if drop leave then
2 +loop ;
: include parse-name included ;

: required ( addr u -- )
(includes) $20 + (includes) do
i @ 0= if included unloop exit then
2dup hash i @ = if 2drop unloop exit
then 2 +loop 1 abort" too many" ;
: require parse-name required ;

hide hash
