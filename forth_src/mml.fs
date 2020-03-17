base @ hex
variable sid 13 allot
variable voice

\ creates array of 4 bytes.
\ first: current byte
\ 2nd. voice 0
\ 3rd. voice 1
\ 4th. voice 2
: voicedata here 0 , 0 , value ;
voicedata octave
voicedata tie
voicedata default-pause
voicedata pause

create .voice7*
voice lda,
+branch bne, rts,
:+ 1 cmp,# +branch bne,
7 lda,# rts,
:+ 7 2* lda,# rts,

code voice7+ \ voice c@ 7 * +
.voice7* jsr,
clc, lsb adc,x lsb sta,x +branch bcc,
msb inc,x :+ ;code

create .ctl
sid 4 + 100/ lda,# w 1+ sta,
.voice7* jsr,
clc, sid 4 + ff and adc,# w sta,
+branch bcc, w 1+ inc, :+ rts,
code ctl 
dex,
.ctl jsr, 
w lda, lsb sta,x
w 1+ lda, msb sta,x
;code

: sid-cutoff d415 ! ;
: sid-flt d417 c! ;
: sid-vol! d418 c! ;

( write adsr )
: srad! ( SR AD -- ) 
[ sid 5 + literal ] voice7+ ! ;

here \ 95 notes from c0, pal
116 , 127 , 138 , 14b , 15e , 173 ,
189 , 1a1 , 1ba , 1d4 , 1f0 , 20d , 
22c , 24e , 271 , 296 , 2bd , 2e7 , 
313 , 342 , 374 , 3a8 , 3e0 , 41b ,
459 , 49c , 4e2 , 52c , 57b , 5ce , 
627 , 684 , 6e8 , 751 , 7c0 , 836 , 
8b3 , 938 , 9c4 , a59 , af6 , b9d , 
c4e , d09 , dd0 , ea2 , f81 , 106d ,
1167 , 1270 , 1388 , 14b2 , 15ed , 
173a , 189c , 1a13 , 1ba0 , 1d44 , 
1f02 , 20da , 22ce , 24e0 , 2711 , 
2964 , 2bda , 2e75 , 3138 , 3426 , 
3740 , 3a89 , 3e04 , 41b4 , 459c , 
49c0 , 4e22 , 52c8 , 57b4 , 5ceb , 
6271 , 684c , 6e80 , 7512 , 7c08 , 
8368 , 8b38 , 9380 , 9c45 , a590 ,
af68 , b9d6 , c4e3 , d098 , dd00 , 
ea24 , f810 ,
: note! ( i -- )
2* [ swap ] literal + @ sid voice7+ ! ;

code gate-on 
.ctl jsr, 0 ldy,#
w lda,(y) 1 eor,#
w sta,(y) ;code
code gate-off
.ctl jsr, 0 ldy,#
w lda,(y) fe and,#
w sta,(y) ;code

2b value .str
create .str-pop
txa, tay,
voice lda, asl,a tax,
.str inc,x +branch bne,
.str 1+ inc,x :+
tya, tax, rts,
code str-pop .str-pop jsr, ;code

create .strget
w stx, voice lda, asl,a tax,
.str lda,(x) w ldx, rts,
code strget 
dex, 0 lda,# msb sta,x
.strget jsr, lsb sta,x ;code

create notetab ( char -- notediff )
lsb lda,x
'c' cmp,# +branch bne,
0 lda,# lsb sta,x rts,
:+ 'd' cmp,# +branch bne,
2 lda,# lsb sta,x rts,
:+ 'e' cmp,# +branch bne,
4 lda,# lsb sta,x rts,
:+ 'f' cmp,# +branch bne,
5 lda,# lsb sta,x rts,
:+ 'g' cmp,# +branch bne,
7 lda,# lsb sta,x rts,
:+ 'a' cmp,# +branch bne,
9 lda,# lsb sta,x rts,
:+ 'b' cmp,# +branch bne,
b lda,# lsb sta,x rts,
:+ 7f lda,# lsb sta,x rts,

create notrest
lsb lda,x 7f cmp,# +branch beq,
dex, 1 lda,# lsb sta,x ;code
:+ 0 lda,# lsb sta,x msb sta,x ;code

code str2note
notetab jsr,
.str-pop jsr,
.strget jsr,
'+' cmp,# +branch bne,
lsb inc,x .str-pop jsr, notrest jmp,
:+ '-' cmp,# +branch bne,
lsb dec,x .str-pop jsr, notrest jmp,
:+ notrest jmp,

create .read-pause
dex, 0 lda,# msb sta,x
.strget jsr,
'1' cmp,# +branch bne,
.str-pop jsr, .strget jsr,
'6' cmp,# +branch bne,
.str-pop jsr, 60 10 / lda,# lsb sta,x 
rts,
:+ 60 lda,# lsb sta,x rts,
:+ '2' cmp,# +branch bne,
.str-pop jsr, .strget jsr,
'4' cmp,# +branch bne,
.str-pop jsr, 60 18 / lda,# lsb sta,x 
rts,
:+ 60 2 / lda,# lsb sta,x rts,
:+ '3' cmp,# +branch bne,
.str-pop jsr, .strget jsr,
'2' cmp,# +branch bne,
.str-pop jsr, 60 20 / lda,# lsb sta,x 
rts,
:+ 60 3 / lda,# lsb sta,x rts,
:+ '4' cmp,# +branch bne,
.str-pop jsr, 60 4 / lda,# lsb sta,x 
rts,
:+ '6' cmp,# +branch bne,
.str-pop jsr, 60 6 / lda,# lsb sta,x 
rts,
:+ '8' cmp,# +branch bne,
.str-pop jsr, 60 8 / lda,# lsb sta,x 
rts,
:+ 0 lda,# lsb sta,x rts,

code read-pause
.read-pause jsr,
lsb lda,x +branch bne,
default-pause lda, lsb sta,x
:+ 
.strget jsr,
'.' cmp,# +branch bne,
.str-pop jsr,
lsb lda,x lsr,a clc, 
lsb adc,x lsb sta,x
:+ 
lsb dec,x ;code

code read-default-pause
.read-pause jsr,
lsb lda,x default-pause sta, 
inx, ;code

: play-note ( -- )
strget ?dup if
str2note if
octave c@ + note! gate-on then
read-pause pause c! then ;

code o
.str-pop jsr,
.strget jsr, \ new character in a
sec, '0' sbc,#
\ multiply by c
asl,a asl,a w sta,
asl,a clc, w adc,
octave sta,
.str-pop jsr, ;code

: do-commands ( -- done )
strget case
'l' of str-pop 
read-default-pause recurse endof
'o' of o recurse endof
'<' of str-pop fff4 octave +!
recurse endof
'>' of str-pop c octave +! 
recurse endof
'&' of str-pop 1 tie c! 
recurse endof
d of str-pop recurse endof
bl of str-pop recurse endof
endcase ;

code stop-note
tie lda, +branch beq,
0 lda,# tie sta, ;code
:+ ' gate-off jmp,

code pause>0
dex,
pause lda,
lsb sta,x msb sta,x ;code

code decpause1=
dex, 0 ldy,#
pause dec, +branch beq,
lsb sty,x msb sty,x ;code
:+ iny, lsb sty,x ;code

: voicetick
pause>0 if decpause1= if 
do-commands stop-note then 
else play-note then ;

code voice0 
0 lda,# voice sta, 
octave 1+ lda, octave sta,
tie 1+ lda, tie sta,
pause 1+ lda, pause sta,
default-pause 1+ lda, 
default-pause sta,
;code

code voice1 
octave lda, octave 1+ sta,
tie lda, tie 1+ sta,
pause lda, pause 1+ sta,
default-pause lda, 
default-pause 1+ sta,
1 lda,# voice sta, 
octave 2+ lda, octave sta,
tie 2+ lda, tie sta,
pause 2+ lda, pause sta,
default-pause 2+ lda, 
default-pause sta,
;code

code voice2 
octave lda, octave 2+ sta,
tie lda, tie 2+ sta,
pause lda, pause 2+ sta,
default-pause lda, 
default-pause 2+ sta,
2 lda,# voice sta, 
octave 3 + lda, octave sta,
tie 3 + lda, tie sta,
pause 3 + lda, pause sta,
default-pause 3 + lda, 
default-pause sta,
;code

code voicedone
octave lda, octave 3 + sta,
tie lda, tie 3 + sta,
pause lda, pause 3 + sta,
default-pause lda, 
default-pause 3 + sta,
;code

code wait 
\ visualize lag
\ a2 lda, sec, lsb sbc,x d020 sta,
lsb lda,x
:- a2 cmp, -branch beq,
lsb inc,x ;code

code apply-sid
14 ldy,#
:- sid lda,y d400 sta,y
dey, -branch bpl, ;code

code notdone
dex,
0 lda,# voice sta,
.strget jsr, pause 1+ ora, +branch bne,
voice inc,
.strget jsr, pause 2+ ora, +branch bne,
voice inc,
.strget jsr, pause 3 + ora, +branch bne,
0 lda,# lsb sta,x msb sta,x ;code
:+ :+ :+
lsb sta,x ;code

: play 
voice0 do-commands
voice1 do-commands
voice2 do-commands voicedone 
a2 c@ wait begin notdone while
voice0 voicetick
voice1 voicetick
voice2 voicetick voicedone 
wait apply-sid
repeat drop ;

: init-voices
f sid-vol!
3 0 do i voice c! 0 pause i + 1+ c!
10 ctl c! 891a srad! loop ;

: play-mml ( str1 str2 str3 -- )
\ init sentinels
over + dup >r dup c@ >r 0 swap c! .str
!
over + dup >r dup c@ >r 0 swap c! .str
2+ !
over + dup >r dup c@ >r 0 swap c! .str
2+ 2+ !

init-voices play
3 0 do i voice c! gate-off loop
apply-sid

\ restore sentinels
r> r> c! r> r> c! r> r> c! ;

base !
