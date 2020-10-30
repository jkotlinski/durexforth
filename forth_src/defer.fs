: >body ( xt -- dataaddr ) 5 + ;
: defer create ['] abort ,
does> @ execute ;
: defer! >body ! ;
: is state @ if
postpone ['] postpone defer!
else ' defer! then ; immediate
: hide ( word -- )
parse-name find-name ?dup if
dup latest @ - ( nt size )
>r c@ $1f and 3 + ( off )
latest @ swap over +  ( srca dsta )
dup latest !
r> move then ;
