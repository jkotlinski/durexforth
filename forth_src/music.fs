

19 allot value sid
var voice

header freqtab # 95 notes from c0, pal
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

# creates array of 3 bytes, one
# for each voice.
: voicedata create 0 , 0 c,
does> voice c@ + ;
voicedata octave
voicedata ctl
voicedata tie
voicedata default-pause
voicedata pause

:asm voice7 # voice c@ 7 *
dex, dex, 0 lda,# 1 sta,x
voice lda,
+branch bne,
0 sta,x ;asm
:+ 1 cmp,#
+branch bne,
7 lda,# 0 sta,x ;asm
:+ 7 2* lda,# 0 sta,x ;asm

: sid-pulse [ sid 2 + literal ] voice7 + ! ;
: ctl! dup [ sid 4 + literal ] voice7 + c! ctl c! ;

: sid-cutoff [ sid 15 + literal ] ! ;
: sid-flt [ sid 17 + literal ] c! ;
: sid-vol! [ sid 18 + literal ] c! ;

( write adsr )
: srad! ( SR AD -- ) [ sid 5 + literal ] voice7 + ! ;

: note! ( i -- )
2* ['] freqtab + @ sid voice7 + ! ;

: gate-on ctl c@ 1 or ctl! ;
: gate-off ctl c@ fe and ctl! ;

here 0 , 0 , 0 , value .str
here 0 , 0 , 0 , value .strlen
:asm str
dex, dex,
voice lda, asl,a
.str ff and adc,# 0 sta,x
.str 100/ lda,# 0 adc,# 1 sta,x ;asm
:asm strlen
dex, dex,
voice lda, asl,a
.strlen ff and adc,# 0 sta,x
.strlen 100/ lda,# 0 adc,# 1 sta,x ;asm

:asm strlen@
dex, dex,
voice lda, asl,a
.strlen ff and adc,# 0 sta,x
.strlen 100/ lda,# 0 adc,# 1 sta,x 

# @
    0   lda,(x)
    tay,
    0   inc,x
    +branch bne,
    1   inc,x
:+
    0   lda,(x)
    1   sta,x
    0   sty,x
;asm

:asm str@c@
dex, dex,
voice lda, asl,a
.str ff and adc,# 0 sta,x
.str 100/ lda,# 0 adc,# 1 sta,x 

# @
    0   lda,(x)
    tay,
    0   inc,x
    +branch bne,
    1   inc,x
:+
    0   lda,(x)
    1   sta,x
    0   sty,x

# c@
    0   lda,(x)
    0   sta,x
    0   ldy,#
    1   sty,x
;asm

: str-pop 
1 str +! ffff strlen +! ;

: strget strlen@ if str@c@ dup else 0 then ;

: read-num
0 begin str@c@ [char] 0 - dup a < strlen@ and while
swap a * + str-pop repeat drop ;

:asm notetab ( char -- notediff )
0 lda,x
key c cmp,# +branch bne,
0 lda,# 0 sta,x ;asm
:+ key d cmp,# +branch bne,
2 lda,# 0 sta,x ;asm
:+ key e cmp,# +branch bne,
4 lda,# 0 sta,x ;asm
:+ key f cmp,# +branch bne,
5 lda,# 0 sta,x ;asm
:+ key g cmp,# +branch bne,
7 lda,# 0 sta,x ;asm
:+ key a cmp,# +branch bne,
9 lda,# 0 sta,x ;asm
:+ key b cmp,# +branch bne,
b lda,# 0 sta,x ;asm
:+ 7f lda,# 0 sta,x ;asm

: str2note ( -- note )
notetab str-pop
# sharp/flat
strget if case
[char] + of 1+ str-pop endof
[char] - of 1- str-pop endof
endcase then ;

: read-pause
read-num ?dup if 60 swap / 1- else
default-pause c@ then
strget if [char] . = if
str-pop dup 2/ + then then ;

: read-default-pause
read-pause default-pause c! ;

: play-note ( -- )
strget if
str2note dup 7f = if drop else
octave c@ + note! gate-on then
read-pause pause c! then ;

: o str-pop str@c@ [char] 0 - str-pop c * octave c! ;
: do-commands ( -- done )
strget if case
[char] l of str-pop read-default-pause recurse endof
[char] o of o recurse endof
[char] < of str-pop fff4 octave +! recurse endof
[char] > of str-pop c octave +! recurse endof
[char] & of str-pop 1 tie c! recurse endof
[char] v of str-pop read-num sid-vol! recurse endof
d of str-pop recurse endof
bl of str-pop recurse endof
endcase then ;

: stop-note
tie c@ if 0 tie c! else gate-off then ;
: voicetick
pause c@ ?dup if 
1- dup pause c! 0= if stop-note then 
else
do-commands 
play-note 
then ;

: tick 
0 voice c! voicetick
1 voice c! voicetick
2 voice c! voicetick ;

:asm wait 0 lda,x
:- a2 cmp, -branch beq,
0 inc,x ;asm

: play 
a2 c@ begin strlen@ while
tick
wait
sid d400 19 cmove
repeat drop ;

: init-voices
f sid-vol!
3 0 do i voice ! 0 pause c!
strlen ! str ! 10 ctl! 891a srad!
loop ;

: play-melody ( str strlen -- )
d400 sid 19 cmove
init-voices play 
3 0 do i voice ! gate-off loop
sid d400 19 cmove ;

: music
s" l16o3f8o4crcrcro3f8o4crcrcro3f8o4crcrcro3f8o4crcro3cre8o4crcrcro3e8o4crcrcro3e8o4crcrcro3e8o4crcro3c8f8o4crcrcro3f8o4crcrcro3f8o4crcrcro3f8o4crcro3cro3e8o4crcrcro3e8o4crcrcro3e8o4crcrcro3e8o4crcrc8o3drardraro2gro3gro2gro3grcro4cro3cro4cro2aro3aro2aro3aro3drardraro2gro3gro2gro3grcro4cro3cro4cro2aro3aro2aro3aro3drardraro2gro3gro2gro3grcro4cro3cro4cro2aro3aro2aro3aro3drararrrdrararrrcrbrbrrrcrbrbrrrerarrrarerarrrarerg+rg+rg+rg+rrre&er"
s" l16o5frarb4frarb4frarbr>erd4<b8>cr<brgre2&e8drergre2&e4frarb4frarb4frarbr>erd4<b8>crer<brg2&g8brgrdre2&e4r1r1frgra4br>crd4e8frg2&g4r1r1<f8era8grb8ar>c8<br>d8cre8drf8er<b>cr<ab1&b2r4e&e&er"
s" l16r1r1r1r1r1r1r1r1o4drerf4grarb4>c8<bre2&e4drerf4grarb4>c8dre2&e4<drerf4grarb4>c8<bre2&e4d8crf8erg8fra8grb8ar>c8<br>d8crefrde1&e2r4"
play-melody ;
