require open

\ select a file for input
\ no error handling
code (chkin) ( file# -- result )
w stx,
lsb lda,x tax, \ x = file#
$ffc6 jsr, \ CHKIN
0 lda,#
+branch bcc, \ carry clear: OK
1 lda,# \ carry set: error
:+
w ldx, lsb sta,x \ push result
;code

\ Set output device to open file #
code (chkout) ( file# -- result )
w stx,
lsb lda,x tax, \ x = file#
$ffc9 jsr, \ CHKOUT
0 lda,#
+branch bcc, \ carry clear: OK
1 lda,# \ carry set: error
:+
w ldx, lsb sta,x \ push result
;code

\ Reset input and output to console 
code clrch ( -- ) 
txa, pha,
$ffcc jsr,  \ CLRCH
pla, tax,
;code

\ Read status of last IO operation
code readst ( -- status )
dex, 0 lda,# msb sta,x
$ffb7 jsr, \ READST
lsb sta,x
;code

\ Get a byte from input device
code chrin ( -- chr )
dex, w stx, 0 lda,# msb sta,x
$ffcf jsr, \ CHRIN
w ldx, lsb sta,x
;code

\ handle returned result value
: ioerr ( file# result -- )
if clrch close rvs ." io err" cr abort 
then drop ;

\ Open a file
\ 'easy' version, aborts on error
: open ( caddr u sa file# -- )
dup >r (open) r> swap ioerr ;

\ Set input device to open file #
\ 'easy' version, aborts on error
: chkin ( file# -- )
dup (chkin) ioerr ;

\ Set output device to open file #
\ 'easy' version, aborts on error
: chkout 
dup (chkout) ioerr ;

\ accept only works for keyboard input
: faccept ( addr len -- len ) 
tuck 0 do \ len addr
chrin over c! 1+ \ len addr+1
dup $d = readst or \ stop at CR or EOF
if 2drop i unloop exit then
loop ;
