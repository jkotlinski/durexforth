: valid? ( t s -- n )
swap
dup 0 > over 18 < and if
drop 21 < exit then
dup 17 > over 25 < and if
drop 19 < exit then
dup 24 > over 31 < and if
drop 18 < exit then
dup 30 > over 36 < and if
drop 17 < exit then
drop 0 ;

: read-block ( n -- )
( tracks sectors
  1-17   0-20
  18-24  0-18
  25-30  0-17
  31-35  0-16
 total: 683 sectors )
 2* 2* 1 swap \ t s
;
