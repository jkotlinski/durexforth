; EMIT PAGE RVS CR TYPE KEY? KEY REFILL SOURCE SOURCE-ID >IN CHAR IOABORT
; INCLUDE-STRING

    +BACKLINK "emit", 4
EMIT
    lda	LSB, x
    inx
    jmp	PUTCHR

    +BACKLINK "page", 4
PAGE
    lda #K_CLRSCR
    jmp PUTCHR

    +BACKLINK "rvs", 3
RVS ; ( -- ) invert text output
    lda #$12
    jmp CHROUT

    +BACKLINK "cr", 2
CR ; ( -- )
    lda #$d
    jmp CHROUT

    +BACKLINK "type", 4
TYPE ; ( caddr u -- )
    lda #0 ; quote mode off
    sta $d4
-   lda LSB,x
    ora MSB,x
    bne +
    inx
    inx
    rts
+   jsr OVER
    jsr FETCHBYTE
    jsr EMIT
    jsr ONE
    jsr SLASH_STRING
    jmp -

    +BACKLINK "key?", 4
    lda $c6 ; Number of characters in keyboard buffer
    beq +
.pushtrue
    lda #$ff
+   tay
    jmp pushya

    +BACKLINK "key", 3
-   lda $c6
    beq -
    stx W
    jsr $e5b4 ; Get character from keyboard buffer
    ldx W
    ldy #0
    jmp pushya

CLOSE_INPUT_SOURCE
    stx W
    lda	SOURCE_ID_LSB
    jsr	CLOSE
    jsr POP_INPUT_SOURCE
    ldx SOURCE_ID_LSB
    beq +
    jsr CHKIN
    jmp ++
+   jsr CLRCHN
++  ldx W
    rts

    +BACKLINK "refill", 6
REFILL ; ( -- flag )

    ldy #0
    sty TO_IN_W
    sty TO_IN_W + 1
    sty TIB_SIZE
    sty TIB_SIZE + 1

    lda SOURCE_ID_LSB
    beq .getLineFromConsole
    cmp #-2
    beq .getLineFromIncludeRam
    cmp #-1
    bne .getLineFromDisk

    ; evaluate = fail

.return_false
    dex
    lda #0
    sta MSB,x
    sta LSB,x
    rts

.getLineFromConsole
    stx W
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
    ldx W
.return_true
    dex
    lda #$ff
    sta LSB,x
    sta MSB,x
    rts

.getLineFromDisk
    jsr READST
    bne .return_false ; eof/error

    lda TIB_PTR
    sta W
    lda TIB_PTR + 1
    sta W+1
-   stx W2
    jsr	CHRIN
    ldx W2
    ora #0
    beq .return_true
    cmp #K_RETURN
    beq .return_true
    inc $d020
    ldy TIB_SIZE
    sta (W),y
    inc TIB_SIZE
    dec $d020
    jmp -

.getLineFromIncludeRam
    lda INCLUDE_STRING_SIZE_LSB
    ora INCLUDE_STRING_SIZE_MSB
    beq .return_false

INCLUDE_STRING_PTR_LSB = * + 1
    lda #0
    sta TIB_PTR
INCLUDE_STRING_PTR_MSB = * + 1
    lda #0
    sta TIB_PTR + 1

.include_ram_loop
    lda INCLUDE_STRING_PTR_LSB
    sta + + 1
    lda INCLUDE_STRING_PTR_MSB
    sta + + 2
+   lda PLACEHOLDER_ADDRESS
    tay

    inc INCLUDE_STRING_PTR_LSB
    bne +
    inc INCLUDE_STRING_PTR_MSB
+
    lda INCLUDE_STRING_SIZE_LSB
    bne +
    dec INCLUDE_STRING_SIZE_MSB
+   dec INCLUDE_STRING_SIZE_LSB

    tya
    cmp #$d
    beq .return_true

    inc TIB_SIZE ; max line length = 256

INCLUDE_STRING_SIZE_LSB = * + 1
    lda #0
INCLUDE_STRING_SIZE_MSB = * + 1
    ora #0
    bne .include_ram_loop
    jmp .return_true

    +BACKLINK "source", 6
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

    +BACKLINK "source-id", 9
SOURCE_ID_LSB = * + 1
SOURCE_ID_MSB = * + 3
    ; -2 : INCLUDE-RAM
    ; -1 : EVALUATE
    ;  0 : keyboard
    ; >0 : file id
    +VALUE	0

    +BACKLINK ">in", 3
TO_IN
    +VALUE TO_IN_W
TO_IN_W
    !word 0

    +BACKLINK "char", 4
CHAR ; ( name -- char )
    jsr PARSE_NAME
    inx
    jmp FETCHBYTE

SAVE_INPUT_STACK
    ; Forth standard 11.3.3 "Input Source":
    ; "Input [...] shall be nestable in any order to at least eight levels."
    ; Eight levels is overkill for INCLUDED, since opening more than four DOS
    ; channels gives a "no channel" error message on C64.
    ; It is anyway nice to keep some extra levels for EVALUATE and LOAD.
    !fill 8*8
SAVE_INPUT_STACK_DEPTH
    !byte 0

push_input_stack
    ; Stack overflow check could be added, but does not seem needed in practice.
    ldy SAVE_INPUT_STACK_DEPTH
    sta SAVE_INPUT_STACK, y
    inc SAVE_INPUT_STACK_DEPTH
    rts

pop_input_stack
    dec SAVE_INPUT_STACK_DEPTH
    ldy SAVE_INPUT_STACK_DEPTH
    lda SAVE_INPUT_STACK, y
    rts

PUSH_INPUT_SOURCE
    lda TO_IN_W
    jsr push_input_stack
    lda TO_IN_W+1
    jsr push_input_stack
    lda SOURCE_ID_LSB
    jsr push_input_stack
    lda SOURCE_ID_MSB
    jsr push_input_stack
    lda TIB_PTR
    jsr push_input_stack
    lda TIB_PTR+1
    jsr push_input_stack
    lda TIB_SIZE
    jsr push_input_stack
    lda TIB_SIZE+1
    jmp push_input_stack

POP_INPUT_SOURCE
    jsr pop_input_stack
    sta TIB_SIZE+1
    jsr pop_input_stack
    sta TIB_SIZE
    jsr pop_input_stack
    sta TIB_PTR+1
    jsr pop_input_stack
    sta TIB_PTR
    jsr pop_input_stack
    sta SOURCE_ID_MSB
    jsr pop_input_stack
    sta SOURCE_ID_LSB
    jsr pop_input_stack
    sta TO_IN_W+1
    jsr pop_input_stack
    sta TO_IN_W
    rts

; handle errors returned by open,
; close, and chkin. If ioresult is
; nonzero, print error message and
; abort.
    +BACKLINK "ioabort", 7
IOABORT ; ( ioresult -- )
    inx
    lda MSB-1,x
    bne .print_ioerr
    lda LSB-1,x
    bne +
    rts
+   cmp #10
    bcc .print_basic_error

.print_ioerr
    lda #<.ioerr
    sta W
    lda #>.ioerr
    sta W+1
    jmp .print_msb_terminated_string

.ioerr
    !text "ioer"
    !byte 'r'|$80

.print_basic_error
    lda #$37
    sta 1

    lda LSB-1,x
    asl
    tax
    lda $a326,x
    sta W
    lda $a327,x
    sta W+1

.print_msb_terminated_string
    jsr CLRCHN
    jsr RVS

    ldy #0
-   lda (W),y
    pha
    and #$7f
    jsr CHROUT
    iny
    pla
    bpl -

.cr_abort
    jsr CR
    jmp ABORT

    +BACKLINK "include-string", 14
    jsr PUSH_INPUT_SOURCE
    lda LSB + 1, x
    sta INCLUDE_STRING_PTR_LSB
    lda MSB + 1, x
    sta INCLUDE_STRING_PTR_MSB
    lda LSB, x
    sta INCLUDE_STRING_SIZE_LSB
    lda MSB, x
    sta INCLUDE_STRING_SIZE_MSB
    inx
    inx

    ldy #-1
    sty SOURCE_ID_MSB
    dey
    sty SOURCE_ID_LSB

    jmp interpret_and_close
