: only -1 set-order ;
: also get-order over swap 1+ set-order ;
: (wordlist)
create , does> 
@ >r get-order nip r> swap set-order ;
forth-wordlist (wordlist) forth
: order \ TODO
;
: previous get-order nip 1- set-order ;
