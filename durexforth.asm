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

    +BACKLINK
    !byte 6
    !text	"pushya"
pushya
    dex
    sta	LSB, x
    sty	MSB, x
    rts

!macro VALUE .word {
    lda	#<.word
    ldy	#>.word
    jmp pushya
}

; ---------- words

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

; FILL ( start len char -- )
    +BACKLINK
    !byte	4
    !text	"fill"
FILL
    lda	LSB, x
    tay
    lda	LSB + 2, x
    sta	.fdst
    lda	MSB + 2, x
    sta	.fdst + 1
    lda	LSB + 1, x
    eor	#$ff
    sta	W
    lda	MSB + 1, x
    eor	#$ff
    sta	W + 1
    inx
    inx
    inx
-
    inc	W
    bne	+
    inc	W + 1
    bne	+
    rts
+
.fdst = * + 1
    sty	PLACEHOLDER_ADDRESS ; replaced with start

    ; advance
    inc	.fdst
    bne	-
    inc	.fdst + 1
    jmp	-

!src "move.asm"

; ---------- variables

    +BACKLINK
    !byte 9
    !text	"source-id"
SOURCE_ID_LSB = * + 1
SOURCE_ID_MSB = * + 3
    ; -1 : string (via evaluate)
    ; 0 : keyboard
    ; 1+ : file id
    +VALUE	0

    +BACKLINK
    !byte 8
    !text	"evaluate" ; ( addr size -- )
EVALUATE
    jsr SAVE_INPUT
    lda LSB + 1, x
    sta TIB_PTR
    lda MSB + 1, x
    sta TIB_PTR + 1
    jsr PLUS
    lda LSB, x
    sta .bufend
    lda MSB, x
    sta .bufend + 1
    inx

    jsr evaluate_get_new_line
    ldy #$ff
    sty SOURCE_ID_LSB
    sty SOURCE_ID_MSB

.eval_loop
    lda TIB_PTR + 1
    cmp .bufend + 1
    bcc +
    lda TIB_PTR
    cmp .bufend
    bcc +
    jmp RESTORE_INPUT ; exit
+
    jsr interpret_tib
    jsr REFILL
    jmp .eval_loop

evaluate_get_new_line
    ldy #0
    sty TO_IN_W
    sty TO_IN_W + 1

    ; Determines TIB_SIZE.
    lda TIB_PTR
    sta W
    lda TIB_PTR + 1
    sta W + 1
.findtibsizeloop
    lda .bufend + 1
    cmp W + 1
    bcc .foundeol
    bne +
    lda W
    cmp .bufend
    bcs .foundeol
+
    ldy #0
    lda (W),y
    cmp #K_RETURN
    beq .foundeol

    inc W
    bne +
    inc W + 1
+
    jmp .findtibsizeloop

.foundeol
    lda W
    sec
    sbc TIB_PTR
    sta TIB_SIZE
    lda W + 1
    sbc TIB_PTR + 1
    sta TIB_SIZE + 1
    rts

evaluate_consume_tib
    lda TIB_PTR
    clc
    adc TIB_SIZE
    sta TIB_PTR
    lda TIB_PTR + 1
    adc TIB_SIZE + 1
    sta TIB_PTR + 1

    inc TIB_PTR ; skip cr
    bne +
    inc TIB_PTR + 1
+   rts

.bufend
    !word 0

SAVE_INPUT_STACK
    !fill 9*4
SAVE_INPUT_STACK_DEPTH
    !byte 0

push_input_stack
    ldy SAVE_INPUT_STACK_DEPTH
    sta SAVE_INPUT_STACK, y
    inc SAVE_INPUT_STACK_DEPTH
    rts

pop_input_stack
    dec SAVE_INPUT_STACK_DEPTH
    ldy SAVE_INPUT_STACK_DEPTH
    lda SAVE_INPUT_STACK, y
    rts

SAVE_INPUT
    lda READ_EOF
    jsr push_input_stack
    lda #0
    sta READ_EOF
    lda TO_IN_W
    jsr push_input_stack
    lda TO_IN_W+1
    jsr push_input_stack
    lda SOURCE_ID_LSB
    jsr push_input_stack
    lda SOURCE_ID_MSB
    jsr push_input_stack
    lda TIB_PTR
    jsr push_input_stack
    lda TIB_PTR+1
    jsr push_input_stack

    lda TO_IN_W
    cmp TIB_SIZE
    beq +
    ; Temporarily moves the input buffer to avoid clobbering.
    lda TIB_SIZE
    clc
    adc TIB_PTR
    sta TIB_PTR
+

    lda TIB_SIZE
    jsr push_input_stack
    lda TIB_SIZE+1
    jmp push_input_stack

RESTORE_INPUT
    jsr pop_input_stack
    sta TIB_SIZE+1
    jsr pop_input_stack
    sta TIB_SIZE
    jsr pop_input_stack
    sta TIB_PTR+1
    jsr pop_input_stack
    sta TIB_PTR
    jsr pop_input_stack
    sta SOURCE_ID_MSB
    jsr pop_input_stack
    sta SOURCE_ID_LSB
    jsr pop_input_stack
    sta TO_IN_W+1
    jsr pop_input_stack
    sta TO_IN_W
    jsr pop_input_stack
    sta READ_EOF
    rts

; HERE - points to the next free byte of memory. When compiling, compiled words go here.
    +BACKLINK
    !byte 4
    !text	"here"
HERE
HERE_LSB = * + 1
HERE_MSB = * + 3
    +VALUE	_LATEST + 2

    +BACKLINK
    !byte 1
    !text	"0"
ZERO
    lda	#0
    tay
    jmp pushya

; ------------ i/o

tmp_x
    !byte	0

    +BACKLINK
    !byte	4
    !text	"getc"
    jsr GET_CHAR_FROM_TIB
    bne +
    jsr REFILL
    lda #K_RETURN
+   ldy #0
    jmp pushya

    +BACKLINK
    !byte	4
    !text	"char" 
CHAR ; ( name -- char )
-   jsr PARSE_NAME
    lda LSB,x
    bne +
    inx
    inx
    jsr REFILL
    jmp -
+   inx
    jmp FETCHBYTE

GET_CHAR_BLOCKING
    stx	tmp_x
-
    jsr	CHRIN ; wastes x
    pha
    jsr	READST
    sta READ_EOF
    pla
    ora #0
    beq -

    ldx tmp_x
    rts

    +BACKLINK
    !byte	3
    !text	">in"
TO_IN
    +VALUE TO_IN_W
TO_IN_W
    !word 0

; WORD ( delim -- strptr )
    +BACKLINK
    !byte      4
    !text      "word"
WORD
    jsr ZERO
    jsr HERE
    jsr STOREBYTE

    ; skips initial delimiters.
-   jsr GET_CHAR_FROM_TIB
    beq .word_end
    jsr .is_delim
    beq -
    jmp .append

-   jsr GET_CHAR_FROM_TIB
    beq .word_end
    jsr .is_delim
    beq .word_end

.append
    jsr pushya

    jsr HERE
    jsr FETCHBYTE
    jsr ONEPLUS
    jsr HERE
    jsr STOREBYTE

    jsr HERE
    jsr HERE
    jsr FETCHBYTE
    jsr PLUS
    jsr STOREBYTE
    jmp -

.word_end
    inx
    jmp HERE

.is_delim
    ; a == delim?
    cmp LSB,x
    beq + ; yes

    ; delim == space?
    ldy LSB,x
    cpy #K_SPACE
    bne + ; no

    ; compare with nonbreaking space, too
    cmp #K_SPACE | $80
+   rts

    +BACKLINK
    !byte	4
    !text	"find"
FIND
    jsr DUP
    jsr TO_R
    jsr COUNT
    jsr FIND_NAME
    lda LSB,x
    beq +
    jsr R_TO
    inx
    rts
+   inx
    inx
    inx
    jsr R_TO
    jmp ZERO

    +BACKLINK
    !byte	9
    !text	"find-name"
FIND_NAME ; ( caddr u -- caddr u 0 | xt 1 | xt -1 )
    txa
    pha

    lda LSB,x
    beq .find_failed
    sta	.findlen + 1
    sta	.findlen2 + 1

    lda	MSB+1,x
    sta	W2+1
    lda	LSB+1,x
    sta	W2

    lda W2
    bne +
    dec W2+1
+   dec W2
    lda W2
    bne +
    dec W2+1
+   dec W2

    ldx	_LATEST
    lda	_LATEST + 1
.examine_word
    sta	W + 1
    stx	W
    ; W now contains new dictionary word.

    ldy	#2
    lda	(W), y ; get string length of dictionary word
    and	#STRLEN_MASK | F_HIDDEN ; include hidden flag... so we don't find the hidden words.
.findlen
    cmp	#0
    beq	.string_compare

.word_not_equal
    ; no match, advance the linked list.
    ldy	#0
    lax	(W), y
    iny
    lda	(W), y
    ; Is word null? If not, examine it.
    bne .examine_word

    ; It is null - give up.
.find_failed
    pla
    tax
    jmp ZERO

.string_compare ; y = 2
    ; equal strlen, now compare strings...
.findlen2
    lda #0
    sta .strlen
-   lda	(W2), y ; find string
    jsr CHAR_TO_LOWERCASE
    iny
    cmp	(W), y ; dictionary string
    bne	.word_not_equal
    dec	.strlen
    beq	.word_is_equal
    jmp	-

.strlen !byte 0

.word_is_equal
    ; return address to dictionary word
    pla
    tax
    inx
    lda	W
    sta	LSB, x
    lda	W + 1
    sta	MSB, x

    jsr TCFA

    dex

    ldy	#2
    lda (W), y
    and #F_NO_TAIL_CALL_ELIMINATION | F_IMMEDIATE
    sta FOUND_WORD_WITH_NO_TCE

    lda	(W), y ; a contains string length + mask
    and	#F_IMMEDIATE
    beq .not_immed
    dey
    sty LSB, x ; 1
    dey
    sty MSB, x ; 0
    rts

.not_immed
    lda #$ff
    sta LSB, x
    sta MSB, x
    rts

; >CFA
    +BACKLINK
    !byte	4
    !text	">cfa"
TCFA
    lda	MSB, x
    sta	W + 1
    lda	LSB, x
    sta W
    ; W contains pointer to word
    ldy	#2
    lda	(W), y ; a contains string length + mask
    and	#STRLEN_MASK
    clc
    adc	#3 ; offset for link + string length
    adc	LSB, x
    sta	LSB, x
    bcc	+
    inc	MSB, x
+   rts

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

IS_SPACE ; ( c -- f )
    ldy #1
    lda LSB,x
    cmp #' ' | 0x80
    beq .is_space
    lda #' '
    cmp LSB,x
    bcs .is_space
    dey
.is_space:
    sty LSB,x
    sty MSB,x
    rts

IS_NOT_SPACE ; ( c -- f )
    jsr IS_SPACE
    jmp ZEQU

XT_SKIP ; ( addr n xt -- addr n )
    ; skip all chars satisfying xt
    jsr TO_R
-   jsr DUP
    jsr ZBRANCH
    !word .done
    jsr OVER
    jsr FETCHBYTE
    jsr R_FETCH
    jsr EXECUTE
    jsr ZBRANCH
    !word .done
    jsr ONE
    jsr SLASH_STRING
    jmp -
.done
    jsr R_TO
    inx
    rts

    +BACKLINK
    !byte 1
    !text	"1"
ONE
    +VALUE 1

    +BACKLINK
    !byte 10
    !text	"parse-name"
PARSE_NAME ; ( name -- addr u )
    jsr SOURCE
    jsr TO_IN
    jsr FETCH
    jsr SLASH_STRING
    jsr LIT
    !word IS_SPACE
    jsr XT_SKIP
    jsr OVER
    jsr TO_R
    jsr LIT
    !word IS_NOT_SPACE
    jsr XT_SKIP
    jsr TWODUP
    jsr ONE
    jsr MIN
    jsr PLUS
    jsr SOURCE
    inx
    jsr MINUS
    jsr TO_IN
    jsr STORE
    inx
    jsr R_TO
    jsr TUCK
    jmp MINUS

    +BACKLINK
    !byte	9
    !text	"interpret"
INTERPRET
    jsr PARSE_NAME

    lda LSB,x
    bne +
    inx
    inx
    rts
+
    jsr	FIND_NAME ; replace string with dictionary ptr
    lda LSB, x
    bne	.found_word

    inx ; drop
    jsr READ_NUMBER
    beq .was_number

    jmp print_word_not_found_error

    ; yep, it's a number...
.was_number
    lda	STATE ; are we compiling?
    bne	+
    rts
+   ; yes, compile the number
    sta curr_word_no_tail_call_elimination
    jmp LITERAL

.found_word
    ; OK, we found a word...

    lda curr_word_no_tail_call_elimination
    sta last_word_no_tail_call_elimination
FOUND_WORD_WITH_NO_TCE = * + 1
    lda #0
    sta curr_word_no_tail_call_elimination

    ; Executes the word if it is immediate, or interpreting.
    inx
    lda MSB-1, x
    and STATE
    beq	EXECUTE

    ; OK, this word should be compiled...
    jmp COMPILE_COMMA

print_word_not_found_error ; ( caddr u -- )
    lda	#$12 ; reverse on
    jsr	PUTCHR
    jsr TYPE
    lda	#'?'
.stop_error_print
    jsr	PUTCHR

    lda	#$d ; cr
    jsr	PUTCHR
    jmp ABORT

    +BACKLINK
    !byte	5
    !text	"abort"
ABORT
    ldx #X_INIT ; reset stack
    jmp QUIT

    +BACKLINK
    !byte	1
    !text	"'"
    jsr BL
    jsr WORD
    jsr FIND
    lda LSB,x
    bne +
    jsr COUNT
    jmp print_word_not_found_error
+   inx
    rts

    +BACKLINK
    !byte	7
    !text	"execute"
EXECUTE
    lda	LSB, x
    sta W
    lda	MSB, x
    sta	W + 1
    inx
    jmp	(W)

; --- QUIT

quit_reset
    sei
    lda	#<restore_handler
    sta $318
    lda	#>restore_handler
    sta $319
    cli

    ; lores
    lda #$9b
    sta $d011
    lda #$17
    sta $dd00
    sta $d018

    txa
    pha

    ldx #0
    stx	$d020
    stx	$d021

    lda #>TIB
    sta TIB_PTR + 1

    lda #$56 ; ram + i/o + kernal
    sta 1

    ; Yellow text.
    lda	#7
    sta	$286

    ; Clears color area.
-   sta $d800, x
    sta $d900, x
    sta $da00, x
    sta $db00, x
    dex
    bne	-

    stx     STATE
    stx     TIB_SIZE
    stx     TIB_SIZE + 1
    stx     TIB_PTR
    stx     TO_IN_W
    stx     TO_IN_W + 1
    stx     SOURCE_ID_LSB
    stx     SOURCE_ID_MSB
    stx     SAVE_INPUT_STACK_DEPTH
    stx     READ_EOF
    jsr     CHKIN
    pla
    tax
    rts

    +BACKLINK
    !byte	4
    !text	"quit"
QUIT
    jsr quit_reset

    ; resets the return stack
    txa
INIT_S = * + 1
    ldx #0
    txs
    tax

interpret_loop
    jsr REFILL

    jsr interpret_tib
    jmp interpret_loop

    +BACKLINK
    !byte 3
    !text "bye"
    ldx INIT_S
    txs
    pla
    sta $319
    pla
    sta $318
    pla
    sta 1
    rts

interpret_tib
    jsr	INTERPRET
    cpx #X_INIT+1
    bpl .on_stack_underflow
    lda TO_IN_W
    cmp TIB_SIZE
    bne interpret_tib
    lda TO_IN_W + 1
    cmp TIB_SIZE + 1
    bne interpret_tib

    lda SOURCE_ID_LSB
    beq +
    rts
+   lda	#'o'
    jsr	PUTCHR
    lda	#'k'
    jsr	PUTCHR
    lda	#$d
    jmp	PUTCHR

.on_stack_underflow
    lda	#$12 ; reverse on
    jsr	PUTCHR
    lda #'e'
    jsr	PUTCHR
    lda #'r'
    jsr	PUTCHR
    jmp .stop_error_print

    +BACKLINK
    !byte   2
    !text	"2*"
    asl LSB, x
    rol MSB, x
    rts

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

    +BACKLINK
    !byte 6
    !text   "within"
WITHIN ; ( test low high -- flag )
    jsr OVER
    jsr MINUS
    jsr TO_R
    jsr MINUS
    jsr R_TO
    jmp U_LESS

    +BACKLINK
    !byte   4
    !text   "pick" ; ( x_u ... x_1 x_0 u -- x_u ... x_1 x_0 x_u )
    stx tmp_x
    txa
    clc
    adc LSB,x
    tax
    inx
    lda LSB,x
    ldy MSB,x
    ldx tmp_x
    sta LSB,x
    sty MSB,x
    rts

    +BACKLINK
    !byte 5
    !text	"depth"
    txa
    eor #$ff
    tay
    iny
    dex
    sty LSB,x
    lda #0
    sta MSB,x
    rts

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
