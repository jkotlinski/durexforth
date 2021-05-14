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

; QUIT INTERPRET FIND FIND-NAME >CFA PARSE-NAME WORD EXECUTE EVALUATE ' ABORT /STRING

restore_handler
   pha				; save a
   txa				; copy x
   pha				; save x
   tya				; copy y
   pha				; save y
   lda	#$7f	    ; disable all interrupts
   sta	$dd0d       ;
   ldy	$dd0d       ; save cia 2 icr
   bpl stop_restore ; if high bit not set

kernal_nmi
   jmp $fe72

stop_restore
    jsr	$f6bc		; increment real time clock
                    ; scan stop key
   	lda	$91 		; read the stop key column
	cmp	#$7f		; compare with [stp] down
	        		; if not [stp] or not just [stp] exit
    bne	kernal_nmi	; if not [stop] restore registers and exit interrupt

brk_handler
   pla
   pla
   tax              ; restore xr for QUIT
   jmp QUIT

quit_reset
    sei
    lda #<restore_handler
    sta $318
    lda #>restore_handler
    sta $319

    lda #<brk_handler
    sta $316
    lda #>brk_handler
    sta $317

    cli ; still have to

    ; lores
    lda #$9b
    sta $d011
    lda #$17
    sta $dd00
    sta $d018

    txa
    pha

    ldx #0
    stx $d020
    stx $d021

    lda #>TIB
    sta TIB_PTR + 1

    lda #$36 ; ram + i/o + kernal
    sta 1

    ; Yellow text.
    lda #7
    sta $286

    ; Clears color area.
-   sta $d800, x
    sta $d900, x
    sta $da00, x
    sta $db00, x
    dex
    bne -

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

    jsr     close_all_logical_files
    jsr     CLRCHN

    pla
    tax
    rts

    +BACKLINK "quit", 4
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

    +BACKLINK "bye", 3
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
    jsr INTERPRET
    cpx #X_INIT+1
    bpl .on_stack_underflow
    lda TO_IN_W
    cmp TIB_SIZE
    bne interpret_tib
    lda TO_IN_W + 1
    cmp TIB_SIZE + 1
    bne interpret_tib

    ; 0 - keyboard, -1 evaluate, else file
    lda SOURCE_ID_LSB
    beq +
    rts
+   lda LATEST_LSB
    sec
    sbc HERE_LSB
    lda LATEST_MSB
    sbc HERE_MSB
    beq .on_data_underflow
    lda #'o'
    jsr PUTCHR
    lda #'k'
    jsr PUTCHR
    lda #$d
    jmp PUTCHR

.on_stack_underflow
    lda #$12 ; reverse on
    jsr PUTCHR
    lda #'e'
    jsr PUTCHR
    lda #'r'
    jsr PUTCHR
    jmp .stop_error_print

.on_data_underflow
    lda #$12 ; reverse on
    jsr PUTCHR
    lda #'f'
    jsr PUTCHR
    lda #'u'
    jsr PUTCHR
    lda #'l'
    jsr PUTCHR
    lda #$d
    jmp PUTCHR

    +BACKLINK "execute", 7
EXECUTE
    lda LSB, x
    sta W
    lda MSB, x
    sta W + 1
    inx
    jmp (W)

    +BACKLINK "interpret", 9
INTERPRET
    jsr PARSE_NAME

    lda LSB,x
    bne +
    inx
    inx
    rts
+
    jsr TWODUP
    jsr FIND_NAME ; ( caddr u 0 | caddr u nt )
    lda MSB, x
    bne .found_word

    inx
    jsr READ_NUMBER
    beq .was_number

    jmp print_word_not_found_error

    ; yep, it's a number...
.was_number
    lda STATE ; are we compiling?
    bne +
    rts
+   ; yes, compile the number
    sta curr_word_no_tail_call_elimination
    jmp LITERAL

.found_word
    ; OK, we found a word...

    lda MSB, x
    ldy LSB, x
    inx
    sta MSB, x
    sty LSB, x
    sta MSB+1, x
    sty LSB+1, x
    jsr TO_XT
    jsr SWAP
    jsr GET_IMMED ; ( xt 1 | xt -1 )
    inx

    lda curr_word_no_tail_call_elimination
    sta last_word_no_tail_call_elimination
FOUND_WORD_WITH_NO_TCE = * + 1
    lda #0
    sta curr_word_no_tail_call_elimination

    ; Executes the word if it is immediate, or interpreting.
    lda MSB-1, x
    and STATE
    beq EXECUTE

    ; OK, this word should be compiled...
    jmp COMPILE_COMMA

print_word_not_found_error ; ( caddr u -- )
    lda #$12 ; reverse on
    jsr PUTCHR
    jsr TYPE
    lda #'?'
.stop_error_print
    jsr PUTCHR

    lda #$d ; cr
    jsr PUTCHR
    jmp ABORT

    +BACKLINK "'", 1
    jsr PARSE_NAME
    jsr TWODUP
    jsr FIND_NAME
    inx
    lda MSB-1,x
    beq print_word_not_found_error
+   ldy LSB-1, x
    sty LSB, x
    sta MSB, x
    sty LSB+1, x
    sta MSB+1, x
    jsr TO_XT
    jsr SWAP
    jsr GET_IMMED
    inx
    rts

    +BACKLINK "find", 4
FIND ; ( xt -1 | xt 1 | caddr 0 )
    jsr DUP
    jsr TO_R
    jsr COUNT
    jsr FIND_NAME
    lda MSB, x
    beq +
    jsr DUP
    jsr TO_XT
    jsr SWAP
    jsr GET_IMMED
    jsr R_TO
    inx
    rts
+   inx
    jsr R_TO
    jmp ZERO

    +BACKLINK "find-name", 9
FIND_NAME ; ( caddr u -- nt | 0 )
    inx
    lda LSB-1,x
    beq .find_failed
    sta .findlen + 1
    sta .findlen2 + 1

    lda MSB,x
    sta W2+1
    lda LSB,x
    sta W2
    lda LATEST_LSB
    sta W
    lda LATEST_MSB
    sta W + 1
    ; W now contains new dictionary pointer.
.examine_word
    ldy #0
    lda (W), y ; get string length of dictionary word
    and #STRLEN_MASK | F_HIDDEN ; include hidden flag... so we don't find the hidden words.
.findlen
    cmp #0
    beq .string_compare

.word_not_equal
    ; no match, advance the dp
    ldy #0
    lda (W), y
    and #STRLEN_MASK
    clc
    adc #3
    adc W
    sta W
    bcc +
    inc W + 1
+   lda(W), y
    ; Is word null? If not, examine it.
    bne .examine_word

    ; It is null - give up.
.find_failed
    inx
    jmp ZERO

.string_compare
    ; equal strlen, now compare strings...
.findlen2
    lda #0
    sta .strlen
-   lda (W2), y ; find string
    jsr CHAR_TO_LOWERCASE
    iny
    cmp (W), y ; dictionary string
    bne .word_not_equal
    dec .strlen
    beq .word_is_equal
    jmp -

.strlen !byte 0

.word_is_equal
    ; return address to dictionary word
    ldy #0
    lda (W), y
    and #F_NO_TAIL_CALL_ELIMINATION | F_IMMEDIATE
    sta FOUND_WORD_WITH_NO_TCE
    lda W
    sta LSB, x
    lda W + 1
    sta MSB, x
    rts


GET_IMMED ; ( nt -- 1 | -1 )
    lda MSB, x
    sta W + 1
    lda LSB, x
    sta W
    ldy #0

    lda (W), y ; a contains string length + mask
    and #F_IMMEDIATE
    beq .not_immed
    sty MSB, x ; 0
    iny
    sty LSB, x ; 1
    rts

.not_immed
    lda #$ff
    sta LSB, x
    sta MSB, x
    rts

    +BACKLINK ">xt", 3
TO_XT
    lda MSB, x
    sta W + 1
    lda LSB, x
    sta W
    ; W contains pointer to word
    ldy #0
    lda (W), y ; a contains string length + mask
    and #STRLEN_MASK
    clc
    adc #1 ; offset for char + string length
    adc LSB, x
    sta LSB, x
    bcc +
    inc MSB, x
+   jsr FETCH
    rts

IS_SPACE ; ( c -- f )
    ldy #1
    lda LSB,x
    cmp #' ' | 0x80
    beq .is_space
    lda #' '
    cmp LSB,x
    bcs .is_space
    dey
.is_space:
    sty LSB,x
    sty MSB,x
    rts

IS_NOT_SPACE ; ( c -- f )
    jsr IS_SPACE
    jmp ZEQU

XT_SKIP ; ( addr n xt -- addr n )
    ; skip all chars satisfying xt
    jsr TO_R
-   jsr DUP
    jsr ZBRANCH
    !word .done
    jsr OVER
    jsr FETCHBYTE
    jsr R_FETCH
    jsr EXECUTE
    jsr ZBRANCH
    !word .done
    jsr ONE
    jsr SLASH_STRING
    jmp -
.done
    jsr R_TO
    inx
    rts

    +BACKLINK "parse-name", 10
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
    +BACKLINK "word", 4
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

    +BACKLINK "evaluate", 8
EVALUATE
    jsr SAVE_INPUT
    lda LSB + 1, x
    sta TIB_PTR
    lda MSB + 1, x
    sta TIB_PTR + 1
    jsr PLUS
    lda LSB, x
    sta .bufend
    lda MSB, x
    sta .bufend + 1
    inx

    jsr evaluate_get_new_line
    ldy #$ff
    sty SOURCE_ID_LSB
    sty SOURCE_ID_MSB

.eval_loop
    lda TIB_PTR + 1
    cmp .bufend + 1
    bcc +
    lda TIB_PTR
    cmp .bufend
    bcc +
    jmp RESTORE_INPUT ; exit
+
    jsr interpret_tib
    jsr REFILL
    jmp .eval_loop

evaluate_get_new_line
    ldy #0
    sty TO_IN_W
    sty TO_IN_W + 1

    ; Determines TIB_SIZE.
    lda TIB_PTR
    sta W
    lda TIB_PTR + 1
    sta W + 1
.findtibsizeloop
    lda .bufend + 1
    cmp W + 1
    bcc .foundeol
    bne +
    lda W
    cmp .bufend
    bcs .foundeol
+
    ldy #0
    lda (W),y
    cmp #K_RETURN
    beq .foundeol

    inc W
    bne +
    inc W + 1
+
    jmp .findtibsizeloop

.foundeol
    lda W
    sec
    sbc TIB_PTR
    sta TIB_SIZE
    lda W + 1
    sbc TIB_PTR + 1
    sta TIB_SIZE + 1
    rts

evaluate_consume_tib
    lda TIB_PTR
    clc
    adc TIB_SIZE
    sta TIB_PTR
    lda TIB_PTR + 1
    adc TIB_SIZE + 1
    sta TIB_PTR + 1

    inc TIB_PTR ; skip cr
    bne +
    inc TIB_PTR + 1
+   rts

.bufend
    !word 0

    +BACKLINK "abort", 5
ABORT
    ldx #X_INIT ; reset stack
    jmp QUIT

    +BACKLINK "/string", 7
SLASH_STRING ; ( addr u n -- addr u )
    jsr DUP
    jsr TO_R
    jsr MINUS
    jsr SWAP
    jsr R_TO
    jsr PLUS
    jmp SWAP

apply_base
    sta BASE
    dec .chars_to_process
    inc W3
    bne +
    inc W3+1
+   lda (W3),y
    rts

; Z = success, NZ = fail
; success: ( caddr u -- number )
; fail: ( caddr u -- caddr u )
READ_NUMBER
    lda LSB,x
    sta .chars_to_process
    lda MSB+1,x
    sta W3+1
    lda LSB+1,x
    sta W3

    lda BASE
    sta OLD_BASE

    ldy #0
    sty .negate
    dex
    dex
    sty LSB+1,x
    sty MSB+1,x
    sty MSB,x

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

    cmp #10 ; within 0-9?
    bcc +

    clc
    adc #-$7 ; a-f..?

    cmp #10
    bcc .parse_failed

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
    sta LSB+3,x
    lda MSB+1,x
    sta MSB+3,x
    inx
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

+BACKLINK "dowords", 7 ; ( xt -- )
    ; to be useful, nothing must be left on stack before execute
    ; so that there is no distance between nt and the rest of the stack
    lda LSB,x
    sta .xt
    lda MSB, x
    sta .xt + 1
    inx
    lda LATEST_LSB
    sta .dowords_nametoken
    lda LATEST_MSB
    sta .dowords_nametoken + 1

.dowords_lambda
    lda .dowords_nametoken
    sta W
    lda .dowords_nametoken + 1
    sta W + 1
    ldy #0
    lda (W), y
    bne +
-   rts
+   and #STRLEN_MASK
    pha
    dex
    lda .dowords_nametoken
    sta LSB, x
    lda .dowords_nametoken + 1
    sta MSB, x
.xt = * + 1
    jsr PLACEHOLDER_ADDRESS
    inx
    pla
    ldy LSB-1, x
    beq -
    clc
    adc #3 ; guaranteed carry clear
    adc .dowords_nametoken
    sta .dowords_nametoken
    lda .dowords_nametoken + 1
    adc #0
    sta .dowords_nametoken + 1
    jmp .dowords_lambda
; using a word here in case the lambda trashes Ws
.dowords_nametoken
    !word 0
