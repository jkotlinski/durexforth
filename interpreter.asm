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

; QUIT

quit_reset
    sei
    lda	#<restore_handler
    sta $318
    lda	#>restore_handler
    sta $319
    cli

    ; lores
    lda #$9b
    sta $d011
    lda #$17
    sta $dd00
    sta $d018

    txa
    pha

    ldx #0
    stx	$d020
    stx	$d021

    lda #>TIB
    sta TIB_PTR + 1

    lda #$56 ; ram + i/o + kernal
    sta 1

    ; Yellow text.
    lda	#7
    sta	$286

    ; Clears color area.
-   sta $d800, x
    sta $d900, x
    sta $da00, x
    sta $db00, x
    dex
    bne	-

    stx     STATE
    stx     TIB_SIZE
    stx     TIB_SIZE + 1
    stx     TIB_PTR
    stx     TO_IN_W
    stx     TO_IN_W + 1
    stx     SOURCE_ID_LSB
    stx     SOURCE_ID_MSB
    stx     SAVE_INPUT_STACK_DEPTH
    stx     READ_EOF
    jsr     CHKIN
    pla
    tax
    rts

    +BACKLINK
    !byte	4
    !text	"quit"
QUIT
    jsr quit_reset

    ; resets the return stack
    txa
INIT_S = * + 1
    ldx #0
    txs
    tax

interpret_loop
    jsr REFILL

    jsr interpret_tib
    jmp interpret_loop

    +BACKLINK
    !byte 3
    !text "bye"
    ldx INIT_S
    txs
    pla
    sta $319
    pla
    sta $318
    pla
    sta 1
    rts

interpret_tib
    jsr	INTERPRET
    cpx #X_INIT+1
    bpl .on_stack_underflow
    lda TO_IN_W
    cmp TIB_SIZE
    bne interpret_tib
    lda TO_IN_W + 1
    cmp TIB_SIZE + 1
    bne interpret_tib

    lda SOURCE_ID_LSB
    beq +
    rts
+   lda	#'o'
    jsr	PUTCHR
    lda	#'k'
    jsr	PUTCHR
    lda	#$d
    jmp	PUTCHR

.on_stack_underflow
    lda	#$12 ; reverse on
    jsr	PUTCHR
    lda #'e'
    jsr	PUTCHR
    lda #'r'
    jsr	PUTCHR
    jmp .stop_error_print
