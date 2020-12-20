marker ---turnkey---

$9fff value top
$9fff value oldtop
start @ value oldstart

: top! ( addr -- )
latest swap top latest - 
2dup - to latest over to top
swap over - swap 1+ move ;

: restore-forth
oldtop top! 
oldstart start !
---turnkey--- ;

: newstart
restore-forth
start @ execute ;

: save-pack ( strptr strlen -- )
start @ to oldstart
top to oldtop 
['] newstart start ! 
here $20 + top latest - + top!
basic-start top 1+ $d word count saveb
restore-forth ;

: save-prg ( strptr strlen -- )
top to latest ['] 0 1+ top! \ constant 0
save-pack ;

hide oldtop
hide oldstart
hide restore-forth
hide newstart
