$d value lf

$7001 value bufstart

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

variable filename $f allot \ counted string
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

: linelen
curlinestart @ find-next-line
curlinestart @ -
dup if 1- then ;

: cursor-scr-pos
cury @ $28 *
curx @ linelen min +
$400 + ( addr ) ;

: sol 0 curx ! ;

: rom-kernal $37 1 c! ;
: ram-kernal $35 1 c! ;

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
$286 c! $d021 c! $d020 c! page ;

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

: find-start-of-line ( addr -- addr )
begin 1- dup c@ eol= until 1+ ;

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

: cur-right
editpos c@ eol=
editpos 1+ c@ eol= or if exit then
1 curx +! ;

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

: word-fwd advance-cur if exit then
begin is-wordstart 0= while
advance-cur if exit then repeat ;

: setcur ( x y -- )
xr ! yr ! $e50c sys ;

: refresh-line
cury @ $28 * $400 + $28 bl fill
0 cury @ setcur
curlinestart @ print-line drop ;

: half-page-back
$c 0 do cur-up refresh-line loop ;

: half-page-fwd
$c 0 do cur-down refresh-line loop ;

: goto-eof ( can be much optimized... )
bufstart eof @ = if exit then
eof @ 1- find-start-of-line
dup curlinestart ! homepos !
sol
$17 begin
homepos @ 1- find-start-of-line homepos !
1- dup 0=
homepos @ bufstart = or
until
$17 swap - dup cury ! 0 swap setcur
1 to need-refresh ;

: goto-start sol 0 cury !
bufstart dup homepos ! curlinestart !
1 to need-refresh ;

: ins-start
1 to insert 'i' set-status ;

: force-right
linelen if 1 curx +! then ;

: append-start force-right ins-start ;

: ins-stop cur-left 0 to insert
clear-status ;

: show-loc ( addr -- )
dup find-start-of-line dup homepos ! 
dup curlinestart ! - curx ! 0 cury !
1 to need-refresh clear-status ;

: replace-char
key editpos c! line-dirty! ;

: nipchar
editpos 1+ eof @ = if exit then
editpos 1+ editpos
eof @ editpos - move
-1 eof +! ;

: too-long-to-join curlinestart @ 
find-next-line find-next-line 
curlinestart @ - $28 > ;

: join-lines
too-long-to-join if exit then
1 to need-refresh
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
editpos c@ eol= if exit then
force-right backspace ;

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
dup $a0 = if drop bl then \ nbsp=>space
dup case
3 of drop endof \ run/stop
$5f of ins-stop drop endof \ <-
left of cur-left drop endof
down of cur-down drop endof
up of cur-up drop endof
right of ins-right drop endof
$14 of backspace drop endof \ inst
$94 of del-char drop endof \ del
lf of ins-char cur-down sol show-page
endof ins-char endcase ;

: del-word
line-dirty!
begin editpos c@ eol= if exit then
editpos c@ del-char space= if exit then
again ;

variable clip $26 allot
variable clip-count
0 clip-count !

: yank-line linelen clip-count !
curlinestart @ clip linelen move ;

: del-line
sol 1 to need-refresh
yank-line
( contract buffer )
curlinestart @ find-next-line
curlinestart @
2dup swap - -rot
eof @ curlinestart @ - move
eof +!
eof @ curlinestart @ = if
0 eof @ ! 1 eof +! then
linelen 0= if cur-up join-lines then ;

: del
'd' set-status
key case
'w' of del-word endof
'd' of del-line endof
endcase clear-status ;

: match? ( count addr -- count found? )
over here + here do dup c@ i c@
<> if unloop drop 0 exit then 1+ loop ;

: do-find ( -- )
0 $18 setcur clear-status '/' emit
here #38 accept
eof @ editpos 1+ ?do i match? if
i show-loc unloop drop exit then loop
editpos bufstart ?do i match? if
i show-loc unloop drop exit then loop
drop ." not found" ;

: write-file filename c@ 0= if
." no filename" exit then

rom-kernal
page ." saving "
filename count type ." .."

\ scratch old file
here
's' over c! 1+
'0' over c! 1+
':' over c! 1+
filename 1+ over filename c@ move
filename c@ + lf swap c!
here filename c@ 4 +
f openw f closew

bufstart eof @
filename count saveb
key to need-refresh ;

: :w! 1 to need-refresh
'!' emit filename 1+ f accept
?dup 0= if exit then
filename c! write-file ;

: :w 1 $18 setcur 'w' emit key case
lf of write-file endof
'!' of :w! endof endcase ;

: open-line
sol lf ins-char sol
ins-start
1 to need-refresh ;

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

: change-word del-word bl ins-char
cur-left ins-start ;

: force-down editpos cur-down editpos =
if eol force-right lf ins-char cur-down
then ;

header maintable
'i' c, ' ins-start ,
'a' c, ' append-start ,
'/' c, ' do-find ,
$15 c, ' half-page-back , \ ctrl+u
4 c, ' half-page-fwd , \ ctrl+d
'J' c, ' join-lines ,
'g' c, ' goto-start ,
'G' c, ' goto-eof ,
'$' c, ' eol ,
'0' c, ' sol ,
'r' c, ' replace-char ,
'O' c, ' open-line ,
'P' c, ' paste-line ,
'x' c, ' del-char ,
'X' c, ' backspace ,
'b' c, ' word-back ,
'w' c, ' word-fwd ,
'd' c, ' del ,
left c, ' cur-left ,
right c, ' cur-right ,
up c, ' cur-up ,
down c, ' cur-down ,
'h' c, ' cur-left ,
'l' c, ' cur-right ,
'k' c, ' cur-up ,
'j' c, ' cur-down ,
0 c,

: do-main ( key -- quit? )
['] maintable begin 2dup c@ = if
1+ @ execute drop 0 exit then
3 + dup c@ 0= until drop

case
  'y' of key
    'y' = if yank-line then
  endof
  'o' of force-down open-line endof
  'p' of force-down paste-line endof
  'Z' of key case 
    'Z' of write-file ffff exit endof
  endcase endof
  ':' of
    ':' set-status
    key case
    'w' of :w endof
    'q' of ffff exit endof
    clear-status
    endcase
  endof

  'c' of key
    'w' = if change-word then
  endof
endcase 0 ;

: main-loop
\ init colors -- border bgcol curscol
$d020 c@ $d021 c@ $286 c@
2 $d021 c! $a $d020 c! 1 $286 c!
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
dup @ $80 or swap c!

key

\ hide cursor
cursor-scr-pos dup @ $7f and
swap c!

\ f7
dup $88 = if 2drop cleanup rom-kernal
bufstart eof @ bufstart - 1-
evaluate quit then

insert if do-insert else do-main if
drop rom-kernal cleanup exit then then

need-refresh if show-page else
line-dirty c@ if refresh-line then then

depth 1- <> abort" stk" \ stack check]
bufstart 1- c@ abort" sof"
eof @ c@ abort" eof"
curlinestart @ bufstart eof @ within
0= abort" cl" again ;

: v
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
2drop main-loop exit \ yes - continue edit
then then

2dup filename c! filename 1+ f move

reset-buffer
?dup if \ load file
rom-kernal bufstart loadb
?dup 0= if reset-buffer else
eof ! 0 eof @ c! then
else drop then main-loop ;
