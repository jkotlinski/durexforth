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

; UM/MOD by Garth Wilson
; http://6502.org/source/integers/ummodfix/ummodfix.htm

; U< - UM* UM/MOD M+ INVERT NEGATE ABS * DNEGATE M* 0< S>D FM/MOD /MOD UD/MOD

    +BACKLINK "u<", 2
U_LESS
    ldy #0
    lda	MSB, x
    cmp	MSB + 1, x
    bcc .false
    bne	.true
    ; ok, msb are equal...
    lda	LSB + 1, x
    cmp	LSB, x
    bcs	.false
.true
    dey
.false
    inx
    sty	MSB, x
    sty	LSB, x
    rts

    +BACKLINK "-", 1
MINUS
    lda	LSB + 1, x
    sec
    sbc LSB, x
    sta	LSB + 1, x

    lda MSB + 1, x
    sbc MSB, x
    sta MSB + 1, x

    inx
    rts

product = W

    +BACKLINK "um*", 3
; wastes W, W2, y
U_M_STAR
    lda #$00
    sta product+2 ; clear upper bits of product
    sta product+3
    ldy #$10 ; set binary count to 16
.shift_r
    lsr MSB + 1, x ; multiplier+1 ; divide multiplier by 2
    ror LSB + 1, x ; multiplier
    bcc rotate_r
    lda product+2 ; get upper half of product and add multiplicand
    clc
    adc LSB, x ; multiplicand
    sta product+2
    lda product+3
    adc MSB, x ; multiplicand+1
rotate_r
    ror ; rotate partial product
    sta product+3
    ror product+2
    ror product+1
    ror product
    dey
    bne .shift_r

    lda	product
    sta	LSB + 1, x
    lda	product + 1
    sta	MSB + 1, x
    lda	product + 2
    sta	LSB, x
    lda	product + 3
    sta	MSB, x
    rts

    +BACKLINK "um/mod", 6
UM_DIV_MOD
; ( lsw msw divisor -- rem quot )
; Wastes W, lo(W2)
        N = W
        SEC
        LDA     LSB+1,X     ; Subtract hi cell of dividend by
        SBC     LSB,X     ; divisor to see if there's an overflow condition.
        LDA     MSB+1,X
        SBC     MSB,X
        BCS     oflo    ; Branch if /0 or overflow.

        LDA     #17     ; Loop 17x.
        STA     N       ; Use N for loop counter.
loop:   ROL     LSB+2,X     ; Rotate dividend lo cell left one bit.
        ROL     MSB+2,X
        DEC     N       ; Decrement loop counter.
        BEQ     end     ; If we're done, then branch to end.
        ROL     LSB+1,X     ; Otherwise rotate dividend hi cell left one bit.
        ROL     MSB+1,X
        lda     #0
        sta     N+1
        ROL     N+1     ; Rotate the bit carried out of above into N+1.

        SEC
        LDA     LSB+1,X     ; Subtract dividend hi cell minus divisor.
        SBC     LSB,X
        STA     N+2     ; Put result temporarily in N+2 (lo byte)
        LDA     MSB+1,X
        SBC     MSB,X
        TAY             ; and Y (hi byte).
        LDA     N+1     ; Remember now to bring in the bit carried out above.
        SBC     #0
        BCC     loop

        LDA     N+2     ; If that didn't cause a borrow,
        STA     LSB+1,X     ; make the result from above to
        STY     MSB+1,X     ; be the new dividend hi cell
        bcs     loop    ; and then branch up.

oflo:   LDA     #$FF    ; If overflow or /0 condition found,
        STA     LSB+1,X     ; just put FFFF in both the remainder
        STA     MSB+1,X
        STA     LSB+2,X     ; and the quotient.
        STA     MSB+2,X

end:    INX
        jmp SWAP

    +BACKLINK "m+", 2
M_PLUS
    ldy #0
    lda MSB,x
    bpl +
    dey
+   clc
    lda LSB,x
    adc LSB+2,x
    sta LSB+2,x
    lda MSB,x
    adc MSB+2,x
    sta MSB+2,x
    tya
    adc LSB+1,x
    sta LSB+1,x
    tya
    adc MSB+1,x
    sta MSB+1,x
    inx
    rts

    +BACKLINK "invert", 6
INVERT
    lda MSB, x
    eor #$ff
    sta MSB, x
    lda LSB, x
    eor #$ff
    sta LSB,x
    rts

    +BACKLINK "negate", 6
NEGATE
    jsr INVERT
    jmp ONEPLUS

    +BACKLINK "abs", 3
ABS
    lda MSB,x
    bmi NEGATE
    rts

DABS_STAR           ; ( n1 n2 -- ud1 )
    lda MSB,x       ;   ud1 = abs(n1) * abs(n2)
    eor MSB+1,x     ;   with final sign output in A register
    pha
    jsr ABS
    inx
    jsr ABS
    dex
    jsr U_M_STAR
    pla
    rts

    +BACKLINK "*", 1
    jsr DABS_STAR
    inx
    and #$ff
    bmi NEGATE
    rts

    +BACKLINK "m*", 2
M_STAR
    jsr DABS_STAR
    bmi DNEGATE
    rts

    +BACKLINK "0<", 2
ZERO_LESS
    lda MSB,x
    and #$80
    beq +
    lda #$ff
+   sta MSB,x
    sta LSB,x
    rts

    +BACKLINK "s>d", 3
S_TO_D
    jsr DUP
    jmp ZERO_LESS

    +BACKLINK "fm/mod", 6
FM_DIV_MOD
    lda MSB,x
    sta DIVISOR_SIGN
    bpl +
    jsr NEGATE
    inx
    jsr DNEGATE
    dex
+   lda MSB+1,x
    bpl +
    jsr TUCK
    jsr PLUS
    jsr SWAP
+   jsr UM_DIV_MOD
DIVISOR_SIGN = * + 1
    lda #$ff        ; placeholder
    bpl +
    inx
    jsr NEGATE
    dex
+   rts

    +BACKLINK "/mod", 4
    lda MSB,x
    sta MSB-1,x
    lda LSB,x
    sta LSB-1,x
    inx
    jsr S_TO_D
    dex
    jmp FM_DIV_MOD

    ; (ud1 u2 -- urem udquot)
    +BACKLINK "ud/mod", 6
UD_DIV_MOD
    jsr ZERO
    jsr SWAP
    jmp UT_DIV_MOD

    +BACKLINK "dnegate", 7
DNEGATE
    jsr INVERT
    inx
    jsr INVERT
    dex
    inc LSB+1,x
    bne +
    inc MSB+1,x
    bne +
    inc LSB,x
    bne +
    inc MSB,x
+   rts


product_hi
    !word 0
    !word 0

;    ( d1 u1 u2 -- d2 )
    +BACKLINK "m*/", 3
; wastes W, W2, W3, y
M_STAR_SLASH
    jsr TO_R
    lda MSB + 2,x
    eor MSB,x
    sta .negateprod
    bpl +
    jsr ABS
    inx
    jsr ABS
    inx
    jsr ABS
    dex
    dex
+   jsr ZERO
    lda #$00
    sta product+2 ; clear upper bits of product
    sta product+3
    sta product_hi
    sta product_hi+1
    sta product_hi+2
    sta product_hi+3
    ldy #$20 ; set binary count to 32
    ; ( muld mul )
.dshift_r
    lsr MSB + 2, x
    ror LSB + 2, x
    ror MSB + 3, x ; multiplier+1 ; divide multiplier by 2
    ror LSB + 3, x ; multiplier
    bcc .drotate_r
    lda product_hi ; get upper half of product and add multiplicand
    clc
    adc LSB+1, x ; multiplicand
    sta product_hi
    lda product_hi+1
    adc MSB+1, x
    sta product_hi+1
    lda product_hi+2
    adc LSB, x
    sta product_hi+2
    lda product_hi+3
    adc MSB, x
.drotate_r
    ror ; rotate partial product

    ror product_hi+3
    ror product_hi+2
    ror product_hi+1
    ror product_hi
    ror product+3
    ror product+2
    ror product+1
    ror product
    dey
    bne .dshift_r

    inx

    lda	product
    sta	LSB + 2, x
    lda	product + 1
    sta	MSB + 2, x
    lda	product + 2
    sta	LSB + 1, x
    lda	product + 3
    sta	MSB + 1, x
    lda	product_hi
    sta	LSB, x
    lda	product_hi + 1
    sta	MSB, x
    jsr R_TO
    jsr UT_DIV_MOD ; ( umod udquot )
    inx
    lda MSB, x
    sta MSB + 1, x
    lda LSB, x
    sta LSB + 1, x
    lda MSB - 1, x
    sta MSB, x
    lda LSB - 1, x
    sta LSB, x

.negateprod = * + 1
    lda #$ff  ; placeholder
    bpl +
    jmp DNEGATE
+   rts

UT_DIV_MOD ; (ut1 u2 -- urem udquot )
    lda LSB,x
    sta W3
    lda MSB,x
    sta W3 + 1		; cache the divisor
    jsr UM_DIV_MOD	; divide the highest word
    ; ( u urem uquot )
    lda LSB,x
    pha           ; throw the result away
    lda MSB,x
    pha		        ; cache the high word of quotient

    lda W3		    ; uncache the divisor
    sta LSB,x
    lda W3 + 1
    sta MSB,x
    jsr UM_DIV_MOD	; divide the low word
    dex
    pla 		    ; push the high word of quotient
    sta MSB,x
    pla
    sta LSB,x
    rts

