: delete
parse-name find-name 
if
xt> ( dp )
dup 2+ c@ 3 + ( offset )
swap latest - >r ( offset ) 
latest + ( dst )
latest swap dup to latest ( src dst ) r>
move exit then 
type ."  not found"
;
