
# by kevin reno
: tens a /mod ;
: digits tens tens tens tens tens 6 ;
: lead swap dup if
swap exit else drop
recurse then 1- ;
: digemit 30 + emit ;
: expand digits lead ;
: do swap digemit 1- ;
: each begin do dup 0 = until ;
: decimal dup if
expand each drop
else 30 + emit
then 20 emit ;
1 value mx
: last dup dup c@ + c@ 30 - 
mx dup a * to mx *
rot + swap ;
: less
dup dup c@ 1- swap c! ;
: doit begin  
last less dup c@ 0 = until ;
: hex 1 to mx 0
word doit drop ;
: .dec sp0 1- 1- begin
dup sp@ 2+ > while
dup @ dec 1- 1- repeat drop ;
