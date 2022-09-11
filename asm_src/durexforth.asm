; ACME assembler

!cpu 6510
!to "durexforth.prg", cbm	; set output file and format

* = $801

!byte $b, $08, $a, 0, $9E, $32, $30, $36, $31, 0, 0, 0 ; basic header

;; Word flags
F_IMMEDIATE = $80
; When set, calls to the word will not be subject to tail call elimination.
; I.e., "jsr WORD + rts" will not be replaced by "jmp WORD".
F_NO_TAIL_CALL_ELIMINATION = $40
STRLEN_MASK = $1f

TIB = $200

; Zeropage

; Parameter stack
; The x register contains the current stack depth.
; It is initially 0 and decrements when items are pushed.
; The parameter stack is placed in zeropage to save space.
; (E.g. lda $FF,x takes less space than lda $FFFF,x)
; We use a split stack that store low-byte and high-byte
; in separate ranges on the zeropage, so that popping and
; pushing gets faster (only one inx/dex operation).
X_INIT = 0
MSB = $73 ; high-byte stack placed in [$3b .. $72]
LSB = $3b ; low-byte stack placed in [3 .. $3a]

W = $8b ; rnd seed
W2 = $8d ; rnd seed
W3 = $9e ; tape error log

OP_JMP = $4c
OP_JSR = $20
OP_RTS = $60
OP_INX = $e8

PUTCHR = $ffd2 ; put char

K_RETURN = $d
K_CLRSCR = $93
K_SPACE = ' '

; PLACEHOLDER_ADDRESS instances are overwritten using self-modifying code.
; It must end in 00 for situations where the Y register is used as the LSB of the address.
PLACEHOLDER_ADDRESS = $1200

!ct pet

; -------- program start

    tsx
    stx INIT_S
    ldx #X_INIT

    jsr quit_reset

    jsr PAGE

    lda	#%00010110 ; lowercase
    sta	$d018

_START = * + 1
    jsr load_base

; ----------- macros

!set WORDLIST_BASE = $9fff
!set __LATEST = WORDLIST_BASE

!set BACK = *
* = __LATEST
!byte 0
* = BACK

!macro BACKLINK .name , .namesize {
    !set __LATEST = __LATEST - 3 - len(.name)
    !set .xt = *
    * = __LATEST
    !byte .namesize
    !text .name
	!word .xt
    * = .xt
}

!macro VALUE .word {
    lda	#<.word
    ldy	#>.word
    jmp pushya
}

; ---------- words

    +BACKLINK "pushya", 6
pushya
    dex
    sta	LSB, x
    sty	MSB, x
    rts

    +BACKLINK "0", 1
ZERO
    lda	#0
    tay
    jmp pushya

    +BACKLINK "1", 1
ONE
    +VALUE 1

; START - points to the code of the startup word.
    +BACKLINK "start", 5
    +VALUE	_START

    +BACKLINK "msb", 3
    +VALUE	MSB

    +BACKLINK "lsb", 3
    +VALUE	LSB

!src "core.asm"
!src "math.asm"
!src "move.asm"
!src "interpreter.asm"
!src "compiler.asm"
!src "control.asm"
!src "io.asm"
!src "lowercase.asm"
!src "disk.asm"

BOOT_STRING
!src "../build/version.asm"
PRINT_BOOT_MESSAGE
    ldx #0
-   lda BOOT_STRING,x
    jsr PUTCHR
    inx
    cpx #(PRINT_BOOT_MESSAGE - BOOT_STRING)
    bne -
    jsr CR
    jmp QUIT

; LATEST - points to the most recently defined dictionary word.

    +BACKLINK "latest", 6
LATEST
LATEST_LSB = * + 1
LATEST_MSB = * + 3
    +VALUE	__LATEST
; ALL CONTENTS BELOW LATEST WILL BE OVERWRITTEN!!!

load_base
    lda #<PRINT_BOOT_MESSAGE
    sta _START
    lda #>PRINT_BOOT_MESSAGE
    sta _START+1
    dex
    dex
    lda #<basename
    sta LSB+1, x
    lda #>basename
    sta MSB+1, x
    lda #(basename_end - basename)
    sta LSB,x
    jsr INCLUDED
    jmp interpret_loop

basename
!text	"base"
basename_end
