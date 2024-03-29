require io

$12 emit .( dump-labels) $92 emit
.(  writes VICE emulator) cr
.( labels to the PRG file 'words') cr cr
.( When written, extract the file from)
cr
.( .d64 using c1541 command) cr
.( 'read words'.) cr
.( Then, load the file from VICE) cr
.( monitor using 'll "words"') cr

\ print a VICE label definition for a
\ given nametoken. returns 1, for use
\ with dowords
: (label) ( nametoken -- 1 )
." al " dup >xt u. '.' emit
name>string
over + swap do i c@
dup 'a' < over 'z' > or if case
\ escape forbidden chars
'0' of ." :zero:" endof
'1' of ." :one:" endof
'2' of ." :two:" endof
'3' of ." :three:" endof
'4' of ." :four:" endof
'5' of ." :five:" endof
'6' of ." :six:" endof
'7' of ." :seven:" endof
'8' of ." :eight:" endof
'9' of ." :nine:" endof
'-' of ." :minus:" endof
'+' of ." :plus:" endof
'#' of ." :hash:" endof
'*' of ." :star:" endof
'/' of ." :slash:" endof
'\' of ." :backslash:" endof
'=' of ." :equals:" endof
',' of ." :comma:" endof
'.' of ." :dot:" endof
'$' of ." :dollar:" endof
'<' of ." :lt:" endof
'>' of ." :gt:" endof
'!' of ." :store:" endof
'@' of ." :fetch:" endof
';' of ." :semicolon:" endof
'[' of ." :lbracket:" endof
']' of ." :rbracket:" endof
'(' of ." :lparen:" endof
')' of ." :rparen:" endof
''' of ." :tick:" endof
'"' of ." :quote:" endof
dup emit endcase
else emit then loop
$a emit 1 ;

: dump-labels base @ >r hex
s" words,w" 1 1 open ioabort
1 chkout ioabort
['] (label) dowords
clrchn 1 close r> base ! ;
