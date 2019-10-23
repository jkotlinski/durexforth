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

; ACME assembler

!cpu 6510
!to "durexforth.prg", cbm	; set output file and format

* = $801

!byte $b, $08, $a, 0, $9E, $32, $30, $36, $31, 0, 0, 0 ; basic header

;; Word flags
F_IMMEDIATE = $80
F_HIDDEN = $40
; When set, calls to the word will not be subject to tail call elimination.
; I.e., "jsr WORD + rts" will not be replaced by "jmp WORD".
F_NO_TAIL_CALL_ELIMINATION = $20
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
PLACEHOLDER_ADDRESS = $1234

!ct pet

; -------- program start

    lda 1
    pha
    lda $318
    pha
    lda $319
    pha
    tsx
    stx INIT_S
    ldx #X_INIT

    jsr quit_reset

    jsr PAGE

    lda	#%00010110 ; lowercase
    sta	$d018

_START = * + 1
    jsr load_base

restore_handler
    cli
    jmp QUIT

; ----------- macros

!set LINK = 0

!macro BACKLINK {
    ; it's tempting to add the string as a macro parameter,
    ; but this does not seem to be supported by ACME.
    !word	LINK
    !set	LINK = * - 2
}

!macro VALUE .word {
    lda	#<.word
    ldy	#>.word
    jmp pushya
}

; ---------- words

    +BACKLINK
    !byte 6
    !text	"pushya"
pushya
    dex
    sta	LSB, x
    sty	MSB, x
    rts

    +BACKLINK
    !byte 1
    !text	"0"
ZERO
    lda	#0
    tay
    jmp pushya

    +BACKLINK
    !byte 1
    !text	"1"
ONE
    +VALUE 1

; START - points to the code of the startup word.
    +BACKLINK
    !byte 5
    !text	"start"
    +VALUE	_START

    +BACKLINK
    !byte 3
    !text	"msb"
    +VALUE	MSB

    +BACKLINK
    !byte 3
    !text	"lsb"
    +VALUE	LSB

!src "core.asm"
!src "math.asm"

!src "move.asm"

; HERE - points to the next free byte of memory. When compiling, compiled words go here.
    +BACKLINK
    !byte 4
    !text	"here"
HERE
HERE_LSB = * + 1
HERE_MSB = * + 3
    +VALUE	_LATEST + 2

tmp_x
    !byte	0

!src "number.asm"

    +BACKLINK
    !byte 7
    !text	"/string"
SLASH_STRING ; ( addr u n -- addr u )
    jsr DUP
    jsr TO_R
    jsr MINUS
    jsr SWAP
    jsr R_TO
    jsr PLUS
    jmp SWAP

!src "interpreter.asm"
!src "compiler.asm"

    !word	LINK
    !set	LINK = * - 2
    !byte	6
    !text	"dodoes"

    ; behavior pointer address => W
    pla
    sta W
    pla
    sta W + 1

    inc W
    bne +
    inc W + 1
+

    ; push data pointer to param stack
    dex
    lda W
    clc
    adc #2
    sta LSB,x
    lda W + 1
    adc #0
    sta MSB,x

    ldy #0
    lda (W),y
    sta W2
    iny
    lda (W),y
    sta W2 + 1
    jmp (W2)

!src "control.asm"
!src "io.asm"
!src "lowercase.asm"
!src "disk.asm"

; LATEST - points to the most recently defined dictionary word.
    +BACKLINK
    !byte 6
    !text	"latest"
LATEST
    +VALUE	_LATEST
_LATEST
    !word	LINK

; ALL CONTENTS BELOW LATEST WILL BE OVERWRITTEN!!!

load_base
    lda #<QUIT
    sta _START
    lda #>QUIT
    sta _START+1
    dex
    dex
    lda #<basename
    sta LSB+1, x
    lda #>basename
    sta MSB+1, x
    lda #4
    sta LSB,x
    jsr INCLUDED
    jmp interpret_loop

basename
!text	"base"
basename_end
