; CATCH THROW (ABORT")

EXCEPTION_HANDLER
    +VALUE _EXCEPTION_HANDLER
_EXCEPTION_HANDLER
    !word 0

TO_ERR ; ( addr u -- )
    lda LSB,x
    sta .msg_len
    inx
    lda LSB,x
    sta .msg_lsb
    lda MSB,x
    sta .msg_msb
    inx
    rts

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
    jsr RVS
    inx
    lda MSB-1,x
    cmp #-1
    beq .print_system_error
    jsr .get_generic_error_string
    jmp .print_error_string
.print_system_error
    lda LSB-1,x
    cmp #-2
    bne +
    jsr .get_custom_error_string
    jmp .print_error_string
+   jsr .get_error_string_from_code
    jsr COUNT
.print_error_string
    jsr TYPE
    jsr CR
    ldx #X_INIT
    jmp QUIT

; It is a bit cheesy to use a hardcoded list, but it works.
; A linked list would be more flexible.
.get_error_string_from_code
    cmp #-1
    bne +
    +VALUE .abort
+   cmp #-4
    bne +
    +VALUE .stack_underflow
+   cmp #-8
    bne +
    +VALUE .mem_full
+   cmp #-10
    bne +
    +VALUE .div_error
+   cmp #-13
    bne +
    jsr .get_custom_error_string
    jsr TYPE
    +VALUE .not_found
+   cmp #-16
    bne +
    +VALUE .no_word
+   cmp #-28
    bne +
    +VALUE .user_interrupt
+   cmp #-37
    bne .get_generic_error_string
    +VALUE .io_error
.get_generic_error_string
    +VALUE .generic_error

.get_custom_error_string
.msg_lsb = * + 1
    lda #0
.msg_msb = * + 1
    ldy #0
    jsr pushya
.msg_len = * + 1
    lda #0
    ldy #0
    jmp pushya

.abort
    !byte 5
    !text "abort"
.stack_underflow
    !byte 5
    !text "stack"
.mem_full
    !byte 4
    !text "full"
.not_found
    !byte 1
    !text "?"
.no_word
    !byte 7
    !text "no name"
.div_error
    !byte 2
    !text "/0" ; division by zero
.io_error
    !byte 3
    !text "i/o"
.user_interrupt
    !byte 3
    !text "brk"
.generic_error
    !byte 3
    !text "err"

+BACKLINK "(abort\")", 8 ; ( addr u -- )
    jsr TO_ERR
    lda #-2
    jmp throw_a
