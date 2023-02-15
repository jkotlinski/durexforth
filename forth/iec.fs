require io

code listen ( dv -- )
here 1+
$ffff stx, \ dummy address
lsb lda,x  \ one byte more but faster
$ffb1 jsr, \ listen
here 1+ swap !  \ actual address
$00 ldx,# inx,     \ dummy byte
;code

code second ( sa -- )
here 1+
$ffff stx, 
lsb lda,x
$ff93 jsr, \ second
here 1+ swap !
$00 ldx,# inx, 
;code

code talk ( dv -- )
here 1+
$ffff stx, 
lsb lda,x 
$ffb4 jsr, \ talk
here 1+ swap !
$00 ldx,# inx, 
;code

code tksa ( sa -- )
here 1+
$ffff stx, 
lsb lda,x 
$ff96 jsr, \ tksa
here 1+ swap !
$00 ldx,# inx, 
;code

code unlisten ( -- )
here 1+
$ffff stx,
$ffae jsr, \unlisten
here 1+ swap ! 
$00 ldx,# 
;code

code untalk ( -- )
here 1+
$ffff stx,
$ffab jsr, \ untalk
here 1+ swap ! 
$00 ldx,# 
;code

code ciout ( chr -- )
here 1+
$ffff stx, 
lsb lda,x 
$ffa8 jsr, \ ciout
here 1+ swap ! 
$00 ldx,# inx,
;code

code acptr ( -- chr )
dex, w stx, 0 lda,# msb sta,x
$ffa5 jsr, \ acptr
w ldx, lsb sta,x
;code

: iqt readst ioabort ; \ legacy of if quit then

: tfname ( addr len -- )
over + swap do
i c@ ciout loop ;

: send-cmd ( addr len -- )
0 $90 c!          \ always zero ST
?dup if
$ba c@ listen iqt 
$ff second iqt    \ don't require $ff open
                  \ for read of error channel
tfname unlisten 
then               \ turn around
                   \ listener is now talker
$ba c@ talk iqt
$6f tksa iqt       \ $6f data channel only
acptr readst begin
0= while emit
acptr readst repeat
emit untalk cr ;

: dos source >in @ /string
dup >in +! \ consume buffer
send-cmd ;

: bsave ( addr addr -- addr )
0 $90 c! parse-name \ always zero ST
$ba c@ listen iqt
$f1 second iqt      \ $F0 + $01 write prg
tfname unlisten
$ba c@ listen       \ if we get here,
$61 second          \ the device exists
over dup 100/ ciout $ff and ciout
over dup -
0 do i + dup c@ ciout loop
1+
unlisten
$ba c@ listen $e1 second
unlisten
;

: dir parse-name ?dup if
else drop s" $0"
then 0 $90 c! \ always zero ST
$ba c@ listen iqt
$f0 second iqt \ $F0 + $00 read as prg
over + swap do \ transmit filename
i c@ ciout loop
unlisten $ba c@ talk        \ turn around
                            \ listener is now talker
$60 tksa acptr acptr 2drop  \ drop load address
here begin acptr over c! 1+ \ load HERE loop until EOF
readst until drop untalk
$ba c@ listen $e0 second unlisten
page here rdir ;
