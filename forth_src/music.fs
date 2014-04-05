

0 value voice
0 value voice7

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
: voicedata create 3 allot 
does> voice + ;
voicedata octave
voicedata ctl

: o> 1 octave +! ;
: o< ffff octave +! ;

: voice! dup to voice 7 * to voice7 ;

: freq! d400 voice7 + ! ;
: sid-pulse d402 voice7 + ! ;
: ctl! dup ctl c! d404 voice7 + c! ;

: sid-cutoff d415 ! ;
: sid-flt d417 c! ;
: sid-vol d418 c! ;

( write adsr )
: srad! ( SR AD -- ) d405 voice7 + ! ;

voice hidden

: note! ( i -- )
2* ['] freqtab + @ freq! ;

: gate! ( onoff -- )
ctl c@ swap if 1 or else fe and then
ctl! ;

: music

# init
f sid-vol
0 voice! 10 ctl! 9 srad!
1 voice! 10 ctl! 9 srad!
2 voice! 10 ctl! 9 srad!

5f 0 do
0 voice!
0 gate! i note! 1 gate!
1 voice!
0 gate! i c + note! 1 gate!
2 voice!
0 gate! i 18 + note! 1 gate!
200 0 do loop
loop ;
music

( :asm burst
f lda, 0 ldy,
d418 sta, d418 sty, d418 sta, d418 sty,
d418 sta, d418 sty, d418 sta, d418 sty,
d418 sta, d418 sty, d418 sta, d418 sty,
d418 sta, d418 sty, d418 sta, d418 sty,
d418 sta, d418 sty, d418 sta, d418 sty,
d418 sta, d418 sty, d418 sta, d418 sty,
;asm

:asm digi-init
ff lda,# d406 sta, d40d sta, d414 sta,
49 lda,# d404 sta, d40b sta, d412 sta,
;asm

: digi digi-init begin burst again ; )

