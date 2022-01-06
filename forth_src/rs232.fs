318 @ value old-vector


code nmi
here        pha,
            txa,
            pha,
            tya,
            pha,
$7f         lda,#
$dd0d       sta,
$dd0d       ldy,
+branch     bpl,

$fe72       jmp,

here swap               \ +branch to tos         
:+          pla,
            pla,
            tax,
            sei,
                        \ quit_reset
swap dup 100/
swap $ff and

            lda,#       \ restore
$318        sta,
            lda,#
$319        sta,

dup 100/ swap
$ff and
 
            lda,#       \ brk
$316        sta,
            lda,#
$317        sta,

$10 old-vector +
             jsr        \ quit-reset
' quit 3 +   jmp        \ reset return stack and interpret             
;code

: install ' nmi $318 ! [ $318 (jmp), ] ;
