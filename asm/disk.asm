; DEVICE RDERR LOADB SAVEB INCLUDED

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

    +BACKLINK "device", 6
    lda LSB,x
    sta $ba
    inx
    rts

    +BACKLINK "rderr", 5
_errorchread
        ; read and print error channel of the active device
        ; from https://codebase64.org/doku.php?id=base:reading_the_error_channel_of_a_disk_drive
        LDA #$00
        STA $90       ; clear STATUS flags

        LDA $BA       ; device number
        JSR $FFB1     ; call LISTEN
        LDA #$6F      ; secondary address 15 (command channel)
        JSR $FF93     ; call SECLSN (SECOND)
        JSR $FFAE     ; call UNLSN
        LDA $90       ; get STATUS flags
        BNE .devnp    ; device not present

        LDA $BA       ; device number
        JSR $FFB4     ; call TALK
        LDA #$6F      ; secondary address 15 (error channel)
        JSR $FF96     ; call SECTLK (TKSA)

.loop   LDA $90       ; get STATUS flags
        BNE .eof      ; either EOF or error
        JSR $FFA5     ; call IECIN (get byte from IEC bus)
        JSR $FFD2     ; call CHROUT (print byte to screen)
        JMP .loop     ; next byte
.eof    jmp $FFAB     ; call UNTLK

.devnp  ; print "device not present" and abort
        ldx #X_INIT-1
        lda #5
        sta LSB,x
        lda #0
        sta MSB,x
        jmp IOABORT

; LOADB ( filenameptr filenamelen dst -- endaddress ) load binary file
;  - s" base" 7000 loadb #load file to 7000
;  - returns 0 on failure, otherwise address after last written byte
    +BACKLINK "loadb", 5
LOADB
    txa
    pha
    lda $b8             ; current logical file
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
    jsr CHKIN           ; restore logical file
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
    rts

.disk_io_setnamsetlfs ;reused by both loadb and saveb
    jsr SETNAM
    ldx $ba     ; keep current device
    lda #1      ; logical file #
    ldy #0      ; if load: 0 = load to new address, if save: 0 = dunno, but okay...
    jmp SETLFS

.disk_io_error
    ; Accumulator contains BASIC error code

    ;... error handling ...
    ldx #$00      ; filenumber 0 = keyboard
    stx	load_binary_status
    jmp CLRCHN

; SAVEB (save binary file)
;  - 7000 71ae s" base" saveb #save file from 7000 to 71ae (= the byte AFTER the last byte in the file)
    +BACKLINK "saveb", 5
SAVEB
    stx W

    lda $b8             ; current logical file
    pha
    lda $ae
    pha
    lda $af
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
    sta $af
    pla
    sta $ae
    pla
    tax
    jsr CHKIN

    ldx W
    inx
    inx
    inx
    inx
    rts

    +BACKLINK "included", 8
INCLUDED
    lda	LSB, x
    sta .filelen
    lda MSB+1, x
    sta .namehi
    lda LSB+1, x
    sta .namelo
    inx
    inx

    jsr PUSH_INPUT_SOURCE

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
    ldx #-1
    sta LSB,x
    jmp IOABORT
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
    jsr _errorchread
    jmp ABORT
+
    pla
    tax

    jmp interpret_and_close
