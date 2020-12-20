$e000 value bmpbase
$cc00 value colbase

code kernal-in
$36 lda,# 1 sta, cli, ;code
code kernal-out
sei, $35 lda,# 1 sta, ;code
code charrom-in
$01 lda, $fb and,# $1 sta, ;code

code hires
$bb lda,# $d011 sta, \ enable bitmap mode
$dd00 lda,
%11111100 and,# \ vic bank 2
$dd00 sta,
$38 lda,# $d018 sta,
;code

code lores
$9b lda,# $d011 sta,
$dd00 lda,
%11 ora,#
$dd00 sta,
$17 lda,#
$d018 sta,
;code
