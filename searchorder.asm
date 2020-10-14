;{{{ The MIT License
;
;Copyright (c) 2020 Poindexter Frink
;
;Permission is hereby granted, free of charge, to any person obtaining a copy
;of this software and associated documentation files (the "Software"), to deal
;in the Software without restriction, including without limitation the rights
;to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
;copies of the Software, and to permit persons to whom the Software is
;furnished to do so, subject to the following conditions:
;
;The above copyright notice and this permission notice shall be included in
;all copies or substantial portions of the Software.
;
;THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
;IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
;FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
;AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
;LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
;OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
;THE SOFTWARE. }}}

; DEFINITIONS FORTH-WORDLIST GET-CURRENT GET-ORDER SET-CURRENT SET-ORDER
; WORDLIST

WIDS
    !fill 16, 0
_ORDER
    !byte 1

CONTEXT
    !byte 1
    !fill 7, 0

    +BACKLINK
    !byte	14
    !text	"forth-wordlist"
    +VALUE 1

    +BACKLINK
    !byte   9
    !text   "get-order"
    lda #0
    sta W
    ldy _ORDER
    beq .gdone_count
.gpush_wid
    dey
    dex
    lda CONTEXT,y
    sta LSB,x
    lda #0 
    sta MSB,x
    inc W
    cpy #0
    bne .gpush_wid
.gdone_count
    dex
    sta MSB,x
    ldy W
    sty LSB,x
    rts

    +BACKLINK
    !byte	9
    !text	"set-order"

    lda LSB,x
    cmp #$ff
    bne .set_not_negative
    lda MSB,x
    cmp #$ff
    bne .set_not_negative
    
    lda #1 
    sta CONTEXT
    lda #1
    sta _ORDER
    inx
    rts
.set_not_negative
    jsr DUP
    jsr ZEQU
    lda LSB,x
    beq .set_not_zero
    lda #0
    sta _ORDER
    inx
    inx
    rts
.set_not_zero
    inx
    lda LSB,x
    sta _ORDER
    inx
    ldy #0
.set_pull_wid
    lda LSB,x
    sta CONTEXT,y
    inx
    iny
    cpy _ORDER
    bne .set_pull_wid
    rts

    +BACKLINK
    !byte   11
    !text   "set-current"
SET_CURRENT
    ; save the current latest pointer
    lda CURRENT
    sec
    sbc #1
    asl
    tay
    lda _LATEST
    sta WIDS,y
    lda _LATEST+1
    sta WIDS+1,y
    ; update current
    lda LSB,x
    sta CURRENT
    ; update the latest pointer
    sec
    sbc #1
    asl
    tay
    lda WIDS,y
    sta _LATEST
    lda WIDS+1,y
    sta _LATEST+1
    inx
    rts

    +BACKLINK
    !byte   11
    !text   "get-current"
    dex
    lda CURRENT
    sta LSB,x
    lda #0
    sta MSB,x
    rts
CURRENT
    !byte 1
NEXT_WID
    !byte 2
    
    +BACKLINK
    !byte 8
    !text "wordlist"
    dex
    lda NEXT_WID
    inc NEXT_WID
    sta LSB,x
    lda #0
    sta MSB,x
    rts
    
    +BACKLINK
    !byte 11
    !text "definitions"
    dex
    lda CONTEXT
    sta LSB,x
    lda #0
    sta MSB,x
    jmp SET_CURRENT

