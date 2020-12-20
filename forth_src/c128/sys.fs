include c128

\ 'far' SYS that calls 128 kernal JSRFAR
$06 value ar $07 value xr 
$08 value yr $05 value sr
code sysfar ( bank addr -- )
\ JSRFAR uses $02-$05 ... better hope
\ we aren't that deep in the stack ...

\ set jump destination
\ yes it is supposed to be MSB-first
msb lda,x $03 sta, lsb lda,x $04 sta,
inx, lsb lda,x $02 sta, inx, \ set bank
\ save X and MMUCR
\ JSRFAR does not save MMUCR and always 
\ returns in bank 15
txa, pha, $ff00 lda, pha,
$ff6e jsr, \ JSRFAR
pla, $ff00 sta, pla, tax,
;code

\ default sys for calling ROM routines
: sys ( addr -- )
\ bank $f = RAM0 + kernal + BASIC
$f swap sysfar ;
