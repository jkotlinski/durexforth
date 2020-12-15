\ Useful words for the Commodore 128

\ Put C128 into 2MHz mode
\ disables 40 column display
code fast ( - )
$d011 lda,
%11101111 and,#
$d011 sta,
$d030 lda,
%00000001 ora,#
$d030 sta,
;code

\ Put C128 into 1MHz mode
\ re-enables 40 column display
code fast ( - )
$d011 lda,
%00010000 ora,#
$d011 sta,
$d030 lda,
%11111110 and,#
$d030 sta,
;code

: fast? ( - is-fast ) \ get current processor speed
$d030 c@
%00000001 and,# ;
