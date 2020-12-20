\ ram + io + kernal rom
code rom-kernal
$36 lda,# 1 sta, ;code
\ ram + io + ram
code ram-kernal
$35 lda,# 1 sta, ;code

\ modifies kernal to change kbd prefs
: v-startup
ram-kernal $eaea @ $8ca <> if
rom-kernal
$e000 dup $2000 move \ rom => ram
$f $eaea c! \ repeat delay
4 $eb1d c! \ repeat speed
then ;
