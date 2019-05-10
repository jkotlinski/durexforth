require rnd
require sprite

340 sp-data
DDD..UU.U.RRR..EEEEX...X
DDD..UU.U.RRR..EEEEX...X
D.DD.UU.U.R.RR.E....X.X.
D.DD.UU.U.R.RR.E....X.X.
D..D.UU.U.RRR..EEE...X..
D..D.UU.U.RRR..EEE...X..
D..D.UU.U.R.RR.E....X.X.
D..D.UU.U.R.RR.E....X.X.
DDD...UUU.R.RR.EEEEX...X
DDD...UUU.R.RR.EEEEX...X

FFFF..OO..RRR.TTTTTTH..H
FFFF.OOOO.RRR.TTTTTTH..H
FF...O..O.R.RR..TT..H..H
FF...O..O.R.RR..TT..H..H
FFFF.O..O.RRR...TT..HHHH
FFFF.O..O.RRR...TT..HHHH
FF...O..O.R.RR..TT..H..H
FF...O..O.R.RR..TT..H..H
FF....OO..R.RR..TT..H..H
FF....OO..R.RR..TT..H..H

: rnds rnd 100/ 7 and ;
: demo
7 begin
340 40 / over 7f8 + c!
dup sp-on
1 over + over sp-col!
?dup while 1- repeat

begin
rnd rnd rnds sp-xy!
rnds sp-1h rnds sp-2h
rnds sp-1w rnds sp-2w
key? until key drop
8 0 do i sp-off loop ;
demo
