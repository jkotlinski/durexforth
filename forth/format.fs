variable holdp
#34 allot here dup
: <# literal holdp ! ;
: #> 2drop holdp @ literal over - ;
: hold -1 holdp +! holdp @ c! ;

: sign 0< if '-' hold then ;
: # base @ ud/mod rot
dup a < if 7 - then $37 + hold ;
: #s # begin 2dup or while # repeat ;

: u. 0 <# #s #> type space ;

\ this is slow :(
: . dup abs 0 <# #s rot sign #>
type space ;
