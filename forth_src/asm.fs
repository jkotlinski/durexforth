

: :asm header ;
: ;asm 4c c, next , ; # jmp next

: 1mi create c, does> c@ c, ;
: 2mi create c, does> c@ c, c, ;
: 3mi create c, does> c@ c, , ;
: 23mi create c, c, does>
over ff00 and if c@ c, ,
else 1+ c@ c, c, then ;

69 2mi adc,#
65 6d 23mi adc,
75 7d 23mi adc,x
79 3mi adc,y
61 2mi adc,(x)
71 2mi adc,(y)

29 2mi and,#
25 2d 23mi and,
35 3d 23mi and,x
39 3mi and,y
21 2mi and,(x)
31 2mi and,(y)

a 1mi asla,
6 e 23mi asl,
16 1e 23mi asl,x

90 2mi bcc,
b0 2mi bcs,
f0 2mi beq,

24 2c 23mi bit,

30 2mi bmi,
d0 2mi bne,
10 2mi bpl,
0 1mi brk,
50 2mi bvc,
70 2mi bvs,
18 1mi clc,
d8 1mi cld,
58 1mi cli,
b8 1mi clv,

c9 2mi cmp,#
c5 cd 23mi cmp,
d5 dd 23mi cmp,x
d9 3mi cmp,y
c1 2mi cmp,(x)
d1 2mi cmp,(y)

e0 2mi cpx,#
e4 ec 23mi cpx,

c0 2mi cpy,#
c4 cc 23mi cpy,

c6 ce 23mi dec,
d6 de 23mi dec,x

ca 1mi dex,
88 1mi dey,

49 2mi eor,#
45 4d 23mi eor,
55 5d 23mi eor,x
59 3mi eor,y
41 2mi eor,(x)
51 2mi eor,(y)

e6 ee 23mi inc,
f6 fe 23mi inc,x

e8 1mi inx,
c8 1mi iny,

4c 3mi jmp,
6c 3mi jmp,()

20 3mi jsr,

a9 2mi lda,#
a5 ad 23mi lda,
b5 bd 23mi lda,x
b9 3mi lda,y
a1 2mi lda,(x)
b1 2mi lda,(y)

a2 2mi ldx,#
a6 ae 23mi ldx,
b6 be 23mi ldx,y

a0 2mi ldy,#
a4 ac 23mi ldy,
b4 bc 23mi ldy,x

4a 1mi lsra,
46 4e 23mi lsr,
56 5e 23mi lsr,x

ea 1mi nop,

9 2mi ora,#
5 d 23mi ora,
15 1d 23mi ora,x
19 3mi ora,y
1 2mi ora,(x)
11 2mi ora,(y)

48 1mi pha,
8 1mi php,
68 1mi pla,
28 1mi plp,

2a 1mi rola,
26 2e 23mi rol,
36 3e 23mi rol,x

6a 1mi rora,
66 6e 23mi ror,
76 7e 23mi ror,x

40 1mi rti,
60 1mi rts,

e9 2mi sbc,#
e5 ed 23mi sbc,
f5 fd 23mi sbc,x
f9 3mi sbc,y
e1 2mi sbc,(x)
f1 2mi sbc,(y)

38 1mi sec,
f8 1mi sed,
78 1mi sei,

85 8d 23mi sta,
95 9d 23mi sta,x
99 3mi sta,y
81 2mi sta,(x)
91 2mi sta,(y)

86 8e 23mi stx,
96 2mi stx,y

84 8c 23mi sty,
94 2mi sty,x

aa 1mi tax,
a8 1mi tay,
ba 1mi tsx,
8a 1mi txa,
9a 1mi txs,
98 1mi tya,

( usage:
foo lda,
+branch beq,
bar inc,
:+ )
: +branch ( -- a ) here @ 0 ;
: :+ ( a -- )
here @ over 2+ - swap 1+ c! ;

( usage:
:- d014 lda, f4 cmp,#
-branch bne, )
: :- here @ ;
: -branch ( absaddr -- reladdr )
here @ 2+ - ;
