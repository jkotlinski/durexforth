CHAR_TO_LOWERCASE ; ( a -- a )
    cmp #'Z' + 1
    bcs +
    cmp #'A'
    bcc +
    sec
    sbc #'A' - #'a'
+   rts
