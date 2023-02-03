\ included into base.fs

variable end
create #>buf #34 allot
: <# #>buf end ! ;
: #> 2drop #>buf end @ over - ;
: hold
\ reserve space for char at start
#>buf dup 1+ end @ #>buf - move
1 end +! #>buf c! ;
: sign 0< if '-' hold then ;
: # base @ ud/mod rot
dup a < if 7 - then $37 + hold ;
: #s # begin 2dup or while # repeat ;

: u. 0 <# #s #> type space ;

\ this is slow :(
: . dup abs 0 <# #s rot sign #>
type space ;
