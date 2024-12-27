; C, , [ ] ; IMMEDIATE STATE LATESTXT : HEADER LIT LITC COMPILE, LITERAL HERE
; DODOES

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

    ; Exempt from TCE because `: x [ ] ;` does not compile a jsr.
    +BACKLINK "]", 1 | F_NO_TAIL_CALL_ELIMINATION
RBRAC
    lda	#1
    sta	STATE
    rts

    +BACKLINK ";", 1 | F_IMMEDIATE
SEMICOLON
    jsr EXIT

    ; Unhides the word.
PENDING_LATEST_MSB = * + 1
    lda #0
    beq +
    sta	LATEST_MSB
PENDING_LATEST_LSB = * + 1
    lda	#0
    sta LATEST_LSB
    lda #0
    sta PENDING_LATEST_MSB
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

    +BACKLINK "latestxt", 8
LATEST_XT_LSB = * + 1
LATEST_XT_MSB = * + 3
    +VALUE	0

    ; Exempt from TCE because `: x ;` does not compile a jsr.
    +BACKLINK ":", 1 | F_NO_TAIL_CALL_ELIMINATION
COLON
    lda LATEST_LSB
    pha
    lda LATEST_MSB
    pha

    jsr HEADER ; makes the dictionary entry / header

    ; defer the LATEST update to ;
    lda LATEST_LSB
    sta PENDING_LATEST_LSB
    lda LATEST_MSB
    sta PENDING_LATEST_MSB

    pla
    sta LATEST_MSB
    pla
    sta LATEST_LSB

    lda HERE_LSB
    sta LATEST_XT_LSB
    lda HERE_MSB
    sta LATEST_XT_MSB

    jmp RBRAC ; enter compile mode

    +BACKLINK "header", 6
HEADER ; ( "name" -- )
    inc last_word_no_tail_call_elimination

    ; Parse, get [W2]name-addr and [LSB-2]length.
    jsr PARSE_NAME
    inx
    lda LSB, x
    sta W2
    lda MSB, x
    sta W2 + 1
    inx

    ; Abort if empty string.
    lda LSB - 2, x
    bne +
    lda #-16 ; attempt to use zero-length string as a name
    jmp throw_a
+   sta .putlen+1

    ; Move back [W]LATEST.
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
+   lda LATEST_MSB
    sta W + 1

    ; Store name length.
    ldy #0
    lda LSB - 2, x
    sta (W), y

    ; Copy name.
-   lda (W2), y
    jsr CHAR_TO_LOWERCASE
    iny
    sta (W), y
.putlen
    cpy #0
    bne -

    ; Store xt.
    iny
    lda HERE_LSB
    sta (W), y
    iny
    lda HERE_MSB
    sta (W), y
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

    clc
    lda W
    adc #3
    sta W
    bcc +
    inc W+1
+   jmp (W)

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
    lda MSB + 1,x
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
    +VALUE  HERE_POSITION

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

