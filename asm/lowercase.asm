CHAR_TO_LOWERCASE ; ( a -- a )
    cmp #'A'
    bcc +
    cmp #'Z' + 1
    bcs +
    sec
    sbc #'A' - 'a'
+   rts
