CHAR_TO_LOWERCASE ; ( a -- a )
    cmp #'A'
    bcc +
    cmp #'Z' + 1
    bcs +
    sbc #'A' - 'a' - 1
+   rts
