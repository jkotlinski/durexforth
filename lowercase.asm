CHAR_TO_LOWERCASE ; ( a -- a )
    sta .save
    sec
    sbc #'a' + $80
    cmp #1 + 'z' - 'a'
    bcs +
    adc #'a'
    rts
+
.save = * + 1
    lda #0
    sec
    sbc #'A'
    cmp #1 + 'z' - 'a'
    bcs +
    adc #'a'
    rts
+
    lda .save
    rts
