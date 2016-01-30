; routines adapted from cc65
; original by Ullrich von Bassewitz, Christian Krueger, Greg King

SRC = W
DST = W2
LEN = W3

cmove_getparams:
	lda	LSB, x
	sta	LEN
	lda	MSB, x
	sta	LEN + 1
	lda	LSB + 1, x
	sta	DST
	lda	MSB + 1, x
	sta	DST + 1
	lda	LSB + 2, x
	sta	SRC
	lda	MSB + 2, x
	sta	SRC + 1
	rts

CMOVE_BACK
	txa
	pha
	jsr cmove_getparams
    ; copy downwards. adjusts pointers to the end of memory regions.
    lda SRC + 1
    clc
    adc LEN + 1
    sta SRC + 1
    lda DST + 1
    clc
    adc LEN + 1
    sta DST + 1
    
    ldy LEN
    bne .entry
    beq .pagesizecopy
.copybyte
    lda (SRC),y
    sta (DST),y
.entry
    dey
    bne .copybyte
    lda (SRC),y
    sta (DST),y
.pagesizecopy
    ldx LEN + 1
    beq cmove_done
.initbase
    dec SRC + 1
    dec DST + 1
    dey
    lda (SRC),y
    sta (DST),y
    dey
.copybytes
    lda (SRC),y
    sta (DST),y
    dey
    lda (SRC),y
    sta (DST),y
    dey
    bne .copybytes
    lda (SRC),y
    sta (DST),y
    dex
    bne .initbase
	jmp cmove_done

CMOVE
    txa
    pha
	jsr cmove_getparams
	ldy #0
	ldx	LEN + 1
	beq	.l2
.l1
	lda	(SRC),y ; copy byte
	sta	(DST),y
	iny
	lda	(SRC),y ; copy byte again, to make it faster
	sta	(DST),y
	iny
	bne .l1
	inc	SRC + 1
	inc DST + 1
	dex ; next 256-byte block
	bne .l1
.l2
	ldx	LEN
	beq cmove_done
.l3
	lda (SRC),y
	sta	(DST),y
	iny
	dex
	bne	.l3
cmove_done
	pla
    clc
	adc #3
	tax
	rts

    +BACKLINK
    !byte	4
    !text	"move" ; ( src dst u -- )
    jsr TO_R
    jsr TWODUP
    jsr U_LESS
    jsr R_TO
    jsr SWAP
    jsr ZBRANCH
    !word .br
    jmp CMOVE_BACK
.br = *
    jmp CMOVE
