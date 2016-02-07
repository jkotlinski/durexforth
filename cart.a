!cpu 6510
!ct raw
!to "build/cart.bin", plain	; set output file and format
	
* = $8000
     !word coldstart            ; coldstart vector
     !word warmstart            ; warmstart vector
     !byte $C3,$C2,$CD,$38,$30  ; "CBM80". Autostart string
 
coldstart
    sei
    stx $d016
    jsr $fda3 ;Prepare IRQ
     
    ; init system constants ($fd50)
    lda #0
    tay
-   sta 2,y
    sta $200,y
    sta $300,y
    iny
    bne -
    lda #4
    sta $288
    
    jsr $fd15 ;Init I/O
    jsr $ff5b ;Init video
 
warmstart
    sei
    lda #<durexforth_bin
    sta $8b
    lda #>durexforth_bin
    sta $8c
    lda #8
    sta $8e
    sta $ba ; last device
    lda #$d
    sta $8d
    ldy #0
-
    lda ($8b),y
    sta ($8d),y
    iny
    lda ($8b),y
    sta ($8d),y
    iny
    lda ($8b),y
    sta ($8d),y
    iny
    lda ($8b),y
    sta ($8d),y
    iny
    bne -
    inc $8c
    inc $8e
    lda $8c
    cmp #$c0
    bne -

    cli
    ldx #0
    lda $de00 ; $a000-$bfff = RAM (Simons' basic)
    jmp $80d

durexforth_bin
    !binary "build/durexforth",,$e
 
* = $bfff                     ; fill up to -$9fff (or $bfff if 16K)
     !byte 0
