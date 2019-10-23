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

; TYPE EMIT PAGE KEY? KEY REFILL SOURCE

    +BACKLINK
    !byte	4
    !text	"emit"
EMIT
    lda	LSB, x
    inx
    jmp	PUTCHR

    +BACKLINK
    !byte   4
    !text   "page"
PAGE
    lda #K_CLRSCR
    jmp PUTCHR

    +BACKLINK
    !byte 4
    !text	"type"
TYPE ; ( caddr u -- )
    lda #0 ; quote mode off
    sta $d4
-   lda LSB,x
    ora MSB,x
    bne +
    inx
    inx
    rts
+   jsr SWAP
    jsr DUP
    jsr FETCHBYTE
    jsr EMIT
    jsr ONEPLUS
    jsr SWAP
    jsr ONEMINUS
    jmp -

    +BACKLINK
    !byte	4
    !text	"key?"
    lda $c6 ; Number of characters in keyboard buffer
    beq +
.pushtrue
    lda #$ff
+   tay
    jmp pushya

    +BACKLINK
    !byte	3
    !text	"key"
-   lda $c6
    beq -
    stx W
    jsr $e5b4 ; Get character from keyboard buffer
    ldx W
    ldy #0
    jmp pushya

    +BACKLINK
    !byte	6
    !text	"refill" ; ( -- )
REFILL

READ_EOF = * + 1
    lda #0
    beq +
    ; handle EOF
    stx tmp_x
    lda	SOURCE_ID_LSB
    jsr	CLOSE
    dec	SOURCE_ID_LSB
    ldx SOURCE_ID_LSB
    jsr CHKIN
    ldx tmp_x
    jmp RESTORE_INPUT ; exit
+
    lda SOURCE_ID_MSB
    beq +
    jsr evaluate_consume_tib
    jmp evaluate_get_new_line
+
    ldy #0
    sty TO_IN_W
    sty TO_IN_W + 1
    sty TIB_SIZE
    sty TIB_SIZE + 1

    lda SOURCE_ID_LSB
    beq .getLineFromConsole
    lda SOURCE_ID_MSB
    beq	.getLineFromDisk

.getLineFromConsole
    stx tmp_x
    ldx #0
-   jsr $e112 ; Input Character
    cmp #$d
    beq .gotReturn
    sta TIB,x
    cpx #$58 ; Default TIB area is $200-$258
    beq -
    inx
    jmp -
.gotReturn
    jsr PUTCHR
    ; Set TIB_SIZE to number of chars fetched.
    stx TIB_SIZE
    ldx tmp_x
    rts

.getLineFromDisk
    lda TIB_PTR
    sta W
    lda TIB_PTR + 1
    sta W + 1
-   jsr GET_CHAR_BLOCKING
    cmp #K_RETURN
    beq +
    inc $d020
    ldy TIB_SIZE
    sta (W),y
    inc TIB_SIZE
    dec $d020
    jmp -
+   rts

GET_CHAR_FROM_TIB
    lda TO_IN_W
    cmp TIB_SIZE
    bne +
    lda TO_IN_W + 1
    cmp TIB_SIZE + 1
    bne +
    lda #0
    rts
+
    clc
    lda TIB_PTR
    adc TO_IN_W
    sta W
    lda TIB_PTR + 1
    adc TO_IN_W + 1
    sta W + 1
    ldy #0
    lda (W),y

    inc TO_IN_W
    bne +
    inc TO_IN_W + 1
+   rts

    +BACKLINK
    !byte 6
    !text	"source"
SOURCE
    dex
    dex
    lda TIB_PTR
    sta LSB+1, x
    lda TIB_PTR + 1
    sta MSB+1, x
    lda TIB_SIZE
    sta LSB, x
    lda TIB_SIZE + 1
    sta MSB, x
    rts

TIB_PTR
    !word 0
TIB_SIZE
    !word 0
