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

; IF THEN BEGIN WHILE REPEAT BRANCH 0BRANCH UNLOOP EXIT

    +BACKLINK
    !byte 2 | F_IMMEDIATE
    !text	"if"
    jsr LIT
    !word ZBRANCH
    jsr COMPILE_COMMA
    jsr HERE
    jsr ZERO
    jmp COMMA

    +BACKLINK
    !byte 4 | F_IMMEDIATE
    !text	"then"
    jsr HERE
    jsr SWAP
    jmp STORE

    +BACKLINK
    !byte 5 | F_IMMEDIATE
    !text	"begin"
    jmp HERE

    +BACKLINK
    !byte 5 | F_IMMEDIATE
    !text	"while"
    jsr LIT
    !word ZBRANCH
    jsr COMPILE_COMMA
    jsr HERE
    jsr ZERO
    jsr COMMA
    jmp SWAP

COMPILE_JMP
    jsr LITC
    !byte OP_JMP
    jmp CCOMMA

    +BACKLINK
    !byte 6 | F_IMMEDIATE
    !text	"repeat"
    jsr COMPILE_JMP
    jsr COMMA
    jsr HERE
    jsr SWAP
    jmp STORE

    +BACKLINK
    !byte	6
    !text	"branch"
BRANCH
    pla
    sta W
    pla
    sta W + 1

    ldy	#2
    lda	(W), y
    sta + + 2
    dey
    lda	(W), y
    sta + + 1
+   jmp PLACEHOLDER_ADDRESS ; replaced with branch destination

    +BACKLINK
    !byte	7
    !text	"0branch"
ZBRANCH
    inx
    lda	LSB-1, x
    ora	MSB-1, x
    beq BRANCH

    ; skip offset
    pla
    clc
    adc #3
    sta + + 1
    pla
    adc #0
    sta + + 2
+   jmp PLACEHOLDER_ADDRESS ; replaced with branch destination

    +BACKLINK
    !byte	6 | F_NO_TAIL_CALL_ELIMINATION
    !text	"unloop"
    jsr R_TO
    jsr R_TO
    jsr R_TO
    inx
    inx
    jsr TO_R
    rts

    +BACKLINK
    !byte	4 | F_IMMEDIATE
    !text	"exit"
EXIT
    lda last_word_no_tail_call_elimination
    bne +
    lda HERE_LSB
    sec
    sbc #3
    sta .instr_ptr
    lda HERE_MSB
    sbc #0
    sta .instr_ptr + 1
    lda #OP_JMP
.instr_ptr = * + 1
    sta PLACEHOLDER_ADDRESS ; replaced with instruction pointer
    rts
+
    lda #OP_RTS
compile_a
    dex
    sta LSB, x
    jmp CCOMMA

