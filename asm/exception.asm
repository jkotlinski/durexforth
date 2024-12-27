; CATCH THROW (ABORT")

EXCEPTION_HANDLER
    +VALUE _EXCEPTION_HANDLER
_EXCEPTION_HANDLER
    !word 0

+BACKLINK "catch", 5
CATCH
    ; save data stack pointer
    txa
    jsr pushya
    jsr TO_R
    ; save previous handler
    jsr EXCEPTION_HANDLER
    jsr FETCH
    jsr TO_R
    ; set current handler
    stx W
    tsx
    txa
    ldx W
    jsr pushya
    jsr EXCEPTION_HANDLER
    jsr STORE
    ; execute returns if no THROW
    jsr EXECUTE
    ; restore previous handler
    jsr R_TO
    jsr EXCEPTION_HANDLER
    jsr STORE
    ; discard saved stack pointer
    jsr R_TO
    inx ; drop
    ; normal completion
    jmp ZERO

+BACKLINK "throw", 5
THROW
    lda LSB,x
    ora MSB,x
    bne +
    ; 0 throw is no-op
    inx
    rts
+   lda _EXCEPTION_HANDLER + 1
    beq .print_error_and_abort

    ; restore previous return stack
    jsr EXCEPTION_HANDLER
    jsr FETCH
    stx W
    lda LSB,x
    tax
    txs
    ldx W
    inx

    ; restore previous handler
    jsr R_TO
    jsr EXCEPTION_HANDLER
    jsr STORE

    ; exc# on return stack
    jsr R_TO
    jsr SWAP
    jsr TO_R

    ; restore stack
    lda LSB,x
    tax
    inx
    jsr R_TO
    rts

.print_error_and_abort
    lda MSB,x
    cmp #-1
    bne .unknown_exception
    lda LSB,x
    cmp #-13 ; Undefined word is printed before THROW.
    beq .cr_and_abort
    cmp #-37 ; File I/O errors are printed before THROW.
    beq .cr_and_abort
    cmp #-2 ; abort"
    bne +
    jsr .get_abort_string
    jmp .type_and_abort
+   jsr .get_system_exception_string
    jsr COUNT
.type_and_abort
    jsr RVS
    jsr TYPE
.cr_and_abort
    jsr CR
    ldx #X_INIT
    jmp QUIT

; It is a bit cheesy to use a hardcoded list, but it works.
; A linked list would be more flexible.
.get_system_exception_string
    cmp #-1
    bne +
    +VALUE .abort_string
+   cmp #-4
    bne +
    +VALUE .stack_underflow
+   cmp #-8
    bne +
    +VALUE .mem_full
+   cmp #-10
    bne +
    +VALUE .div_error
+   cmp #-16
    bne +
    +VALUE .no_word
+   cmp #-28
    bne .unknown_exception
    +VALUE .user_interrupt

.unknown_exception
    jsr RVS
    jsr DOT
    lda #'e'
    jsr PUTCHR
    lda #'r'
    jsr PUTCHR
    jsr PUTCHR
    jmp .cr_and_abort

.get_abort_string
.msg_lsb = * + 1
    lda #0
.msg_msb = * + 1
    ldy #0
    jsr pushya
.msg_len = * + 1
    lda #0
    ldy #0
    jmp pushya

.abort_string
    !byte 5
    !text "abort"
.stack_underflow
    !byte 5
    !text "stack"
.mem_full
    !byte 4
    !text "full"
.no_word
    !byte 7
    !text "no name"
.div_error
    !byte 2
    !text "/0" ; division by zero
.user_interrupt
    !byte 3
    !text "brk"

+BACKLINK "(abort\")", 8 ; ( addr u -- )
    lda LSB,x
    sta .msg_len
    inx
    lda LSB,x
    sta .msg_lsb
    lda MSB,x
    sta .msg_msb
    inx
    lda #-2
    jmp throw_a
