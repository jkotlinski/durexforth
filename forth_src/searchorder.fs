
\ core words, unimplimented by default
: definitions get-order 1- swap set-current
0 do nip loop ;
1 value forth-wordlist
\ extension words
: only -1 set-order ;
: also get-order over swap 1+ set-order ;
: (wordlist)
create , does> 
@ >r get-order nip r> swap set-order ;
forth-wordlist (wordlist) forth
: order ." Order: " get-order 0 do . loop
." Current: " get-current . cr;
: previous get-order nip 1- set-order ;
