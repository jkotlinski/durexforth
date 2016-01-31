\ forth2012 compatibility stuff

: environment? 2drop 0 ;
: cells 2* ;
: cell+ 2+ ;
: char+ 1+ ;
: chars ; : align ; : aligned ;

\ from FIG UK
: sm/rem 
2dup xor >r over >r abs >r dabs
r> um/mod swap r> ?negate
swap r> ?negate ;
