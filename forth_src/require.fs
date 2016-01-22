variable req 1e allot
req 20 0 fill
: hash ( addr u -- hash )
over + swap 0 -rot
do 1f * i c@ + loop ;
: included ( addr u -- )
req 20 + req do 
2dup hash i @ = if leave then
i @ 0= if 2dup hash i ! leave then
2 +loop included ;
: include bl word count included ;

: required ( addr u -- )
req 20 + req do 
i @ 0= if included leave then
2dup hash i @ = if 2drop leave then
2 +loop ;
: require bl word count required ;

