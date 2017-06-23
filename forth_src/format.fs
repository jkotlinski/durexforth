variable end
: <# here end ! ;
: #> 2drop here end @ over - ;
: hold 
\ reserve space for char at start
here dup 1+ end @ here - move
1 end +!  
here c! ;
: sign 0< if '-' hold then ;
: ud/mod \ from Gforth
>r 0 r@ um/mod r> swap >r um/mod r> ;
: # base @ ud/mod rot 
dup a < if 7 - then 37 + hold ;
: #s # begin 2dup or while # repeat ;

: u. 0 <# #s #> type space ;
: . dup abs 0 <# #s rot sign #> 
type space ;

: accept ( addr u -- u )
refill source dup >in !
rot min -rot swap rot
dup >r move r> ;
