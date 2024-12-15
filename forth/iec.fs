require io

code listen ( dv -- )
here 1+
$ffff stx, \ dummy address
lsb lda,x  \ one byte more but faster
$ffb1 jsr, \ listen
here 1+ swap !  \ actual address
$00 ldx,# inx,     \ dummy byte
rts, end-code

code second ( sa -- )
here 1+
$ffff stx,
lsb lda,x
$ff93 jsr, \ second
here 1+ swap !
$00 ldx,# inx,
rts, end-code

code talk ( dv -- )
here 1+
$ffff stx,
lsb lda,x
$ffb4 jsr, \ talk
here 1+ swap !
$00 ldx,# inx,
rts, end-code

code tksa ( sa -- )
here 1+
$ffff stx,
lsb lda,x
$ff96 jsr, \ tksa
here 1+ swap !
$00 ldx,# inx,
rts, end-code

code unlisten ( -- )
here 1+
$ffff stx,
$ffae jsr, \ unlisten
here 1+ swap !
$00 ldx,#
rts, end-code

code untalk ( -- )
here 1+
$ffff stx,
$ffab jsr, \ untalk
here 1+ swap !
$00 ldx,#
rts, end-code

code ciout ( chr -- )
here 1+
$ffff stx,
lsb lda,x
$ffa8 jsr, \ ciout
here 1+ swap !
$00 ldx,# inx,
rts, end-code

code acptr ( -- chr )
dex, w stx, 0 lda,# msb sta,x
$ffa5 jsr, \ acptr
w ldx, lsb sta,x
rts, end-code

: iqt readst ioabort ; \ legacy of if quit then

: tfname ( addr len -- )
over + swap do
i c@ ciout loop ;

: send-cmd ( addr len -- )
0 $90 c!          \ always zero ST
?dup if           \ command to be sent?
$ba c@ listen iqt \ Yes
$6f second iqt    \ don't require $ff open,
                  \ error channel always open.
tfname unlisten   \ turn around
then              \ No
$ba c@ talk iqt   \ listener is now talker
$6f tksa iqt      \ $6f data channel only
acptr readst begin
0= while emit
acptr readst repeat
emit untalk cr ; \ no need to close error channel

: dos source >in @ /string
dup >in +! \ consume buffer
send-cmd ;

: bsave ( addr addr -- addr )
0 $90 c! parse-name \ always zero ST
$ba c@ listen iqt
$f1 second iqt      \ $F0 + $01 write prg
tfname unlisten     \ always all devices
$ba c@ listen       \ if we get here,
$61 second          \ the device exists
over split ciout ciout
                    \ send load addr
over dup
0 do i + dup c@ ciout loop
1+ \ keep saveb compatibility
unlisten $ba c@ listen
$e1 second \ $E0 + $01 close
unlisten
;

: dir parse-name ?dup if
else drop s" $0"
then 0 $90 c! \ always zero ST
$ba c@ listen iqt
$f0 second iqt \ $F0 OPEN + $00 channel, read as prg
tfname
unlisten $ba c@ talk  \ turn around
$60 tksa              \ $60 open, opened channel
acptr acptr 2drop     \ listener is now talker. drop load address
here begin acptr over c! 1+ \ load HERE loop until EOF
readst until drop untalk
$ba c@ listen $e0 second \ $E0 + $00 close
unlisten
page here rdir ;
