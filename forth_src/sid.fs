

0 value voice

: sid-voice 7 * to voice ;

: sid-freq d400 voice + ! ;
: sid-pulse d402 voice + ! ;
: sid-ctl d404 voice + c! ;
: sid-ad d405 voice + c! ;
: sid-sr d406 voice + c! ;

: sid-cutoff d415 ! ;
: sid-flt d417 c! ;
: sid-vol d418 c! ;

voice hidden

:asm burst
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

: digi digi-init begin burst again ;

