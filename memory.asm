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
;
; INITMEM ALLOCATE FREE RESIZE

H_LATEST = $cbfe
NULL_RECORD = H_LATEST - 2

INIT_MEM
    lda #0
    ; construct null record
    sta NULL_RECORD
    sta NULL_RECORD + 1
    lda #<NULL_RECORD
    sta H_LATEST
    lda #>NULL_RECORD
    sta H_LATEST + 1
    rts

.modify_record
    !byte 0

    +BACKLINK "allocate", 8 ; ( size -- addr )
ALLOCATE
    ; record structure:
    ; [ next | flags_size | curr_size | data_0 ... data_size-1 ]
    ; next_addr = h_latest
    lda #0
    sta .modify_record
    lda H_LATEST
    sta W
    sta W2
    lda H_LATEST + 1
    sta W + 1
    sta W2 + 1
.search_continue
    ; check the list for the smallest record that fits
    ldy #1      ; msb of link, 0 = null record
    lda (W), y
    beq .search_done
    ldy #3      ; size_flags msb
    lda (W), y
    and #<FREE_MASK
    beq .next_record
    lda (W), y	; slot is free, big enough?
    eor #<FREE_MASK
    sta W3
    dey
    lda (W), y
    sec
    sbc LSB, x
    bcs +
    dec W3
    sec
+   lda W3
    sbc MSB, x
    bcc .next_record
    lda .modify_record
    beq .new_modify
    ldy #2          ; is this smaller?
    sec
    lda (W), y
    sbc (W2), y
    iny
    lda (W2), y
    eor #<FREE_MASK
    sta W3
    lda (W), y
    eor #<FREE_MASK
    sbc W3
    bcc .next_record
.new_modify
    lda #1
    sta .modify_record
    lda W
    sta W2
    lda W + 1
    sta W2 + 1
.next_record
    ldy #0
    lda (W), y
    sta W3
    iny
    lda (W), y
    sta W + 1
    lda W3
    sta W
    jmp .search_continue
.search_done
    lda .modify_record
    bne .modify
    ; append
    ; subtract the size of the allocation plus header
+   sec
    lda W2
    sbc #6
    bcs +
    sta W2
    lda W2 + 1
    sbc #0
    sta W2 + 1
    lda W2
+   sbc LSB, x
    sta W2
    bcs +
    dec W2 + 1
+   lda W2 + 1
    sbc MSB, x
    sta W2 + 1      ; W now contains the start address of the record
    ldy #0
    lda H_LATEST    ; store the backlink
    sta (W2), y
    iny
    lda H_LATEST + 1
    sta (W2), y
    iny
    lda W2          ; update H_LATEST
    sta H_LATEST
    lda W2 + 1
    sta H_LATEST + 1
    lda LSB, x      ; store the size
    sta (W2), y
    iny
    lda MSB, x
    sta (W2), y
    iny
.size_and_return
    lda LSB, x      ; store the size
    sta (W2), y
    iny
    lda MSB, x
    sta (W2), y
    iny
    tya             ; return the address of the data field
    clc
    adc W2
    sta LSB, x
    lda #0
    adc W2 + 1
    sta MSB, x
    rts
.modify
    ldy #3          ; clear the free flag
    lda (W2), y
    eor #<FREE_MASK
    sta (W2), y
    iny
    jmp .size_and_return

    +BACKLINK "free", 4 ; ( addr -- )
FREE
    inx
    lda MSB-1, x
    sta W + 1
    lda LSB-1, x
    sec
    sbc #3
    sta W
    bcs +
    dec W + 1
+   ldy #0
    lda (W), y
    ora #<FREE_MASK
    sta (W), y
    rts

