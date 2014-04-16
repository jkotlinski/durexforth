

15 allot value sid
var voice

here # 95 notes from c0, pal
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
value freqtab 

:asm voicec@+
voice lda, clc, 0 adc,x 0 sta,x
+branch bcc, 1 inc,x :+ ;asm

# creates array of 4 bytes.
# first: current byte
# 2nd. voice 0
# 3rd. voice 1
# 4th. voice 2
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

:asm voice7+ # voice c@ 7 * +
.voice7* jsr,
clc, 0 adc,x 0 sta,x +branch bcc,
1 inc,x :+ ;asm

create .ctl
sid 4 + 100/ lda,# zptmp 1+ sta,
.voice7* jsr,
clc, sid 4 + ff and adc,# zptmp sta,
+branch bcc, zptmp 1+ inc, :+ rts,
:asm ctl 
dex, dex,
.ctl jsr, 
zptmp lda, 0 sta,x
zptmp 1+ lda, 1 sta,x
;asm

: sid-cutoff d415 ! ;
: sid-flt d417 c! ;
: sid-vol! d418 c! ;

( write adsr )
: srad! ( SR AD -- ) [ sid 5 + literal ] voice7+ ! ;

: note! ( i -- )
2* freqtab + @ sid voice7+ ! ;

:asm gate-on 
.ctl jsr, 0 ldy,#
zptmp lda,(y) 1 eor,#
zptmp sta,(y) ;asm
:asm gate-off
.ctl jsr, 0 ldy,#
zptmp lda,(y) fe and,#
zptmp sta,(y) ;asm

2b value .str
create .str-pop
txa, tay,
voice lda, asl,a tax,
.str inc,x +branch bne,
.str 1+ inc,x :+
tya, tax, rts,
:asm str-pop .str-pop jsr, ;asm

create .strget
zptmp stx, voice lda, asl,a tax,
.str lda,(x) zptmp ldx, rts,
:asm strget 
dex, dex, 0 lda,# 1 sta,x
.strget jsr, 0 sta,x ;asm

create notetab ( char -- notediff )
0 lda,x
key c cmp,# +branch bne,
0 lda,# 0 sta,x rts,
:+ key d cmp,# +branch bne,
2 lda,# 0 sta,x rts,
:+ key e cmp,# +branch bne,
4 lda,# 0 sta,x rts,
:+ key f cmp,# +branch bne,
5 lda,# 0 sta,x rts,
:+ key g cmp,# +branch bne,
7 lda,# 0 sta,x rts,
:+ key a cmp,# +branch bne,
9 lda,# 0 sta,x rts,
:+ key b cmp,# +branch bne,
b lda,# 0 sta,x rts,
:+ 7f lda,# 0 sta,x rts,

create notrest
0 lda,x 7f cmp,# +branch beq,
dex, dex, 1 lda,# 0 sta,x ;asm
:+ 0 lda,# 0 sta,x 1 sta,x ;asm

:asm str2note
notetab jsr,
.str-pop jsr,
.strget jsr,
key + cmp,# +branch bne,
0 inc,x .str-pop jsr, notrest jmp,
:+ key - cmp,# +branch bne,
0 dec,x .str-pop jsr, notrest jmp,
:+ notrest jmp,

create .read-pause
dex, dex, 0 lda,# 1 sta,x
.strget jsr,
key 1 cmp,# +branch bne,
.str-pop jsr, .strget jsr,
key 6 cmp,# +branch bne,
.str-pop jsr, 60 10 / 1- lda,# 0 sta,x rts,
:+ 60 1- lda,# 0 sta,x rts,
:+ key 2 cmp,# +branch bne,
.str-pop jsr, .strget jsr,
key 4 cmp,# +branch bne,
.str-pop jsr, 60 18 / 1- lda,# 0 sta,x rts,
:+ 60 2 / 1- lda,# 0 sta,x rts,
:+ key 3 cmp,# +branch bne,
.str-pop jsr, .strget jsr,
key 2 cmp,# +branch bne,
.str-pop jsr, 60 20 / 1- lda,# 0 sta,x rts,
:+ 60 3 / 1- lda,# 0 sta,x rts,
:+ key 4 cmp,# +branch bne,
.str-pop jsr, 60 4 / 1- lda,# 0 sta,x rts,
:+ key 6 cmp,# +branch bne,
.str-pop jsr, 60 6 / 1- lda,# 0 sta,x rts,
:+ key 8 cmp,# +branch bne,
.str-pop jsr, 60 8 / 1- lda,# 0 sta,x rts,
:+ 0 lda,# 0 sta,x rts,

:asm read-pause
.read-pause jsr,
0 lda,x +branch bne,
default-pause lda, 0 sta,x
:+ 
.strget jsr,
key . cmp,# +branch bne,
.str-pop jsr,
0 lda,x lsr,a clc, 0 adc,x 0 sta,x
:+ 
0 lda,x pause sta, inx, inx, ;asm

:asm read-default-pause
.read-pause jsr,
default-pause sta, inx, inx, ;asm

: play-note ( -- )
strget ?dup if
str2note if
octave c@ + note! gate-on then
read-pause then ;

:asm o
.str-pop jsr,
.strget jsr, # new character in a
sec, key 0 sbc,#
# multiply by c
asl,a asl,a zptmp sta,
asl,a clc, zptmp adc,
octave sta,
.str-pop jsr, ;asm

: do-commands ( -- done )
strget case
[char] l of str-pop read-default-pause recurse endof
[char] o of o recurse endof
[char] < of str-pop fff4 octave +! recurse endof
[char] > of str-pop c octave +! recurse endof
[char] & of str-pop 1 tie c! recurse endof
d of str-pop recurse endof
bl of str-pop recurse endof
endcase ;

:asm stop-note
tie lda, +branch beq,
0 lda,# tie sta, ;asm
:+ loc gate-off >cfa jmp,

:asm pause>0
dex, dex,
pause lda,
0 sta,x 1 sta,x ;asm

:asm decpause1=
dex, dex, 0 ldy,#
pause dec, +branch beq,
0 sty,x 1 sty,x ;asm
:+ iny, 0 sty,x ;asm

: voicetick
pause>0 if decpause1= if 
do-commands stop-note then 
else play-note then ;

:asm voice0 
0 lda,# voice sta, 
octave 1+ lda, octave sta,
tie 1+ lda, tie sta,
pause 1+ lda, pause sta,
default-pause 1+ lda, default-pause sta,
;asm

:asm voice1 
octave lda, octave 1+ sta,
tie lda, tie 1+ sta,
pause lda, pause 1+ sta,
default-pause lda, default-pause 1+ sta,
1 lda,# voice sta, 
octave 2+ lda, octave sta,
tie 2+ lda, tie sta,
pause 2+ lda, pause sta,
default-pause 2+ lda, default-pause sta,
;asm

:asm voice2 
octave lda, octave 2+ sta,
tie lda, tie 2+ sta,
pause lda, pause 2+ sta,
default-pause lda, default-pause 2+ sta,
2 lda,# voice sta, 
octave 3 + lda, octave sta,
tie 3 + lda, tie sta,
pause 3 + lda, pause sta,
default-pause 3 + lda, default-pause sta,
;asm

:asm voicedone
octave lda, octave 3 + sta,
tie lda, tie 3 + sta,
pause lda, pause 3 + sta,
default-pause lda, default-pause 3 + sta,
;asm

:asm wait 
# visualize lag
# a2 lda, sec, 0 sbc,x d020 sta,
0 lda,x
:- a2 cmp, -branch beq,
0 inc,x ;asm

:asm apply-sid
14 ldy,#
:- sid lda,y d400 sta,y
dey, -branch bpl, ;asm

: play 
voice0 do-commands
voice1 do-commands
voice2 do-commands voicedone 
a2 c@ wait begin strget while
voice0 voicetick
voice1 voicetick
voice2 voicetick voicedone 
wait apply-sid
repeat drop ;

: init-voices
f sid-vol!
3 0 do i voice c! 0 pause i + 1+ c!
10 ctl c! 891a srad! loop ;

: play-mml ( string1 string2 string3 -- )
# init sentinels
over + dup >r dup c@ >r 0 swap c! .str !
over + dup >r dup c@ >r 0 swap c! .str 2+ !
over + dup >r dup c@ >r 0 swap c! .str 2+ 2+ !

init-voices play
3 0 do i voice c! gate-off loop
apply-sid

# restore sentinels
r> r> c! r> r> c! r> r> c! ;

loc play-mml
hide-to sid
hidden
