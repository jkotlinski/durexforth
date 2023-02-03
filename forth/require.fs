\ included into base.fs

: hash ( addr u -- hash )
over + swap 0 -rot
do $1f * i c@ + loop ;

: included ( addr u -- )
2dup hash (includes)
here to (includes) , , included ;
: include parse-name included ;

: required ( addr u -- )
2dup hash (includes) begin ?dup while
2dup 2+ @ = if 2drop 2drop exit then
@ repeat drop included ;

: require parse-name required ;

hide hash
