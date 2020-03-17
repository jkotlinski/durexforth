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

    +BACKLINK
    !byte	2
    !text	"c,"
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

    +BACKLINK
    !byte	1
    !text	","
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

    +BACKLINK
    !byte	1 | F_IMMEDIATE
    !text	"["
LBRAC
    lda	#0
    sta	STATE
    rts

; -----

    +BACKLINK
    ; disable tail call elimination in case of inline assembly
    !byte      1 | F_NO_TAIL_CALL_ELIMINATION
    !text	"]"
RBRAC
    lda	#1
    sta	STATE
    rts

    +BACKLINK
    !byte	1 | F_IMMEDIATE
    !text	";"
SEMICOLON
    jsr EXIT

    ; Unhides the word.
    inx
    lda MSB - 1, x
    beq +
    sta	W + 1
    lda	LSB - 1, x
    sta W

    ldy	#2 ; skip link, point to flags
    lda	(W), y
    and	#!F_HIDDEN ; clear hidden flag
    sta	(W), y
+

    ; go back to IMMEDIATE mode.
    jmp LBRAC

    +BACKLINK
    !byte	9
    !text	"immediate"
    lda	_LATEST
    sta	W
    lda	_LATEST + 1
    sta	W + 1
    ldy	#2
    lda	(W), y
    ora	#F_IMMEDIATE
    sta	(W), y
    rts

; STATE - Is the interpreter executing code (0) or compiling a word (non-zero)?
    +BACKLINK
    !byte 5
    !text	"state"
    +VALUE	STATE
STATE
    !word 0

    +BACKLINK
    !byte	1 | F_NO_TAIL_CALL_ELIMINATION
    !text	":"
COLON
    jsr HEADER ; makes the dictionary entry / header

    ; Hides the word.
    dex
    lda	_LATEST
    sta	W
    sta LSB, x
    lda	_LATEST + 1
    sta W + 1
    sta MSB, x

    ldy	#2 ; skip link, point to flags
    lda	(W), y
    ora	#F_HIDDEN ; sets hidden flag
    sta	(W), y

    jmp RBRAC ; enter compile mode


; --- HEADER ( name -- )
    +BACKLINK
    !byte	6
    !text	"header"
HEADER
    inc last_word_no_tail_call_elimination

    jsr HERE

    ; Store backlink.
    jsr LATEST
    jsr FETCH
    jsr COMMA

-   jsr PARSE_NAME
    lda LSB,x
    bne +
    jsr REFILL
    jmp -
+
    ; Store length byte.
    jsr DUP
    jsr CCOMMA

-   jsr SWAP
    jsr DUP
    jsr FETCHBYTE
    lda LSB,x
    jsr CHAR_TO_LOWERCASE
    sta LSB,x
    jsr CCOMMA
    jsr ONEPLUS
    jsr SWAP
    jsr ONEMINUS
    lda LSB,x
    bne -
    inx
    inx

    jsr LATEST
    jmp STORE

    +BACKLINK
    !byte	3
    !text	"lit"
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

    +BACKLINK
    !byte	4
    !text	"litc"
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

    +BACKLINK
    !byte	8
    !text	"compile,"
COMPILE_COMMA
    lda #OP_JSR
    jsr compile_a
    jmp COMMA

    +BACKLINK
    !byte 7 | F_IMMEDIATE
    !text "literal"
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
    +BACKLINK
    !byte 4
    !text	"here"
HERE
HERE_LSB = * + 1
HERE_MSB = * + 3
    +VALUE	_LATEST + 2

    !word	LINK
    !set	LINK = * - 2
    !byte	6
    !text	"dodoes"

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

