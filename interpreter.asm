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

; QUIT INTERPRET FIND FIND-NAME >CFA PARSE-NAME WORD EXECUTE

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

    +BACKLINK
    !byte	7
    !text	"execute"
EXECUTE
    lda	LSB, x
    sta W
    lda	MSB, x
    sta	W + 1
    inx
    jmp	(W)

    +BACKLINK
    !byte	9
    !text	"interpret"
INTERPRET
    jsr PARSE_NAME

    lda LSB,x
    bne +
    inx
    inx
    rts
+
    jsr	FIND_NAME ; replace string with dictionary ptr
    lda LSB, x
    bne	.found_word

    inx ; drop
    jsr READ_NUMBER
    beq .was_number

    jmp print_word_not_found_error

    ; yep, it's a number...
.was_number
    lda	STATE ; are we compiling?
    bne	+
    rts
+   ; yes, compile the number
    sta curr_word_no_tail_call_elimination
    jmp LITERAL

.found_word
    ; OK, we found a word...

    lda curr_word_no_tail_call_elimination
    sta last_word_no_tail_call_elimination
FOUND_WORD_WITH_NO_TCE = * + 1
    lda #0
    sta curr_word_no_tail_call_elimination

    ; Executes the word if it is immediate, or interpreting.
    inx
    lda MSB-1, x
    and STATE
    beq	EXECUTE

    ; OK, this word should be compiled...
    jmp COMPILE_COMMA

print_word_not_found_error ; ( caddr u -- )
    lda	#$12 ; reverse on
    jsr	PUTCHR
    jsr TYPE
    lda	#'?'
.stop_error_print
    jsr	PUTCHR

    lda	#$d ; cr
    jsr	PUTCHR
    jmp ABORT

    +BACKLINK
    !byte	4
    !text	"find"
FIND
    jsr DUP
    jsr TO_R
    jsr COUNT
    jsr FIND_NAME
    lda LSB,x
    beq +
    jsr R_TO
    inx
    rts
+   inx
    inx
    inx
    jsr R_TO
    jmp ZERO

    +BACKLINK
    !byte	9
    !text	"find-name"
FIND_NAME ; ( caddr u -- caddr u 0 | xt 1 | xt -1 )
    txa
    pha

    lda LSB,x
    beq .find_failed
    sta	.findlen + 1
    sta	.findlen2 + 1

    lda	MSB+1,x
    sta	W2+1
    lda	LSB+1,x
    sta	W2

    lda W2
    bne +
    dec W2+1
+   dec W2
    lda W2
    bne +
    dec W2+1
+   dec W2

    ldx	_LATEST
    lda	_LATEST + 1
.examine_word
    sta	W + 1
    stx	W
    ; W now contains new dictionary word.

    ldy	#2
    lda	(W), y ; get string length of dictionary word
    and	#STRLEN_MASK | F_HIDDEN ; include hidden flag... so we don't find the hidden words.
.findlen
    cmp	#0
    beq	.string_compare

.word_not_equal
    ; no match, advance the linked list.
    ldy	#0
    lax	(W), y
    iny
    lda	(W), y
    ; Is word null? If not, examine it.
    bne .examine_word

    ; It is null - give up.
.find_failed
    pla
    tax
    jmp ZERO

.string_compare ; y = 2
    ; equal strlen, now compare strings...
.findlen2
    lda #0
    sta .strlen
-   lda	(W2), y ; find string
    jsr CHAR_TO_LOWERCASE
    iny
    cmp	(W), y ; dictionary string
    bne	.word_not_equal
    dec	.strlen
    beq	.word_is_equal
    jmp	-

.strlen !byte 0

.word_is_equal
    ; return address to dictionary word
    pla
    tax
    inx
    lda	W
    sta	LSB, x
    lda	W + 1
    sta	MSB, x

    jsr TCFA

    dex

    ldy	#2
    lda (W), y
    and #F_NO_TAIL_CALL_ELIMINATION | F_IMMEDIATE
    sta FOUND_WORD_WITH_NO_TCE

    lda	(W), y ; a contains string length + mask
    and	#F_IMMEDIATE
    beq .not_immed
    dey
    sty LSB, x ; 1
    dey
    sty MSB, x ; 0
    rts

.not_immed
    lda #$ff
    sta LSB, x
    sta MSB, x
    rts

; >CFA
    +BACKLINK
    !byte	4
    !text	">cfa"
TCFA
    lda	MSB, x
    sta	W + 1
    lda	LSB, x
    sta W
    ; W contains pointer to word
    ldy	#2
    lda	(W), y ; a contains string length + mask
    and	#STRLEN_MASK
    clc
    adc	#3 ; offset for link + string length
    adc	LSB, x
    sta	LSB, x
    bcc	+
    inc	MSB, x
+   rts

    +BACKLINK
    !byte 10
    !text	"parse-name"
PARSE_NAME ; ( name -- addr u )
    jsr SOURCE
    jsr TO_IN
    jsr FETCH
    jsr SLASH_STRING
    jsr LIT
    !word IS_SPACE
    jsr XT_SKIP
    jsr OVER
    jsr TO_R
    jsr LIT
    !word IS_NOT_SPACE
    jsr XT_SKIP
    jsr TWODUP
    jsr ONE
    jsr MIN
    jsr PLUS
    jsr SOURCE
    inx
    jsr MINUS
    jsr TO_IN
    jsr STORE
    inx
    jsr R_TO
    jsr TUCK
    jmp MINUS

; WORD ( delim -- strptr )
    +BACKLINK
    !byte      4
    !text      "word"
WORD
    jsr ZERO
    jsr HERE
    jsr STOREBYTE

    ; skips initial delimiters.
-   jsr GET_CHAR_FROM_TIB
    beq .word_end
    jsr .is_delim
    beq -
    jmp .append

-   jsr GET_CHAR_FROM_TIB
    beq .word_end
    jsr .is_delim
    beq .word_end

.append
    jsr pushya

    jsr HERE
    jsr FETCHBYTE
    jsr ONEPLUS
    jsr HERE
    jsr STOREBYTE

    jsr HERE
    jsr HERE
    jsr FETCHBYTE
    jsr PLUS
    jsr STOREBYTE
    jmp -

.word_end
    inx
    jmp HERE

.is_delim
    ; a == delim?
    cmp LSB,x
    beq + ; yes

    ; delim == space?
    ldy LSB,x
    cpy #K_SPACE
    bne + ; no

    ; compare with nonbreaking space, too
    cmp #K_SPACE | $80
+   rts
