;{{{ The MIT License
;
;Copyright (c) 2008 Johan Kotlinski
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
; >R R> R@ BL PICK DEPTH WITHIN FILL BASE 2*

    +BACKLINK "drop", 4 | F_IMMEDIATE
DROP
    lda STATE
    bne +
    inx
    rts
+   lda #OP_INX
    jmp compile_a

    +BACKLINK "swap", 4
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

    +BACKLINK "dup", 3
DUP
    dex
    lda	MSB + 1, x
    sta	MSB, x
    lda	LSB + 1, x
    sta	LSB, x
    rts

    +BACKLINK "?dup", 4
QDUP
    lda MSB, x
    ora LSB, x
    bne DUP
    rts

    +BACKLINK "over", 4
OVER
    dex
    lda	MSB + 2, x
    sta	MSB, x
    lda	LSB + 2, x
    sta	LSB, x
    rts

    +BACKLINK "2dup", 4
TWODUP
    jsr OVER
    jmp OVER

    +BACKLINK "1+", 2
ONEPLUS
    inc LSB, x
    bne +
    inc MSB, x
+   rts

    +BACKLINK "1-", 2
ONEMINUS
    lda LSB, x
    bne +
    dec MSB, x
+   dec LSB, x
    rts

    +BACKLINK "+", 1
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

    +BACKLINK "=", 1
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
    +BACKLINK "0=", 2
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

    +BACKLINK "and", 3
    lda	MSB, x
    and MSB + 1, x
    sta MSB + 1, x

    lda	LSB, x
    and LSB + 1, x
    sta LSB + 1, x

    inx
    rts

    +BACKLINK "!", 1
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

    +BACKLINK "@", 1
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

    +BACKLINK "c!", 2
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

    +BACKLINK "c@", 2
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

    +BACKLINK "count", 5
COUNT
    jsr DUP
    jsr ONEPLUS
    jsr SWAP
    jmp FETCHBYTE

    +BACKLINK ">", 1
GREATER_THAN
    ldy #0
    sec
    lda LSB,x
    sbc LSB+1,x
    lda MSB,x
    sbc MSB+1,x
    bpl +
    dey
+   inx
    sty LSB,x
    sty MSB,x
    rts

    +BACKLINK "<", 1
LESS_THAN
    jsr SWAP
    jmp GREATER_THAN

    +BACKLINK "max", 3
MAX
    jsr TWODUP
    jsr LESS_THAN
    jsr ZBRANCH
    !word +
    jsr SWAP
+   inx
    rts

    +BACKLINK "min", 3
MIN
    jsr TWODUP
    jsr GREATER_THAN
    jsr ZBRANCH
    !word +
    jsr SWAP
+   inx
    rts

    +BACKLINK "tuck", 4
TUCK ; ( x y -- y x y )
    jsr SWAP
    jmp OVER

    ; Exempt from TCE as top of return stack must contain a return address.
    +BACKLINK ">r", 2 | F_NO_TAIL_CALL_ELIMINATION
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

    ; Exempt from TCE as top of return stack must contain a return address.
    +BACKLINK "r>", 2 | F_NO_TAIL_CALL_ELIMINATION
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

    ; Exempt from TCE as top of return stack must contain a return address.
    +BACKLINK "r@", 2 | F_NO_TAIL_CALL_ELIMINATION
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

    +BACKLINK "bl", 2
BL
    +VALUE	K_SPACE

    +BACKLINK "pick", 4
    txa
    sta + + 1
    clc
    adc LSB,x
    tax
    inx
    lda LSB,x
    ldy MSB,x
+   ldx #0
    sta LSB,x
    sty MSB,x
    rts

    +BACKLINK "depth", 5
    txa
    eor #$ff
    tay
    iny
    dex
    sty LSB,x
    lda #0
    sta MSB,x
    rts

    +BACKLINK "within", 6
WITHIN ; ( test low high -- flag )
    jsr OVER
    jsr MINUS
    jsr TO_R
    jsr MINUS
    jsr R_TO
    jmp U_LESS

; FILL ( start len char -- )
    +BACKLINK "fill", 4
FILL
    lda	LSB, x
    tay
    lda	LSB + 2, x
    sta	.fdst
    lda	MSB + 2, x
    sta	.fdst + 1
    lda	LSB + 1, x
    eor	#$ff
    sta	W
    lda	MSB + 1, x
    eor	#$ff
    sta	W + 1
    inx
    inx
    inx
-
    inc	W
    bne	+
    inc	W + 1
    bne	+
    rts
+
.fdst = * + 1
    sty	PLACEHOLDER_ADDRESS ; replaced with start

    ; advance
    inc	.fdst
    bne	-
    inc	.fdst + 1
    jmp	-

    +BACKLINK "base", 4
    +VALUE	BASE
BASE
    !word 16

    +BACKLINK "2*", 2
    asl LSB, x
    rol MSB, x
    rts
