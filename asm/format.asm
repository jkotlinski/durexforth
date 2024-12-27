; <# #> HOLD SIGN # #S U. . SPACE

.hold_start = $3fc

; : <# $3fc holdp ! ;
+BACKLINK "<#", 2
LESS_NUMBER_SIGN
    lda #<.hold_start
    sta .holdp
    rts

; : #> 2drop holdp @ $3fc over - ;
+BACKLINK "#>", 2
NUMBER_SIGN_GREATER
    lda .holdp
    sta LSB+1,x
    lda #>.hold_start
    sta MSB+1,x
    lda #<.hold_start
    sec
    sbc .holdp
    sta LSB,x
    lda #0
    sta MSB,x
    rts

; : hold -1 holdp +! holdp @ c! ;
+BACKLINK "hold", 4
HOLD
    dec .holdp
    inx
    lda LSB-1,x
.holdp = * + 1
    sta .hold_start
    rts

; : sign 0< if '-' hold then ;
+BACKLINK "sign", 4
SIGN
    inx
    lda MSB-1,x
    and #$80
    bne +
    rts
+   jsr LITC
    !byte '-'
    jmp HOLD

; : # base @ ud/mod rot
; dup $a < if 7 - then $37 + hold ;
+BACKLINK "#", 1
NUMBER_SIGN
    jsr BASE
    jsr FETCH
    jsr UD_MOD
    jsr ROT
    lda LSB,x
    cmp #10
    bcs +
    sbc #6
+   clc
    adc #$37
    sta LSB,x
    jmp HOLD

; : #s # begin 2dup or while # repeat ;
+BACKLINK "#s", 2
NUMBER_SIGN_S
    jsr NUMBER_SIGN
    lda LSB,x
    ora MSB,x
    ora LSB+1,x
    ora MSB+1,x
    bne NUMBER_SIGN_S
    rts

; : u. 0 <# #s #> type space ;
+BACKLINK "u.", 2
    jsr ZERO
    jsr LESS_NUMBER_SIGN
    jsr NUMBER_SIGN_S
    jsr NUMBER_SIGN_GREATER
    jsr TYPE
    jmp SPACE

; : . dup abs 0 <# #s rot sign #>
; type space ;
+BACKLINK ".", 1
DOT
    jsr DUP
    jsr ABS
    jsr ZERO
    jsr LESS_NUMBER_SIGN
    jsr NUMBER_SIGN_S
    jsr ROT
    jsr SIGN
    jsr NUMBER_SIGN_GREATER
    jsr TYPE
    jmp SPACE

+BACKLINK "space", 5
SPACE
    lda #' '
    jmp PUTCHR
