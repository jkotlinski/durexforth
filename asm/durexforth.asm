; ACME assembler

!cpu 6510
!to "durexforth.prg", cbm	; set output file and format
!ct pet

; Opcodes.
OP_JMP = $4c
OP_JSR = $20
OP_RTS = $60
OP_INX = $e8

; CHROUT keys.
K_RETURN = $d
K_CLRSCR = $93
K_SPACE = ' '

; Addresses.
LSB = $3b ; low-byte stack placed in [3 .. $3a]
MSB = $73 ; high-byte stack placed in [$3b .. $72]
W = $8b ; rnd seed        \  Temporary work area
W2 = $8d ; rnd seed        ) available for words.
W3 = $9e ; tape error log /  Each two bytes.
TIB = $200 ; text input buffer
PROGRAM_BASE = $801
;HERE_POSITION = $801 + assembled program (defined below)
WORDLIST_BASE = $9fff
PUTCHR = $ffd2 ; kernal CHROUT routine

; Parameter Stack
; ---------------

; The x register contains the current stack depth.
; It is initially 0 and decrements when items are pushed.
; The parameter stack is placed in zeropage to save space.
; (E.g. lda $FF,x takes less space than lda $FFFF,x)
; We use a split stack that store low-byte and high-byte
; in separate ranges on the zeropage, so that popping and
; pushing gets faster (only one inx/dex operation).

X_INIT = 0

; Dictionary
; ----------

; Grows backwards from WORDLIST_BASE. Each entry has one
; byte of flag bits + name length, followed by the bytes of
; the word's name, and a two-byte "execution token," the
; address of its code. The address of a dictionary entry is
; called the word's "name token."

STRLEN_MASK = $1f
F_IMMEDIATE = $80 ; interpret the word even in compiler STATE
F_NO_TAIL_CALL_ELIMINATION = $40
; Exempt this word from tail call elimination i.e.
; "jsr WORD + rts" will not be replaced by "jmp WORD".

* = WORDLIST_BASE

!byte 0 ; zero name length signals end of dictionary.

!set __LATEST = WORDLIST_BASE
!macro BACKLINK .name , .namesize {
    !set __LATEST = __LATEST - 3 - len(.name)
    !set .xt = *
    * = __LATEST
    !byte .namesize
    !text .name
    !word .xt
    * = .xt
}

; Program Space
; -------------

; Main assembly starts at PROGRAM_BASE, then the assembled
; compiler begins writing at HERE_POSITION, to which we
; assemble a startup routine that we're okay with being
; overwritten.

; PLACEHOLDER_ADDRESSes are assembled into the instruction
; stream then self-modified by the running program. Low
; byte must be 0 for situations where the Y register is
; used instead.
PLACEHOLDER_ADDRESS = $1200

* = PROGRAM_BASE

!byte $b, $08, $a, 0, $9E, $32, $30, $36, $31, 0, 0, 0
; basic header, and program entry:

    tsx
    stx INIT_S
    ldx #X_INIT

    jsr quit_reset

    jsr PAGE

    lda	#%00010110 ; lowercase
    sta	$d018

_START = * + 1
    jsr load_base

    ; begin word definitions

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

!macro VALUE .word {
    lda	#<.word
    ldy	#>.word
    jmp pushya
}

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

; Word Definitions
; ----------------

; nonword labels in (parens).

;defined above and below in this file:
; pushya 0 1 start msb lsb latest

!src "core.asm"
; drop swap dup ?dup nip over 2dup 1+ 1- + = 0= and
; ! @ c! c@ count < > max min tuck >r r> r@ bl pick
; depth within fill base 2* rot (compile_a)

!src "math.asm"
; u< - um* um/mod m+ invert negate abs * dnegate m*
; 0< s>d fm/mod /mod ud/mod

!src "move.asm"
; move

!src "interpreter.asm"
; quit execute interpret notfound ' find find-name
; >xt parse-name word evaluate abort /string dowords
; (quit_reset interpret_loop INIT_S)

!src "compiler.asm"
; c, , [ ] ; immediate state latestxt : header lit
; litc compile, literal here dodoes

!src "control.asm"
; if then begin while repeat branch 0branch unloop exit

!src "io.asm"
; emit page rvs cr type key? key refill source
; source-id >in getc char ioabort

!src "lowercase.asm"
; (CHAR_TO_LOWERCASE)

!src "disk.asm"
; device rderr loadb saveb included (CHROUT)

; refer to base.fs for the rest

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
    ldx #X_INIT
    jmp QUIT

; LATEST - points to the most recently defined dictionary word.

    +BACKLINK "latest", 6
LATEST
LATEST_LSB = * + 1
LATEST_MSB = * + 3
    +VALUE	__LATEST

HERE_POSITION ; everything below this will be overwritten!

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
    lda #>(interpret_loop-1)
    pha
    lda #<(interpret_loop-1)
    pha
    jmp INCLUDED

basename
!text	"base"
basename_end
