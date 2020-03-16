\ ! required stops working after
\ 16 includes
: hash ( addr u -- hash )
over + swap 0 -rot
do $1f * i c@ + loop ;
: included ( addr u -- )
2dup hash -rot included
(includes) $20 + (includes) do
i @ 0= if i ! leave then
dup i @ = if drop leave then
2 +loop ;
: include parse-name included ;

: required ( addr u -- )
(includes) $20 + (includes) do
i @ 0= if included leave then
2dup hash i @ = if 2drop leave then
2 +loop ;
: require parse-name required ;
