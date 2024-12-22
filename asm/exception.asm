; CATCH THROW

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
    jsr RVS
    lda LSB,x
    cmp #-2 ; ABORT"
    bne +
    inx
    jsr TYPE
    jmp .print_cr_and_abort
+   ; TODO print error number
    lda #'e'
    jsr PUTCHR
    lda #'r'
    jsr PUTCHR
    jsr PUTCHR
.print_cr_and_abort
    jsr CR
    ldx #X_INIT
    jmp QUIT
