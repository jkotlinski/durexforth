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

; C, , ; IMMEDIATE [ ] STATE : HEADER LIT LITC COMPILE, LITERAL HERE DODOES

curr_word_no_tail_call_elimination
    !byte 1
last_word_no_tail_call_elimination
    !byte 1

    +BACKLINK "c,", 2
CCOMMA
    lda	HERE_LSB
    sta	W
    lda	HERE_MSB
    sta	W + 1

    ldy	#0
    lda	LSB, x
    sta	(W), y

    ; update HERE
    inc	HERE_LSB
    bne	+
    inc HERE_MSB
+   inx
    rts

    +BACKLINK ",", 1
COMMA
    lda	HERE_LSB
    sta	W
    lda	HERE_MSB
    sta	W + 1

    ldy	#0
    lda	LSB, x
    sta	(W), y
    iny
    lda	MSB, x
    sta	(W), y

    ; update HERE
    lda	HERE_LSB
    clc
    adc	#2
    sta	HERE_LSB
    bcc	+
    inc HERE_MSB
+
    inx
    rts

; -----

    +BACKLINK "[", 1 | F_IMMEDIATE
LBRAC
    lda	#0
    sta	STATE
    rts

; -----

    ; disable tail call elimination in case of inline assembly
    +BACKLINK "]", 1 | F_NO_TAIL_CALL_ELIMINATION
RBRAC
    lda	#1
    sta	STATE
    rts

    +BACKLINK ";", 1 | F_IMMEDIATE
SEMICOLON
    jsr EXIT

    ; Unhides the word.
    inx
    lda MSB - 1, x
    beq +
    sta	W + 1
    lda	LSB - 1, x
    sta W

    ldy	#0
    lda	(W), y
    and	#!F_HIDDEN ; clear hidden flag
    sta	(W), y
+

    ; go back to IMMEDIATE mode.
    jmp LBRAC

    +BACKLINK "immediate", 9
    ldy #0
    lda LATEST_LSB
    sta W
    lda LATEST_MSB
    sta W + 1
    lda	(W), y
    ora	#F_IMMEDIATE
    sta	(W), y
    rts

; STATE - Is the interpreter executing code (0) or compiling a word (non-zero)?
    +BACKLINK "state", 5
    +VALUE	STATE
STATE
    !word 0

    +BACKLINK ":", 1 | F_NO_TAIL_CALL_ELIMINATION
COLON
    jsr HEADER ; makes the dictionary entry / header

    ; Hides the word.
    dex
    lda	LATEST_LSB
    sta	W
    sta LSB, x
    lda	LATEST_MSB
    sta W + 1
    sta MSB, x

    ldy	#0
    lda	(W), y
    ora	#F_HIDDEN ; sets hidden flag
    sta	(W), y

    jmp RBRAC ; enter compile mode


; --- HEADER ( name -- )
    +BACKLINK "header", 6
HEADER
    inc last_word_no_tail_call_elimination

    ; update dictionary

-   jsr PARSE_NAME
    lda LSB,x
    bne +
    jsr REFILL
    jmp -
+
    ; update dictionary pointer
    lda LSB, x
    sta .putlen+1
    clc
    adc #3
    sta W
    lda LATEST_LSB
    sec
    sbc W
    sta LATEST_LSB
    sta W
    bcs +
    dec LATEST_MSB
+
    lda LATEST_MSB
    sta W + 1
    ldy #0
    ; Store length byte.
    lda LSB, x
    sta (W), y
    inx
    lda LSB, x
    sta W2
    lda MSB, x
    sta W2 + 1
    ; copy string
-   lda (W2), y
    jsr CHAR_TO_LOWERCASE
    iny
    sta (W), y
.putlen
    cpy #0
    bne -
    ; store here
    iny
    lda HERE_LSB
    sta (W), y
    iny
    lda HERE_MSB
    sta (W), y
    inx
    rts

    +BACKLINK "lit", 3
LIT
    dex

    ; load IP
    pla
    sta W
    pla
    sta W + 1

    ; copy literal to stack
    ldy	#1
    lda	(W), y
    sta	LSB, x
    iny
    lda	(W), y
    sta	MSB, x

    lda W
    clc
    adc #3
    sta + + 1
    lda W + 1
    adc #0
    sta + + 2
+   jmp PLACEHOLDER_ADDRESS ; replaced with instruction pointer

    +BACKLINK "litc", 4
LITC
    dex

    ; load IP
    pla
    sta W
    pla
    sta W + 1

    inc W
    bne +
    inc W + 1
+
    ; copy literal to stack
    ldy	#0
    lda	(W), y
    sta	LSB, x
    sty	MSB, x

    inc W
    bne +
    inc W + 1
+   jmp (W)

    +BACKLINK "compile,", 8
COMPILE_COMMA
    lda #OP_JSR
    jsr compile_a
    jmp COMMA

    +BACKLINK "literal", 7 | F_IMMEDIATE
LITERAL
    dex
    lda MSB+1,x
    bne +
    lda #<LITC
    sta LSB,x
    lda #>LITC
    sta MSB,x
    jsr COMPILE_COMMA
    jmp CCOMMA ; writes byte
+
    lda #<LIT
    sta LSB, x
    lda #>LIT
    sta MSB, x
    jsr COMPILE_COMMA
    jmp COMMA ; writes number

; HERE - points to the next free byte of memory. When compiling, compiled words go here.
    +BACKLINK "here", 4
HERE
HERE_LSB = * + 1
HERE_MSB = * + 3
    +VALUE  load_base

    +BACKLINK "dodoes", 6

    ; behavior pointer address => W
    pla
    sta W
    pla
    sta W + 1

    inc W
    bne +
    inc W + 1
+

    ; push data pointer to param stack
    dex
    lda W
    clc
    adc #2
    sta LSB,x
    lda W + 1
    adc #0
    sta MSB,x

    ldy #0
    lda (W),y
    sta W2
    iny
    lda (W),y
    sta W2 + 1
    jmp (W2)
