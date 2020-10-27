marker ---turnkey---

$9fff value top
$9fff value oldtop
start @ value oldstart

: top! ( addr -- )
latest @ swap top latest @ - 
2dup - latest ! over to top
swap over - swap 1+ move ;

: restore-forth
oldtop top! 
oldstart
---turnkey---
execute ;

: save-pack ( strptr strlen -- )
start @ to oldstart
top to oldtop 
['] restore-forth start ! 
here $20 + top latest @ - + top!
$801 top 1+ $d word count saveb ;

: save-prg ( strptr strlen -- )
here 0 , top latest ! top!
save-pack ;
