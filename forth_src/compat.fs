\ forth2012 compatibility stuff

: environment? 2drop 0 ;
: cells 2* ;
: cell+ 2+ ;
: char+ 1+ ;
: chars ; : align ; : aligned ;

: 2@ ( addr -- x1 x2 )
dup 2+ @ swap @ ;
: 2! ( x1 x2 addr -- )
swap over ! 2+ ! ;

code d+ ( d1 d2 -- d3 )
clc,
lsb 1+ lda,x lsb 3 + adc,x lsb 3 + sta,x
msb 1+ lda,x msb 3 + adc,x msb 3 + sta,x
lsb lda,x lsb 2+ adc,x lsb 2+ sta,x
msb lda,x msb 2+ adc,x msb 2+ sta,x
inx, inx, ;code

: accumulate ( +d0 addr digit - +d1 addr )
swap >r swap base @ um* drop
rot base @ um* d+ r> ;

: pet# ( char -- num )
7f and dup \ lowercase
':' < if '0' else '7' then - ;

: digit? ( char -- flag )
pet# dup 0< 0= swap base @ < and ;

: >number ( ud addr u -- ud addr u )
begin over c@ digit? over and while
>r dup c@ pet# accumulate
1+ r> 1- repeat ;

\ from FIG UK
: sm/rem 
2dup xor >r over >r abs >r dabs
r> um/mod swap r> ?negate
swap r> ?negate ;
