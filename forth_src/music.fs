

0 value voice

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
does> voice + ;
voicedata octave
voicedata ctl

: o c * octave c! ;
: o> octave c@ c + octave c! ;
: o< octave c@ c - octave c! ;

: voice7 voice 7 * ;

: freq! d400 voice7 + ! ;
: sid-pulse d402 voice7 + ! ;
: ctl! dup d404 voice7 + c! ctl c! ;

: sid-cutoff d415 ! ;
: sid-flt d417 c! ;
: sid-vol! d418 c! ;

( write adsr )
: srad! ( SR AD -- ) d405 voice7 + ! ;

voice hidden

: note! ( i -- )
2* ['] freqtab + @ freq! ;

: gate-on ctl c@ 1 or ctl! ;
: gate-off ctl c@ fe and ctl! ;

: voicedata2 create 0 , 0 , 0 ,
does> voice 2* + ;
voicedata2 str
voicedata2 strlen
voicedata tie
: str-pop 
str @ c@ emit
1 str +! ffff strlen +! ;

: strget strlen @ if str @ c@ dup else 0 then ;

: isnum ( ch -- v )
dup [char] 0 >= swap [char] 9 <= and ;

: read-num
0 begin strlen @ str @ c@ isnum and while
a * str @ c@ [char] 0 - +
str-pop repeat ;

: str2note ( -- note )
case 
[char] c of 0 endof
[char] d of 2 endof
[char] e of 4 endof
[char] f of 5 endof
[char] g of 7 endof
[char] a of 9 endof
[char] b of b endof
[char] r of 7f endof
endcase str-pop
# sharp/flat
strget if case
[char] + of 1+ str-pop endof
[char] - of 1- str-pop endof
endcase then ;

voicedata default-pause
voicedata pause
: read-pause
read-num ?dup if 60 swap / else
default-pause c@ then
strget if [char] . = if
str-pop dup 2/ + then then ;

: read-default-pause
read-pause default-pause c! ;

: play-note ( -- )
strget if
tie c@ if 0 tie c! else gate-off then
str2note dup 7f = if drop else
octave c@ + note! gate-on then
read-pause 1- pause c! then ;

: do-commands ( -- done )
strget if case
[char] l of str-pop read-default-pause recurse endof
[char] o of str-pop read-num o recurse endof
[char] < of str-pop o< recurse endof
[char] > of str-pop o> recurse endof
[char] & of str-pop 1 tie c! recurse endof
[char] v of str-pop read-num sid-vol! recurse endof
d of str-pop recurse endof
bl of str-pop recurse endof
endcase then ;

: voicetick
pause c@ if ffff pause +! else
do-commands play-note then ;

: tick 
0 to voice voicetick
1 to voice voicetick
2 to voice voicetick ;

: play 
a2 c@ begin strlen @ while
1 d020 c!
tick
0 d020 c!
begin dup a2 c@ <> until
1+ ff and
repeat drop ;

: init-voices
3 0 do i to voice 0 pause c!
strlen ! str ! 10 ctl! 8919 srad!
loop ;

: play-melody ( str strlen -- )
init-voices play 0 sid-vol! ;

: music
s" v10l16o3f8o4crcrcro3f8o4crcrcro3f8o4crcrcro3f8o4crcro3cre8o4crcrcro3e8o4crcrcro3e8o4crcrcro3e8o4crcro3c8f8o4crcrcro3f8o4crcrcro3f8o4crcrcro3f8o4crcro3cro3e8o4crcrcro3e8o4crcrcro3e8o4crcrcro3e8o4crcrc8o3drardraro2gro3gro2gro3grcro4cro3cro4cro2aro3aro2aro3aro3drardraro2gro3gro2gro3grcro4cro3cro4cro2aro3aro2aro3aro3drardraro2gro3gro2gro3grcro4cro3cro4cro2aro3aro2aro3aro3drararrrdrararrrcrbrbrrrcrbrbrrrerarrrarerarrrarerg+rg+rg+rg+rrre&v5er"
s" v14l16o5frarb4frarb4frarbr>erd4<b8>cr<brgre2&e8drergre2&e4frarb4frarb4frarbr>erd4<b8>crer<brg2&g8brgrdre2&e4r1r1frgra4br>crd4e8frg2&g4r1r1<f8era8grb8ar>c8<br>d8cre8drf8er<b>cr<ab1&b2r4e&v8e&v5er"
s" v14l16r1r1r1r1r1r1r1r1o4drerf4grarb4>c8<bre2&e4drerf4grarb4>c8dre2&e4<drerf4grarb4>c8<bre2&e4d8crf8erg8fra8grb8ar>c8<br>d8crefrde1&e2r4"
play-melody ;
