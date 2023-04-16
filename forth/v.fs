marker ---editor---

header v

latest \ begin hiding words

$d value lf

$a001 value bufstart \ use $a000-$cbff

\ eof points to 0 sentinel
variable eof ( ram eof )
0 eof !

variable homepos \ screen home position
variable curlinestart

( cursor screen pos )
variable curx
variable cury
0 value need-refresh
variable line-dirty
0 value insert \ flag

: line-dirty! 1 line-dirty c! ;

\ counted string
variable filename $f allot
0 filename c!

: editpos curlinestart @ curx @ + ;

create foundeol
clc, tya, w adc, lsb sta,x
2 bcc, msb inc,x ;code

code print-line ( addr -- addr )
lsb ldy,x w sty,
msb ldy,x w 1+ sty,
0 ldy,#
here w lda,(y)
0 cmp,# foundeol -branch beq,
$e716 jsr, iny, \ putchar
$d cmp,# foundeol -branch beq, jmp,

\ nb: may return eof
code find-next-line ( addr -- addr )
lsb ldy,x w sty,
msb ldy,x w 1+ sty,
0 ldy,#
here
w lda,(y)
iny,
0 cmp,#
foundeol -branch beq,
$d cmp,#
foundeol -branch beq,
jmp,
: find-next-line ( addr -- addr )
dup eof @ u< if find-next-line then ;

: next-line-start
curlinestart @ find-next-line ;

: linelen
next-line-start
curlinestart @ -
dup if 1- then ;

: cursor-scr-pos
cury @ $28 *
curx @ linelen min +
$400 + ( addr ) ;

: sol 0 curx ! ;

\ ram + io + kernal rom
code rom-kernal
$36 lda,# 1 sta, ;code
\ ram + io + ram
code ram-kernal
$35 lda,# 1 sta, ;code

: reset-buffer
0 bufstart 1- c!
bufstart 1+ eof !
0 eof @ c! sol 0 cury !
lf bufstart c!
bufstart homepos !
bufstart curlinestart ! ;

$7c0 value status-pos

: show-page
status-pos c@ page status-pos c!
homepos @
$18 0 do print-line loop
drop ;

: clear-status ( -- )
status-pos $18 bl fill ;

: set-status ( c -- )
clear-status status-pos c! ;

: cleanup ( bordercolor bgcolor
cursorcolor -- )
0 $28a c! \ default key repeat
[ lsb lda,x $286 sta, inx,
  lsb lda,x $d020 sta,
  msb lda,x $d021 sta, inx, ]
page ;

: fit-curx-in-linelen
linelen curx @ min curx ! ;

: cur-down
curlinestart @ ( curline )
find-next-line dup ( 2xnextline )
eof @ u< 0= if drop exit then
curlinestart !
cury @ $17 < if 1 cury +! else
homepos @ find-next-line homepos !
$428 $400 $398 move
line-dirty!
then
fit-curx-in-linelen ;

: cr= lf = ;
: eol= dup 0= swap cr= or ;
: space= dup cr= swap bl = or ;
: eof= eof @ = ;

: find-start-of-line ( addr -- addr )
begin 1- dup c@ eol= until 1+ ;

: goto ( line )
sol dup cury !
homepos @ swap
?dup if 0
do find-next-line loop then
curlinestart ! ;

: cur-up
curlinestart @
1-
dup c@ 0= if drop exit then
find-start-of-line
curlinestart !
fit-curx-in-linelen
cury @ 0= if
curlinestart @ homepos !
$400 $428 $398 move
line-dirty!
else
-1 cury +!
then ;

: cur-left
curx @ ?dup if 1- curx ! then ;

: at-eol editpos c@ eol= ;

: cur-right at-eol editpos 1+ c@
eol= or if exit then 1 curx +! ;

: eol
linelen dup if 1- then curx ! ;

\ left, or up+eol if we're at xpos 0
: rewind-cur
curx @ 0= if bufstart editpos <> if
cur-up eol then else cur-left then ;

: is-wordstart
editpos 1- c@ space=
editpos c@ space= 0= and ;

: word-back rewind-cur begin
editpos bufstart = is-wordstart or
0= while rewind-cur repeat ;

\ right, or down+sol if we're at EOL.
\ ret 1 if we cant advance
: advance-cur editpos
curx @ linelen 1- = linelen 0= or if
sol cur-down else cur-right then
editpos = ;

: word-forward
begin editpos eof= editpos c@ space=
or advance-cur or until ;

: word-end
begin advance-cur editpos 1+ dup
eof= swap c@ space= or or until ;

: setcur ( x y -- )
xr ! yr ! $e50c sys ;

: refresh-line
cury @ $28 * $400 + $28 bl fill
0 cury @ setcur
curlinestart @ print-line drop ;

0 value !"mode \ not quote-mode

: ins-start
-1 editpos curlinestart @ ?do
i c@ '"' = xor loop to !"mode
1 to insert 'i' set-status ;

: repl-start
2 to insert 'r' set-status ;

: force-right
linelen if 1 curx +! then ;

: ins-stop cur-left 0 to insert
clear-status ;

: need-refresh! 1 to need-refresh ;

: show-loc ( addr -- )
dup find-start-of-line dup homepos !
dup curlinestart ! - curx ! 0 cury !
need-refresh! clear-status ;

: nipchar
editpos 1+ eof= if exit then
editpos 1+ editpos
eof @ editpos - move
-1 eof +! ;

: join-lines
\ too long to join?
curlinestart @
find-next-line find-next-line
curlinestart @ - $28 >
if exit then
need-refresh!
linelen 0= if nipchar exit then

cury @ curx @ curlinestart @

editpos
cur-down
editpos = if 2drop drop exit then
sol
linelen if
bl editpos 1- c! \ cr => space
else nipchar then

curlinestart ! curx ! cury ! ;

: backspace
curx @ if cur-left nipchar line-dirty!
then ;

: del-char
at-eol if exit then
force-right backspace ;

: repl-char
editpos c! line-dirty! ;

: ins-char
dup lf <> linelen $26 > and if
drop exit then

editpos
editpos 1+
eof @ editpos -
move
editpos c!
1 curx +!
1 eof +!
0 eof @ c!
line-dirty! ;

$9d value left
$11 value down
$91 value up
$1d value right

: ins-right
curx @ linelen 1- = if
force-right else cur-right then ;

: do-insert
[ \ nbsp => space
lsb lda,x $a0 cmp,# +branch bne,
$20 lda,# lsb sta,x
\ shift+return => return
:+ $8d cmp,# +branch bne,
$d lda,# lsb sta,x :+ ]

dup case
'"' of !"mode 0= to !"mode endof
$5f of ins-stop drop exit endof \ <-
$14 of backspace drop exit endof \ inst
$94 of del-char drop exit endof \ del
lf of ins-char cur-down sol show-page
exit endof endcase

\ handles control chars outside quotes
dup $7f and $20 < !"mode and if
dup case
left of cur-left drop exit endof
right of cur-right drop exit endof
up of cur-up drop exit endof
down of cur-down drop exit endof
endcase drop exit then

insert 2 = if at-eol if
ins-start ins-char else repl-char
1 curx +! then
else ins-char then ;

: del-word
line-dirty!
begin at-eol if exit then
editpos c@ del-char space=
until ;

variable clip $26 allot
variable clip-count
0 clip-count !

: yank-line linelen clip-count !
curlinestart @ clip linelen move ;

: del-between ( addr )
2dup swap - -rot ( off a1 a2 )
eof @ over - move eof +!
eof @ editpos = if
0 eof @ ! 1 eof +! then
need-refresh! ;

: del-line
sol yank-line
( contract buffer )
next-line-start curlinestart @
del-between ;

: del-to-eol
next-line-start 1- editpos
del-between ;

create fbuf #39 allot
0 fbuf c!

: match? ( addr -- found? )
fbuf c@ fbuf + 1+ fbuf 1+ do dup c@ i
c@ <> if unloop drop 0 exit
then 1+ loop ;

: do-match ( -- )
eof @ editpos 1+ ?do i match? if
i show-loc unloop exit then loop
editpos bufstart ?do i match? if
i show-loc unloop exit then loop
." not found" ;

: word-len ( -- )
1 begin dup editpos + dup c@ space= 0=
swap eof @ < AND
while 1+ repeat ;

: write-file filename c@ 0= if
." no filename" exit then

rom-kernal
page

\ scratch old file
here
's' over c! 1+
'0' over c! 1+
':' over c! 1+
filename 1+ over filename c@ move
filename c@ + lf swap c!
here filename c@ 4 +
$f $f open ioabort $f close

bufstart eof @
filename count saveb
key to need-refresh ;

: open-line
sol lf ins-char sol
ins-start
need-refresh! ;

: paste-line
open-line ins-stop
( make room for clip contents )
curlinestart @
dup clip-count @ +
eof @ 1+ curlinestart @ - move
( copy from clip )
clip
curlinestart @
clip-count @ move
( update eof )
clip-count @ eof +! ;

: force-down editpos cur-down editpos =
if eol force-right lf ins-char cur-down
then ;

: append force-right ins-start ;

: append-line eol append ;

: delete-enter
'd' set-status
key case
'w' of del-word endof
'd' of del-line endof
endcase clear-status ;

: find-enter
0 $18 setcur clear-status '/' emit
fbuf 1+ #38 accept fbuf c!
do-match ;

: find-under
0 $18 setcur clear-status '/' emit
is-wordstart 0= if word-back then
editpos fbuf 1+ word-len dup fbuf c!
move
fbuf 1+ fbuf c@ type bl emit
do-match ;

: page-up
$c 0 do cur-up refresh-line loop ;

: page-down
$c 0 do cur-down refresh-line loop ;

: go-enter
sol 0 cury !
bufstart dup homepos ! curlinestart !
need-refresh! ;
: go-sof
\ can be much optimized...
bufstart eof= if exit then
eof @ 1- find-start-of-line
dup curlinestart ! homepos !
sol
$17 begin
homepos @ 1- find-start-of-line
homepos !
1- dup 0=
homepos @ bufstart = or
until
$17 swap - dup cury ! 0 swap setcur
need-refresh! ;

: repl-under key repl-char ;

: line-down
next-line-start
eof= if exit then homepos @
find-next-line homepos !
next-line-start curlinestart !
sol need-refresh!  ;

: go-home 0 goto ;

: line-up cury @ next-line-start
go-home cur-up curlinestart ! cury !
need-refresh! ;

: yank key 'y' = if yank-line then ;

: open-line-down force-down
open-line ;

: paste-line-down force-down
paste-line ;

: change-word key 'w' = if
del-word bl ins-char cur-left
ins-start then ;

: substitute-char del-char ins-start ;

: substitute-line del-line open-line
ins-start ;

: go-last $17 goto ;

: go-mid $c goto ;

: change-line del-to-eol ins-start ;

: findchar ( dir )
curx @ swap key begin
over editpos + c@ eol= invert while
over curx +! dup editpos c@ = if
2drop drop exit then repeat
2drop curx ! ;

: findchar-fwd 1 findchar ;

: findchar-back -1 findchar ;

\ key handler table
\ semi-ordered by most-used
header keytab
left c, ' cur-left ,
right c, ' cur-right ,
up c, ' cur-up ,
down c, ' cur-down ,
'h' c, ' cur-left ,
'l' c, ' cur-right ,
'k' c, ' cur-up ,
'j' c, ' cur-down ,
'$' c, ' eol ,
'0' c, ' sol ,
$13 c, ' sol ,
'i' c, ' ins-start ,
'R' c, ' repl-start ,
'a' c, ' append ,
'A' c, ' append-line ,
's' c, ' substitute-char ,
'S' c, ' substitute-line ,
'J' c, ' join-lines ,
'O' c, ' open-line ,
'P' c, ' paste-line ,
'X' c, ' backspace ,
'x' c, ' del-char ,
'D' c, ' del-to-eol ,
'C' c, ' change-line ,
'b' c, ' word-back ,
'e' c, ' word-end ,
'd' c, ' delete-enter ,
'g' c, ' go-enter ,
'r' c, ' repl-under ,
'w' c, ' word-forward ,
'y' c, ' yank ,
'o' c, ' open-line-down ,
'p' c, ' paste-line-down ,
'c' c, ' change-word ,
'n' c, ' do-match ,
'+' c, ' line-down ,
'-' c, ' line-up ,
$15 c, ' page-up , \ ctrl+u
4 c,   ' page-down , \ ctrl+d
$17 c, ' del-word , \ ctrl+w
'*' c, ' find-under ,
'/' c, ' find-enter ,
'G' c, ' go-sof ,
'H' c, ' go-home ,
'L' c, ' go-last ,
'M' c, ' go-mid ,
'f' c, ' findchar-fwd ,
'F' c, ' findchar-back ,
0 c,
\ --- key handlers end

: do-main ( key -- quit? )
dup ['] keytab begin 2dup c@ = if
1+ nip @ execute drop 0 exit then
3 + dup c@ 0= until 2drop

case \ keys that can quit
  'Z' of key case
    'Z' of write-file -1 exit endof
  endcase endof
  ':' of
    ':' set-status
    key case
    'w' of
      1 $18 setcur 'w' emit key case
      lf of write-file endof
      '!' of
        need-refresh!
        '!' emit here $f accept
        ?dup 0= if exit then
        filename c! here
        filename count move
        write-file
      endof endcase
    endof
    'q' of -1 exit endof
    clear-status
    endcase
  endof
endcase 0 ;

: evaluate-buffer
bufstart dup begin 1+ dup c@ case
lf of swap 2dup - evaluate dup endof
 0 of 2drop exit endof endcase again ;

: main-loop
\ init colors -- border bgcol curscol
[ dex, $d020 lda, lsb sta,x
       $d021 lda, msb sta,x
  dex, $286  lda, lsb sta,x

  2  lda,#  $d021 sta,
  $a lda,#  $d020 sta,
  1  lda,#  $286  sta, ]
$d800 $400 1 fill

show-page

begin ram-kernal
0 to need-refresh
0 line-dirty c!

depth \ stack check[

\ show cursor
insert 0= if curx @
linelen dup if 1- then min
curx c! then cursor-scr-pos
dup @ $80 xor swap c!

key

\ hide cursor
cursor-scr-pos dup @ $80 xor
swap c!

\ f7
dup $88 = if 2drop cleanup rom-kernal
evaluate-buffer quit then

insert if do-insert else do-main if
drop rom-kernal cleanup exit then then

need-refresh if show-page else
line-dirty c@ if refresh-line then then

depth 1- <> abort" stk" \ stack check]
bufstart 1- c@ abort" sof"
eof @ c@ abort" eof"
curlinestart @ bufstart eof @ within
0= abort" cl" again ;

define v
$ba c@ 8 < abort" bad device#"

\ modifies kernal to change kbd prefs
ram-kernal $eaea @ $8ca <> if
rom-kernal
$e000 dup $2000 move \ rom => ram
$f $eaea c! \ repeat delay
4 $eb1d c! \ repeat speed
then

0 to insert
$80 $28a c! \ key repeat on
clear-status

lf word count dup 0= if \ no param?
eof @ if \ something in buffer?
2drop main-loop exit \ yes - cont. edit
then then

filename c! filename 1+ $f move

reset-buffer
filename c@ if \ load file
rom-kernal

\ Abort if the file is too big to load.
'$' here c! ':' here 1+ c!
filename 1+ here 2+ $f move
here filename c@ 2+ here loadb drop
here $22 + @ $2020 = \ found?
here $20 + @ #44 > and \ 44=$2c00/254
abort" too big"

filename count bufstart loadb
?dup 0= if reset-buffer else
eof ! 0 eof @ c! then then main-loop ;

to latest \ end hiding words
