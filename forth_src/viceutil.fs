$12 emit .( dump-labels) $92 emit
.(  writes VICE emulator
labels to the file 'words'

When written, extract the file from
.d64 using c1541 command 
'read words'.
Then, load the file from VICE 
monitor using 'll "words"'
)

: dump-labels base @ hex
s" words" 1 openw
latest @ begin ?dup while
." al " dup >cfa . '.' emit
dup name>string
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
$a emit @ repeat 1 closew base ! ;
