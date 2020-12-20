(
The C128's MMU has 'magic' addresses 
at $ff00-$ff04 that our bitmap must not
interfere with; so the bitmap must start
at $c000, with the color map at $e000.
Note that this means part of the bitmap
is under IO space, so clrcol and blkcol
can no longer 'blindly' write to it.
)

$c000 value bmpbase
$e000 value colbase

code kernal-in
%00001110 lda,# $ff00 sta, cli, ;code
code kernal-out
sei, %00111111 lda,# $ff00 sta, ;code
code charrom-in
$ff00 lda, %11001111 and,#
%00000001 ora,# $ff00 sta, ;code

code hires
\ VIC bank = $c000
$dd00 lda, %11111100 and,# $dd00 sta,

\ Use Kernal variables to set up screen
\ mode. The Kernal IRQ handler picks
\ these up and sets up the VIC
\ accordingly.

\ hi-res mode
%00100000 lda,# $d8 sta,
\ VIC sees RAM instead of character ROM
%00000100 lda,# $d9 sta,
\ value at $0a2d gets put into $d018
$80 lda,# $0a2d sta,
;code

code lores
$dd00 lda, %00000011 ora,# $dd00 sta,
$0 lda,# $d8 sta, $d9 sta,
$78 lda,# $0a2d sta,
;code
