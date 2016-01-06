variable end
: <# here end ! ;
: #> 2drop here end @ over - ;
: ins \ reserve space for char at start
here dup 1+ end @ here - move
1 end +! ;
: hold ins here c! ;
: sign 0< if '-' hold then ;
: ud/mod \ from Gforth
>r 0 r@ um/mod r> swap >r
um/mod r> ;
: # base @ ud/mod rot #pet hold ;
: #s # begin 2dup or while # repeat ;
: pet# ( char -- num )
7f and dup \ lowercase
':' < if '0' else '7' then - ;
: digit? ( char -- flag )
pet# dup 0< 0= swap base @ < and ;

code d+ ( d1 d2 -- d3 )
clc,
sp0 1+ lda,x sp0 3 + adc,x sp0 3 + sta,x
sp1 1+ lda,x sp1 3 + adc,x sp1 3 + sta,x
sp0 lda,x sp0 2+ adc,x sp0 2+ sta,x
sp1 lda,x sp1 2+ adc,x sp1 2+ sta,x
inx, inx, ;code
: accumulate ( +d0 addr digit - +d1 addr )
swap >r swap base @ um* drop
rot base @ um* d+ r> ;

: >number ( ud addr u -- ud addr u )
begin over c@ digit? over and while
>r dup c@ pet# accumulate
1+ r> 1- repeat ;

: accept ( addr u -- u )
refill source dup >in !
rot min -rot swap rot
dup >r move r> ;
