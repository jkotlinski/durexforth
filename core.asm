;{{{ The MIT License
;
;Copyright (c) 2008 Johan Kotlinski, Mats Andren
;
;Permission is hereby granted, free of charge, to any person obtaining a copy
;of this software and associated documentation files (the "Software"), to deal
;in the Software without restriction, including without limitation the rights
;to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
;copies of the Software, and to permit persons to whom the Software is
;furnished to do so, subject to the following conditions:
;
;The above copyright notice and this permission notice shall be included in
;all copies or substantial portions of the Software.
;
;THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
;IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
;FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
;AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
;LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
;OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
;THE SOFTWARE. }}}

; DROP SWAP DUP ?DUP OVER 2DUP 1+ 1- + = 0= AND ! @ C! C@ COUNT > < MAX MIN TUCK
; >R R> R@ BL PICK DEPTH 0 1 WITHIN

    +BACKLINK
    !byte	4 | F_IMMEDIATE
    !text	"drop"
DROP
    lda STATE
    bne +
    inx
    rts
+   lda #OP_INX
    jmp compile_a

    +BACKLINK
    !byte	4
    !text	"swap"
SWAP
    ldy	MSB, x
    lda	MSB + 1, x
    sta MSB, x
    sty	MSB + 1, x

    ldy	LSB, x
    lda	LSB + 1, x
    sta LSB, x
    sty	LSB + 1, x
    rts

    +BACKLINK
    !byte	3
    !text	"dup"
DUP
    dex
    lda	MSB + 1, x
    sta	MSB, x
    lda	LSB + 1, x
    sta	LSB, x
    rts

    +BACKLINK
    !byte 4
    !text "?dup"
QDUP
    lda MSB, x
    ora LSB, x
    bne DUP
    rts

    +BACKLINK
    !byte	4
    !text	"over"
OVER
    dex
    lda	MSB + 2, x
    sta	MSB, x
    lda	LSB + 2, x
    sta	LSB, x
    rts

    +BACKLINK
    !byte	4
    !text	"2dup"
TWODUP
    jsr OVER
    jmp OVER

    +BACKLINK
    !byte	2
    !text	"1+"
ONEPLUS
    inc LSB, x
    bne +
    inc MSB, x
+   rts

    +BACKLINK
    !byte	2
    !text	"1-"
ONEMINUS
    lda LSB, x
    bne +
    dec MSB, x
+   dec LSB, x
    rts

    +BACKLINK
    !byte	1
    !text	"+"
PLUS
    lda	LSB, x
    clc
    adc LSB + 1, x
    sta	LSB + 1, x

    lda	MSB, x
    adc MSB + 1, x
    sta MSB + 1, x

    inx
    rts

    +BACKLINK
    !byte	1
    !text	"="
EQUAL
    ldy #0
    lda	LSB, x
    cmp	LSB + 1, x
    bne	+
    lda	MSB, x
    cmp	MSB + 1, x
    bne	+
    dey
+   inx
    sty MSB, x
    sty	LSB, x
    rts

; 0=
    +BACKLINK
    !byte	2
    !text	"0="
ZEQU
    ldy #0
    lda MSB, x
    bne +
    lda LSB, x
    bne +
    dey
+   sty MSB, x
    sty LSB, x
    rts

    +BACKLINK
    !byte	3
    !text	"and"
    lda	MSB, x
    and MSB + 1, x
    sta MSB + 1, x

    lda	LSB, x
    and LSB + 1, x
    sta LSB + 1, x

    inx
    rts

    +BACKLINK
    !byte	1
    !text	"!"
STORE
    lda LSB, x
    sta W
    lda MSB, x
    sta W + 1

    ldy #0
    lda	LSB+1, x
    sta (W), y
    iny
    lda	MSB+1, x
    sta	(W), y

    inx
    inx
    rts

    +BACKLINK
    !byte	1
    !text	"@"
FETCH
    lda LSB,x
    sta W
    lda MSB,x
    sta W+1

    ldy #0
    lda	(W),y
    sta LSB,x
    iny
    lda	(W),y
    sta MSB,x
    rts

    +BACKLINK
    !byte	2
    !text	"c!"
STOREBYTE
    lda LSB,x
    sta + + 1
    lda MSB,x
    sta + + 2
    lda	LSB+1,x
+   sta PLACEHOLDER_ADDRESS ; replaced with addr
    inx
    inx
    rts

    +BACKLINK
    !byte	2
    !text	"c@"
FETCHBYTE
    lda LSB,x
    sta + + 1
    lda MSB,x
    sta + + 2
+   lda PLACEHOLDER_ADDRESS ; replaced with addr
    sta LSB,x
    lda #0
    sta MSB,x
    rts

    +BACKLINK
    !byte   5
    !text   "count"
COUNT
    jsr DUP
    jsr ONEPLUS
    jsr SWAP
    jmp FETCHBYTE

    +BACKLINK
    !byte 1
    !text	"<"
LESS_THAN
    ldy #0
    sec
    lda LSB+1,x
    sbc LSB,x
    lda MSB+1,x
    sbc MSB,x
    bvc +
    eor #$80
+   bpl +
    dey
+   inx
    sty LSB,x
    sty MSB,x
    rts

    +BACKLINK
    !byte 1
    !text	">"
GREATER_THAN
    jsr SWAP
    jmp LESS_THAN

    +BACKLINK
    !byte 3
    !text "max"
MAX
    jsr TWODUP
    jsr LESS_THAN
    jsr ZBRANCH
    !word +
    jsr SWAP
+   inx
    rts

    +BACKLINK
    !byte 3
    !text "min"
MIN
    jsr TWODUP
    jsr GREATER_THAN
    jsr ZBRANCH
    !word +
    jsr SWAP
+   inx
    rts

    +BACKLINK
    !byte	4
    !text	"tuck"
TUCK ; ( x y -- y x y ) 
    jsr SWAP
    jmp OVER

    +BACKLINK
    !byte	2 | F_NO_TAIL_CALL_ELIMINATION
    !text	">r"
TO_R
    pla
    sta W
    pla
    sta W+1
    inc W
    bne +
    inc W+1
+
    lda MSB,x
    pha
    lda LSB,x
    pha
    inx
    jmp (W)

    +BACKLINK
    !byte	2 | F_NO_TAIL_CALL_ELIMINATION
    !text	"r>"
R_TO
    pla
    sta W
    pla
    sta W+1
    inc W
    bne +
    inc W+1
+
    dex
    pla
    sta LSB,x
    pla
    sta MSB,x
    jmp (W)

    +BACKLINK
    !byte	2 | F_NO_TAIL_CALL_ELIMINATION
    !text	"r@"
R_FETCH
    txa
    tsx
    ldy $103,x
    sty W
    ldy $104,x
    tax
    dex
    sty MSB,x
    lda W
    sta LSB,x
    rts

    +BACKLINK
    !byte 2
    !text	"bl"
BL
    +VALUE	K_SPACE

    +BACKLINK
    !byte   4
    !text   "pick" ; ( x_u ... x_1 x_0 u -- x_u ... x_1 x_0 x_u )
    stx tmp_x
    txa
    clc
    adc LSB,x
    tax
    inx
    lda LSB,x
    ldy MSB,x
    ldx tmp_x
    sta LSB,x
    sty MSB,x
    rts

    +BACKLINK
    !byte 5
    !text	"depth"
    txa
    eor #$ff
    tay
    iny
    dex
    sty LSB,x
    lda #0
    sta MSB,x
    rts

    +BACKLINK
    !byte 1
    !text	"0"
ZERO
    lda	#0
    tay
    jmp pushya

    +BACKLINK
    !byte 1
    !text	"1"
ONE
    +VALUE 1

    +BACKLINK
    !byte 6
    !text   "within"
WITHIN ; ( test low high -- flag )
    jsr OVER
    jsr MINUS
    jsr TO_R
    jsr MINUS
    jsr R_TO
    jmp U_LESS
