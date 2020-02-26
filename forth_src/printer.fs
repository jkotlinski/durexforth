\ simple printer driver for a printer
\ connected to #4

code print  ( addr n -- )
  \ preserve stack depth
  w stx,

  \ no filename
  0 lda,#   0 ldx,#   0 ldy,#
  $ffbd jsr,

  \ file, device, channel
  4 lda,#   4 ldx,#   7 ldy,#
  $ffba jsr,

  \ open
  $ffc0 jsr,

  \ chkout
  4 ldx,#
  $ffc9 jsr,

  \ restore the stack depth and
  \ simply call 'type'
  w ldx,
  ' type jsr,
  w stx,

  \ clrchn
  $ffcc jsr,

  \ close file
  4 lda,#
  $ffc3 jsr,

  \ TODO: it seems that some more
  \ cleanup should be done...?

  \ restore stack depth
  w ldx,
;code

\ prints the file being edited
: vprint  ( -- )
  bufstart eof @ over - print ;

\ prints a file fom disk
\ limited by memory, since it's
\ loaded all at once

\ : print-file  ( "name" -- )
\   parse-name $a000 loadb
\   prn ;

: prn(  ( "text" -- )  immediate
  ')' word count print ;

: prn"  ( "text" -- )  immediate
  postpone s" postpone print ;
