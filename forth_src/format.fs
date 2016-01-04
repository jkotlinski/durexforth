variable end
: <# here end ! ;
: #> 2drop here end @ over - ;
: ins \ reserve space for char at start
here dup 1+ end @ here - cmove> 
1 end +! ;
: hold ins here c! ;
: sign 0< if [char] - hold then ;
: ud/mod \ from Gforth
>r 0 r@ um/mod r> swap >r
um/mod r> ;
: # base @ ud/mod rot #pet hold ;
: #s # begin 2dup or while # repeat ;
