; DROP SWAP DUP ?DUP NIP OVER 2DUP 1+ 1- + = 0= AND ! @ C! C@ COUNT < > MAX MIN
; TUCK >R R> R@ BL PICK DEPTH WITHIN ERASE FILL BASE 2* ROT +! 100/

    +BACKLINK "drop", 4 | F_IMMEDIATE
DROP
    lda STATE
    bne +
    inx
    rts
+   lda #OP_INX
compile_a
    dex
    sta LSB, x
    jmp CCOMMA

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

    +BACKLINK "nip", 3
NIP ; ( a b -- b )
    jsr SWAP
    inx
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
    lda LSB, x
    bne +
    lda MSB, x
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
    ldy LSB,x
    lda MSB,x
    sta + + 2
    lda	LSB+1,x
+   sta PLACEHOLDER_ADDRESS,y ; replaced with addr
    inx
    inx
    rts

    +BACKLINK "c@", 2
FETCHBYTE
    ldy LSB,x
    lda MSB,x
    sta + + 2
+   lda PLACEHOLDER_ADDRESS,y ; replaced with addr
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

    +BACKLINK "<", 1
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

    +BACKLINK ">", 1
GREATER_THAN
    jsr SWAP
    jmp LESS_THAN

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

; ERASE ( start len -- )
    +BACKLINK "erase", 5
    ldy #0
    jmp ERASE_

; FILL ( start len char -- )
    +BACKLINK "fill", 4
FILL
    lda	LSB, x
    tay
    inx
ERASE_
    lda	LSB + 1, x
    sta	.fdst
    lda	MSB + 1, x
    sta	.fdst + 1
    lda	LSB, x
    eor	#$ff
    sta	W
    lda	MSB, x
    eor	#$ff
    sta	W + 1
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

    +BACKLINK "rot", 3 ; ( a b c -- b c a )
    ldy MSB+2, x
    lda MSB+1, x
    sta MSB+2, x
    lda MSB  , x
    sta MSB+1, x
    sty MSB  , x
    ldy LSB+2, x
    lda LSB+1, x
    sta LSB+2, x
    lda LSB  , x
    sta LSB+1, x
    sty LSB  , x
    rts

    +BACKLINK "+!", 2 ; ( num addr -- )
    lda LSB,x
    sta W
    lda MSB,x
    sta W+1
    ldy #0
    clc
    lda (W),y
    adc LSB+1,x
    sta (W),y
    iny
    lda (W),y
    adc MSB+1,x
    sta (W),y
    inx
    inx
    rts

    +BACKLINK "100/", 4 ; ( num addr -- )
    lda MSB,x
    sta LSB,x
    lda #0
    sta MSB,x
    rts
