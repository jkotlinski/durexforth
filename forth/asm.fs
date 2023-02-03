: 1mi create c, does> c@ c, ;
: 2mi create c, does> c@ c, c, ;
: 3mi create c, does> c@ c, , ;
: 23mi create , does>
over $ff00 and if c@ c, ,
else 1+ c@ c, c, then ;

$69 2mi adc,#
$656d 23mi adc,
$757d 23mi adc,x
$79 3mi adc,y
$61 2mi adc,(x)
$71 2mi adc,(y)

$29 2mi and,#
$252d 23mi and,
$353d 23mi and,x
$39 3mi and,y
$21 2mi and,(x)
$31 2mi and,(y)

$a 1mi asl,a
$060e 23mi asl,
$161e 23mi asl,x

$90 2mi bcc,
$b0 2mi bcs,
$f0 2mi beq,

$242c 23mi bit,

$30 2mi bmi,
$d0 2mi bne,
$10 2mi bpl,
$0 1mi brk,
$50 2mi bvc,
$70 2mi bvs,
$18 1mi clc,
$d8 1mi cld,
$58 1mi cli,
$b8 1mi clv,

$c9 2mi cmp,#
$c5cd 23mi cmp,
$d5dd 23mi cmp,x
$d9 3mi cmp,y
$c1 2mi cmp,(x)
$d1 2mi cmp,(y)

$e0 2mi cpx,#
$e4ec 23mi cpx,

$c0 2mi cpy,#
$c4cc 23mi cpy,

$c6ce 23mi dec,
$d6de 23mi dec,x

$ca 1mi dex,
$88 1mi dey,

$49 2mi eor,#
$454d 23mi eor,
$555d 23mi eor,x
$59 3mi eor,y
$41 2mi eor,(x)
$51 2mi eor,(y)

$e6ee 23mi inc,
$f6fe 23mi inc,x

$e8 1mi inx,
$c8 1mi iny,

$4c 3mi jmp,
$6c 3mi (jmp),

$20 3mi jsr,

$a9 2mi lda,#
$a5ad 23mi lda,
$b5bd 23mi lda,x
$b9 3mi lda,y
$a1 2mi lda,(x)
$b1 2mi lda,(y)

$a2 2mi ldx,#
$a6ae 23mi ldx,
$b6be 23mi ldx,y

$a0 2mi ldy,#
$a4ac 23mi ldy,
$b4bc 23mi ldy,x

$4a 1mi lsr,a
$464e 23mi lsr,
$565e 23mi lsr,x

$ea 1mi nop,

$9 2mi ora,#
$050d 23mi ora,
$151d 23mi ora,x
$19 3mi ora,y
$1 2mi ora,(x)
$11 2mi ora,(y)

$48 1mi pha,
$8 1mi php,
$68 1mi pla,
$28 1mi plp,

$2a 1mi rol,a
$262e 23mi rol,
$363e 23mi rol,x

$6a 1mi ror,a
$666e 23mi ror,
$767e 23mi ror,x

$40 1mi rti,
$60 1mi rts,

$e9 2mi sbc,#
$e5ed 23mi sbc,
$f5fd 23mi sbc,x
$f9 3mi sbc,y
$e1 2mi sbc,(x)
$f1 2mi sbc,(y)

$38 1mi sec,
$f8 1mi sed,
$78 1mi sei,

$858d 23mi sta,
$959d 23mi sta,x
$99 3mi sta,y
$81 2mi sta,(x)
$91 2mi sta,(y)

$868e 23mi stx,
$96 2mi stx,y

$848c 23mi sty,
$94 2mi sty,x

$aa 1mi tax,
$a8 1mi tay,
$ba 1mi tsx,
$8a 1mi txa,
$9a 1mi txs,
$98 1mi tya,

\ illegal opcodes
$cb 2mi sbx,#

: code header ;
: ;code rts, ;

( usage:
foo lda,
+branch beq,
bar inc,
:+ )
: +branch ( -- a ) here 0 ;
: :+ ( a -- )
here over 2+ - swap 1+ c! ;

( usage:
:- $d014 lda, f4 cmp,#
-branch bne, )
: :- here ;
: -branch ( absaddr -- reladdr )
here 2+ - ;
