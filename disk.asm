;{{{ The MIT License
;
;Copyright (c) 2008 Johan Kotlinski, Mats Andren
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

; DEVICE OPENW CLOSEW LOADB SAVEB INCLUDED

READST = $ffb7
SETLFS = $ffba
SETNAM = $ffbd
OPEN = $ffc0
CLOSE = $ffc3
CHKIN = $ffc6
CHKOUT = $ffc9
CLRCHN = $ffcc
CHRIN = $ffcf
CHROUT = $ffd2
LOAD = $ffd5
SAVE = $ffd8

; -----

    +BACKLINK
    !byte 6
    !text	"device" ; ( deviceno -- )
    lda LSB,x
    sta $ba
    inx
    rts

; CLOSEW ( file# -- )
    +BACKLINK
    !byte 6
    !text	"closew"
CLOSEW
    lda LSB,x
    sta W
    stx	W2
    jsr	CLOSE
    ldx	W
    jsr	CHKOUT
    ldx W2
    inx
    rts

_errorchread
        LDA #$00      ; no filename
        tax
        tay
        JSR SETNAM
        LDA #$0F      ; file number 15
        LDX $BA       ; last used device number
        BNE +
        LDX #$08      ; default to device 8
+	    LDY #$0F      ; secondary address 15 (error channel)
        JSR SETLFS

        JSR OPEN
        BCS .error    ; if carry set, the file could not be opened

        LDX #$0F      ; filenumber 15
        JSR CHKIN     ; file 15 now used as input

        LDY #$00
.loop   JSR READST    ; read status byte
        BNE geof      ; either EOF or read error
        JSR CHRIN     ; get a byte from file
        JSR CHROUT    ; print byte to screen
        JMP .loop     ; next byte

geof
.error
        ; Accumulator contains BASIC error code

        ; most likely error:
        ; A = $05 (DEVICE NOT PRESENT)

        ; ... error handling for open errors ...

.glose
        LDA #$0F      ; filenumber 15
        JSR CLOSE

        LDX #$00      ; filenumber 0 = keyboard
        JMP CHKIN     ; keyboard now input device again


; LOADB ( filenameptr filenamelen dst -- endaddress ) load binary file
;  - s" base" 7000 loadb #load file to 7000
;  - returns 0 on failure, otherwise address after last written byte
    +BACKLINK
    !byte 5
    !text	"loadb"
LOADB
    txa
    pha

    lda MSB, x		; >destination
    sta load_binary_laddr_hi
    lda LSB, x		; <destination
    sta load_binary_laddr_lo

    lda LSB+1, x		; a filename length
    pha
    ldy MSB+2, x 		; y >basename
    lda LSB+2, x		; x <basename
    tax
    pla
    jsr	load_binary

    pla
    tax

    inx
    inx
load_binary_status = * + 1
    lda	#0 ; 0 = fail, ff = success
    bne	.success
    sta MSB,x
    sta LSB,x
    txa
    pha
    jsr	_errorchread
    pla
    tax
    rts
.success:
    lda $af
    sta	MSB, x
    lda $ae
    sta	LSB, x
    rts

;load_binary_base
;	lda	#basename_end - basename
;	ldx	#<basename
;	ldy	#>basename
;	; fall through...

load_binary
    jsr .disk_io_setnamsetlfs

load_binary_laddr_lo = *+1
    ldx #$ff	;<load_address
load_binary_laddr_hi = *+1
    ldy #$ff	;>load_address
    sty	load_binary_status
    lda #0		;0 = load to memory (no verify)
    jsr LOAD
    bcs .disk_io_error
    ldx #$00      ; filenumber 0 = keyboard
    jmp CHKIN     ; call CHKIN (keyboard now input device again)

.disk_io_setnamsetlfs ;reused by both loadb and saveb
    jsr SETNAM
    lda $ba		;last used device number
    and #3		;Make 0-3 possible numbers
    ora #8		;Transform to 8-B
    tax
    lda #1
    ldy #0		;if load: 0 = load to new address, if save: 0 = dunno, but okay...
    jmp SETLFS	;End with JMP instead of jsr/rts to save a jsr/rts pair...

.disk_io_error
    ; Accumulator contains BASIC error code

    ;... error handling ...
    ldx #$00      ; filenumber 0 = keyboard
    stx	load_binary_status
    jsr CHKIN     ; call CHKIN (keyboard now input device again)
    rts

; SAVEB (save binary file)
;  - 7000 71ae s" base" saveb #save file from 7000 to 71ae (= the byte AFTER the last byte in the file)
    +BACKLINK
    !byte 5
    !text	"saveb"
SAVEB
    stx W

    lda	$ae
    pha
    lda	$af
    pha

    lda LSB+3, x		; range begin lo
    sta $c1
    lda MSB+3, x		; range begin hi
    sta $c2

    lda LSB+2, x		; range end lo
    sta save_binary_srange_end_lo
    lda MSB+2, x		; range end hi
    sta save_binary_srange_end_hi

    lda LSB, x		; a filename length
    pha
    ldy MSB+1, x 		; y basename hi
    lda LSB+1, x		; x basename lo
    tax
    pla

    jsr .disk_io_setnamsetlfs

    ;This should point to the byte AFTER the last byte in the file.
save_binary_srange_end_lo = *+1
    ldx #$ff	;load_address lo
save_binary_srange_end_hi = *+1
    ldy #$ff	;load_address hi
    lda #$c1	;tell routine that start address is located in $c1/$c2
    jsr SAVE
    jsr _errorchread

    pla
    sta	$af
    pla
    sta	$ae

    ldx W
    inx
    inx
    inx
    inx
    rts

; OPENW ( strptr strlen file# ) open file for writing
    +BACKLINK
    !byte 5
    !text	"openw"
OPENW
    lda LSB,x
    sta W ; fileno
    stx	W2

    lda	LSB+1, x
    ldy	MSB+2, x
    pha
    lda	LSB+2, x
    tax
    pla

    jsr	SETNAM
    lda	W ; file number
    ldx	$ba ; last used device#
    tay ; secondary address
    jsr	SETLFS
    jsr	OPEN
    bcc	+
    jsr .close
    jmp ++
+
    ldx	W ; file number
    jsr	CHKOUT
++
    ldx	W2
    inx
    inx
    inx
    rts

.close
    lda $b8 ; current file
    jsr CLOSE
    jmp CLRCHN

    +BACKLINK
    !byte 8
    !text	"included"
INCLUDED
    lda	LSB, x
    sta .filelen
    lda MSB+1, x
    sta .namehi
    lda LSB+1, x
    sta .namelo
    inx
    inx

    jsr SAVE_INPUT

    ; Is TIB_PTR pointing to TIB?
    lda	TIB_PTR+1
    cmp #>TIB
    bne .reset_tib_ptr_to_tib

    ; ...if yes: Adjust TIB_PTR to point past the current TIB content, to avoid clobbering.
    lda TO_IN_W
    cmp TIB_SIZE
    beq .load_file ; If TIB is already consumed, no need to do anything.
    lda TIB_SIZE
    clc
    adc TIB_PTR
    sta TIB_PTR
    jmp .load_file

    ; ...if no: Reset TIB_PTR so that it points to TIB again.
.reset_tib_ptr_to_tib:
    lda #<TIB
    sta TIB_PTR
    lda #>TIB
    sta TIB_PTR+1

.load_file:

    txa
    pha

.filelen = * + 1
    lda #0
.namehi = * + 1
    ldy #0
.namelo = * + 1
    ldx #0

    ; open file
    jsr	SETNAM
    lda #0
    sta SOURCE_ID_MSB
    ldy SOURCE_ID_LSB
    iny
    tya
    ora #8
    tay
    sty SOURCE_ID_LSB

    ldx	$ba ; last used device#
    jsr	SETLFS
    jsr	OPEN
    bcc	+
    jsr .close
    jmp ABORT
+
    ldx	SOURCE_ID_LSB ; file number
    jsr	CHKIN

    ; Skips load address. It is tempting to keep the source
    ; code as .SEQ files instead of .PRG to avoid this step.
    ; However, the advantage with .PRG is that loading/saving
    ; files from text editor can be dramatically speeded up
    ; by fast loader cartridges such as Retro Replay.
    JSR CHRIN     ; get a byte from file
    JSR CHRIN     ; get a byte from file

    jsr READST
    beq +
    jsr .close
    jsr _errorchread
    jmp ABORT
+
    pla
    tax

    ; interpret until EOF
-   jsr REFILL
    lda READ_EOF
    bne +
    jsr interpret_tib
    jmp -
+   jmp interpret_tib
