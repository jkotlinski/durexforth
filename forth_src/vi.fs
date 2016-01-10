d value lf
: clrscr e544 sys ;

7001 value bufstart

variable eof ( ram eof )
0 eof !

variable homepos ( position at screen home )
variable curlinestart

( cursor screen pos )
variable curx
variable cury
0 value need-refresh
variable line-dirty
0 value ins-active

: line-dirty! 1 line-dirty c! ;

here dup 10 allot
value filename-len
1+ value filename

: editpos
curlinestart @ curx @ + ;

create foundeol
clc,
tya, zptmp adc, sp0 sta,x
2 bcc,
sp1 inc,x
;code

code print-line ( addr -- addr )
sp0 ldy,x zptmp sty,
sp1 ldy,x zptmp 1+ sty,
0 ldy,#

here
zptmp lda,(y)
0 cmp,#
foundeol -branch beq,
e716 jsr, \ putchar
iny,
d cmp,#
foundeol -branch beq,
jmp,

code find-next-line ( addr -- addr )
sp0 ldy,x zptmp sty,
sp1 ldy,x zptmp 1+ sty,
0 ldy,#
here
zptmp lda,(y)
iny,
0 cmp,#
foundeol -branch beq,
d cmp,#
foundeol -branch beq,
jmp,
: find-next-line ( addr -- addr )
dup eof @ < if find-next-line then ;

: linelen
curlinestart @ find-next-line 
curlinestart @ -
dup if 1- then ;

: cursor-scr-pos
cury @ 28 *
curx @ linelen min +
400 + ( addr ) ;

: hide-cursor
cursor-scr-pos
dup @ 7f and
swap c! ;

: show-cursor
ins-active 0= if
curx @ linelen dup if 1- then min 
curx c!
then
cursor-scr-pos
dup @ 80 or
swap c! ;

: rom-kernal 37 1 c! ;
: ram-kernal 35 1 c! ;
: init-kernal
ram-kernal
eaea c@ a <> if
rom-kernal
\ modifies kernal to change kbd prefs
e000 dup 2000 move \ rom => ram
\ hopefully basic is not used...
a eaea c! \ repeat delay
2 eb1d c! \ repeat speed
then ;

: do-load
rom-kernal
bufstart 400 0 fill
bufstart loadb

if \ file error?
bufstart 1+ eof !
0 dup dup eof @ c! curx ! cury !
lf bufstart c!
exit then

ae @ eof !
0 eof @ c! ;

: go-to-file-start
0 dup curx ! cury !
bufstart homepos !
bufstart curlinestart ! ;

7c0 value status-pos

: show-page
status-pos c@ clrscr status-pos c!
homepos @
18 0 do print-line loop
drop ;

: clear-status ( -- )
status-pos 18 bl fill ;

: set-status ( c -- )
clear-status status-pos c! ;

: init
init-kernal
80 28a c! \ key repeat on
0 bufstart 1- c! \ sentinel
clear-status ;

: push-colors
d020 c@
d021 c@
286 c@

2 d021 c!
a d020 c!
1 286 c!
d800 400 1 fill ;

: cleanup ( bordercolor bgcolor cursorcolor -- )
0 28a c! \ default key repeat
286 c! \ cursor col
d021 c! d020 c!
clrscr ;

: fit-curx-in-linelen
linelen curx @ min curx ! ;

: cur-down
curlinestart @ ( curline )
find-next-line dup ( 2xnextline )
eof @ < 0= if drop exit then
curlinestart !
cury @ 17 < if 1 cury +! else
homepos @ find-next-line homepos !
428 400 398 move
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
400 428 398 move
line-dirty!
else
ffff cury +!
then ;

: cur-left
curx @ ?dup if 1- curx ! then ;

: cur-right
editpos c@ eol=
editpos 1+ c@ eol= or if exit then
1 curx +! ;

: eol
linelen dup if 1- then curx ! ;

( left, or up + eol if we're at xpos 0 )
: rewind-cur
curx @ 0= if bufstart editpos <> if
cur-up eol then else cur-left then ;

: sol 0 curx ! ;

: is-wordstart
editpos 1- c@ space=
editpos c@ space= 0= and ;

: word-back
	rewind-cur
	begin
		editpos bufstart =
        is-wordstart or 0=
	while
		rewind-cur
	repeat
;

\ right, or down + sol if we're at EOL. ret 1 if we cant advance
: advance-cur
	editpos
	curx @ linelen 1- = linelen 0= or if
		0 curx ! cur-down
	else
		cur-right
	then
	editpos =
;

: word-fwd
	advance-cur if exit then
	begin
        is-wordstart 0=
	while
		advance-cur if exit then
	repeat
;

: setcur ( x y -- )
xr ! yr ! e50c sys ;

: refresh-line
cury @ 28 * 400 + 28 bl fill
0 cury @ setcur
curlinestart @ print-line drop ;

: half-page-back
c 0 do cur-up refresh-line loop ;

: half-page-fwd
c 0 do cur-down refresh-line loop ;

: goto-eof ( can be much optimized... )
bufstart eof @ = if exit then
eof @ 1- find-start-of-line
dup curlinestart ! homepos !
0 curx !
17 begin
homepos @ 1- find-start-of-line homepos !
1- dup 0=
homepos @ bufstart = or
until
17 swap - dup cury ! 0 swap setcur
1 to need-refresh ;

: goto-start
0 dup curx ! cury !
bufstart dup homepos ! curlinestart !
1 to need-refresh ;

: ins-start
1 to ins-active
'i' set-status ;

: force-cur-right
linelen if 1 curx +! then ;

: append-start
force-cur-right
ins-start ;

: ins-stop
cur-left
0 to ins-active
clear-status ;

: show-location
	dup ( loc loc )
	begin
		dup c@ eol= if
			1+ ( loc sol )
			tuck ( sol loc sol )
			- curx !
			0 cury !
			dup homepos !
			curlinestart !
			1 to need-refresh
			clear-status
			exit
		then
		1-
	again
;

: replace-char
key editpos c! line-dirty! ;

: nipchar
editpos 1+ eof @ = if exit then
editpos 1+ editpos
eof @ editpos - move 
ffff eof +! ;

: too-long-to-join
curlinestart @ find-next-line find-next-line
curlinestart @ - 28 > ;

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

: del-char force-cur-right backspace ;

: ins-char
	dup lf <> linelen 26 > and if drop exit then

	editpos
	editpos 1+
	eof @ editpos -
	move
	editpos c!
	1 curx +!
	1 eof +!
	0 eof @ c!
    line-dirty!
;

9d value left
11 value down
91 value up
1d value right

: ins-right
curx @ linelen 1- = if
force-cur-right else cur-right then ;

: ins-handler
	dup a0 = if drop bl then \ shift space => space

	dup
	case
    3 of drop endof \ run/stop
	5f of ins-stop drop endof \ leftarrow
	left of cur-left drop endof
	down of cur-down drop endof
	up of cur-up drop endof
	right of ins-right drop endof
	14 of backspace drop endof \ inst
	94 of del-char drop endof \ del
	lf of ins-char cur-down sol show-page endof
	ins-char
	endcase
;

: del-word
line-dirty!
begin 
editpos c@ eol= if exit then
editpos c@ del-char space= if exit then
again ;

variable clip 26 allot
variable clip-count
0 clip-count !

: yank-line
linelen clip-count !
curlinestart @ clip linelen 
move ;

: del-line
sol 1 to need-refresh 
yank-line
( contract buffer )
curlinestart @ find-next-line
curlinestart @
2dup swap - -rot
eof @ curlinestart @ - move
eof +! ;

: del
'd' set-status
key case
'w' of del-word endof
'd' of del-line endof
endcase clear-status ;

variable search-buf e allot 

: are-equal ( len a1 a2 -- equal? )
	rot ( a1 a2 len )
	>r ( a1 a2 )
	begin
		r@ ( a1 a2 len )
		0= if ( is len 0? )
			( matches! )
			2drop
            r> drop
			1
			exit
		then
		( a1 a2 )
		dup c@ ( a1 a2 c2 )
		rot ( a2 c2 a1 )
		dup c@ ( a2 c2 a1 c1 )
		rot ( a2 a1 c1 c2 )
		<> ( a2 a1 diff? )
		if
			( not equal!! )
			2drop ( )
            r> drop
			0
			exit
		then
		1+ ( a2 a1 )
		swap 1+ ( a1 a2 )
		r> 1- >r
	again
;

: do-find ( count -- addr )
	editpos ( count a1 )
	1+ ( count a1 )
	begin
		dup eof @ = if
			drop bufstart ( count a1 )
		then
		dup editpos = if
			( not found )
			2drop
			0
			exit
		then

		dup @ search-buf @ = if
			( first char matches... examine )
			2dup ( count a1 count a1 )
			search-buf ( count a1 count a1 search-buf )
			are-equal ( count a1 equal? )
			if
				( count a1 )
				swap drop ( a1 )
				exit
			then
		then
		1+
	again
;

variable drivebuf 16 allot 

: do-backup
	\ scratch old backup
	drivebuf
	's' over c! 1+
	':' over c! 1+
	'.' over c! 1+
	dup
	filename swap filename-len c@ move
	filename-len c@ +
	lf swap c!

	drivebuf filename-len c@ 4 +
    f openw f closew

	\ rename to new backup
	drivebuf
	'r' over c! 1+
	1+ \ colon already in place...
	'.' over c! 1+
	filename-len c@ + \ filename ok
	'=' over c! 1+
	dup
	filename swap filename-len c@ move
	filename-len c@ + \ filename ok
	lf swap c!

	drivebuf filename-len c@ 2 * 5 +
    f openw f closew
;

: write-file
rom-kernal
do-backup

bufstart
eof @
filename filename-len c@
saveb
1 to need-refresh ;

: save-as
	'!' emit
	0 ( len )
	filename ( len filename )
	begin
		key

		dup 5f = if \ leftarrow
			2drop
			drop
			exit
		then
		dup cr= if
			2drop ( len )
			filename-len c!
			write-file
			exit
		then

		dup emit

		( len filename key )
		over c!

		( len filename )
		1+
		swap 1+
		swap
	again
;

: colon-w
	1 18 setcur
	'w' emit
	key
	case
	lf of write-file endof
	'!' of save-as endof
	endcase
;

: find-handler
	0 18 setcur
	clear-status
	'/' emit
	0 ( count )
	begin
		key dup
		lf <> if
			( count key )
			dup emit
			over search-buf + ( count key dst )
			c! ( count )
			1+
			0
		else
			drop
			1
		then
	until
	do-find ( count )
	?dup if
		( found! )
		show-location
	then
;

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

: change-word
del-word
bl ins-char
cur-left
ins-start ;

: force-cur-down
editpos
cur-down
editpos = if
eol
force-cur-right
lf ins-char
cur-down
then ;

header maintable
'i' c, ' ins-start ,
'a' c, ' append-start ,
'/' c, ' find-handler ,
( ctrl+u )
15 c, ' half-page-back ,
( ctrl+d )
4 c, ' half-page-fwd ,
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

variable viloc
\ custom restore handler
here
ff ldx,# txs, inx,
\ lores
9b lda,# d011 sta, 17 lda,# dd00 sta,
17 lda,# d018 sta,
here 1+ viloc !
1234 jsr, ' quit jmp,
: compile-run literal 318 sei ! cli
bufstart eof @ bufstart - 1- evaluate ;

: main-handler ( key -- quit? )
	['] maintable ( key tableptr )

	begin
		( key tableptr )
		2dup ( key tableptr key tableptr )
		c@ = if
			( key tableptr )
			1+ @ 
			execute
			drop 0 exit
		then
		3 +

		dup c@ 0=
	until
	
	drop

	case ( key )

    'y' of \ yy
     key 'y' = if
     yank-line
    then endof
	'o' of force-cur-down open-line endof
	'p' of force-cur-down paste-line endof
	'Z' of
		key
		case
		'Z' of write-file ffff exit endof
		endcase
	endof
	':' of 
		':' set-status
		key 
		case
		'w' of colon-w endof
		'q' of ffff exit endof
		endcase
		clear-status
	endof

	'c' of
		key
		'w' = if change-word then
	endof

	( cursor )
    \ eof should be 0 terminated!
    eof @ c@ 0= assert
    \ eof @ ae ! 
	88 of drop cleanup compile-run quit endof \ f7

	endcase
	0
;

: main-loop
	begin
        ram-kernal
		0 to need-refresh
		0 line-dirty c!

		depth \ stack check...

		show-cursor
        key
		hide-cursor

		ins-active if
			ins-handler
		else
			main-handler if 
				drop
                rom-kernal
				exit
			then
		then

		need-refresh if
			show-page
		else
			line-dirty c@ if
				refresh-line
			then
		then

		depth 1- = assert \ warn if stack changed
	again
;

\ bring back editor
: fg
\ check sentinel
bufstart 1- c@ if ." err" exit then
init
push-colors
show-page
main-loop
cleanup ;

: vi
depth 0= if \ in case no param
eof @ if fg exit else
s" noname" then then

init
go-to-file-start

\ store away filename
2dup ( str len str len )	
filename-len c!
filename f move

do-load
push-colors
show-page
main-loop
cleanup ;
' vi viloc @ !
