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

; U< - UM* UM/MOD M+

    +BACKLINK
    !byte	2
    !text	"u<"
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

    +BACKLINK
    !byte	1
    !text	"-"
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

    +BACKLINK
    !byte	3
    !text	"um*"
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

    +BACKLINK
	!byte	6
	!text	"um/mod"
; ( lsw msw divisor -- rem quot )
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

    +BACKLINK
	!byte	2
	!text	"m+" ; ( d1 u -- d2 )
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
