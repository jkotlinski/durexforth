;The MIT License
;
;Copyright (c) 2013 Johan Kotlinski
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
;THE SOFTWARE.

; Methods for number parsing

    +BACKLINK
    !byte 6
    !text "invert"
INVERT
    lda MSB, x
    eor #$ff
    sta MSB, x
    lda LSB, x
    eor #$ff
    sta LSB,x
    rts

    +BACKLINK
    !byte 6
    !text "negate"
NEGATE
    jsr INVERT
    jmp ONEPLUS

    +BACKLINK
    !byte 4
    !text "base"
    +VALUE	BASE
BASE
    !word 16

apply_base
    sta BASE
    dec .chars_to_process
    inc W3
    bne +
    inc W3+1
+   lda (W3),y
    rts

; Z = success, NZ = fail
; success: ( string ptr -- number )
; fail: ( string ptr -- string ptr )
READ_NUMBER
    lda MSB, x
    sta W3 + 1
    lda LSB, x
    sta W3
    ; W3 now points to string length
    ; followed by string. (Using W3
    ; because U_M_STAR trashes W, W2)

    dex
    dex

    lda BASE
    sta OLD_BASE

    ldy #0
    sty .negate
    sty LSB+1,x
    sty MSB+1,x
    sty MSB,x
    lda (W3), y
    sta .chars_to_process

    inc W3
    bne +
    inc W3+1
+

    lda (W3), y
    cmp #"'"
    beq .parse_char

    cmp #"#"
    bne .check_decimal
    lda #10
    jsr apply_base

.check_decimal
    cmp #"$"
    bne .check_binary
    lda #16
    jsr apply_base

.check_binary
    cmp #"%"
    bne .check_negate
    lda #2
    jsr apply_base

.check_negate
    cmp #"-"
    bne .loop_entry
    inc .negate
    jmp .prepare_next_char

.next_digit
    ; number *= BASE
    lda BASE
    sta LSB,x
    jsr U_M_STAR
    lda LSB,x
    bne .parse_failed ; overflow!

    inc W3
    bne +
    inc W3+1
+   lda (W3), y

.loop_entry
    jsr CHAR_TO_LOWERCASE

    clc
    adc #-$30 ; petscii 0-9 -> 0-9

    cmp	#10 ; within 0-9?
    bcc	+

    clc
    adc	#-$7 ; a-f..?

    cmp	#10
    bcc	.parse_failed

+   cmp BASE
    bcs .parse_failed

    adc LSB+1,x
    sta LSB+1,x
    bcc .prepare_next_char
    inc MSB+1,x
    beq .parse_failed
.prepare_next_char
    dec .chars_to_process
    bne .next_digit

.parse_done
OLD_BASE = * + 1
    lda #0
    sta BASE

    lda LSB+1,x
    sta LSB+2,x
    lda MSB+1,x
    sta MSB+2,x
    inx
    inx
.negate = * + 1
    lda #0
    beq +
    jsr NEGATE
    tya ; clear Z flag
+   rts

.parse_char
    lda .chars_to_process
    cmp #3
    bne .parse_failed
    ldy #2
    lda (W3),y
    cmp #"'"
    bne .parse_failed
    dey
    lda (W3),y
    sta LSB+1,x
    lda #0
    sta MSB+1,x
    jmp .parse_done

.parse_failed
    inx
    inx ; Z flag set
    rts

.chars_to_process
    !byte 0
