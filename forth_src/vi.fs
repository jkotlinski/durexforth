d value CR
: clrscr e544 sys ;

6001 value bufstart

variable eof ( ram eof )
0 eof !

variable homepos ( position at screen home )
variable curlinestart

( cursor screen pos )
variable curx
variable cury
0 value need-refresh
variable line-dirty
0 value insert-active

: line-dirty! 1 line-dirty c! ;

10 allot dup
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
insert-active 0= if
curx @ linelen dup if 1- then min 
curx c!
then
cursor-scr-pos
dup @ 80 or
swap c! ;

: do-load
0 bufstart 400 fill
bufstart loadb

if \ file error?
bufstart 1+ eof !
0 dup dup eof @ c! curx ! cury !
CR bufstart c!
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
bl status-pos 18 fill ;

: set-status ( c -- )
clear-status status-pos c! ;

: rom-kernal 37 1 c! ;
: ram-kernal 35 1 c! ;
: init-key
ram-kernal
eaea c@ a <> if
rom-kernal
\ modifies kernal to change kbd prefs
e000 e000 2000 cmove \ copy rom => ram
\ hopefully basic is not used...
80 28a c! \ key repeat on
a eaea c! \ repeat delay
2 eb1d c! \ repeat speed
then ;

: init
init-key
0 bufstart 1- c! \ sentinel
0 compile-ram ! \ to enable editor start from base.src
clear-status ;

: push-colors
d020 c@
d021 c@
286 c@

2 d021 c!
a d020 c!
1 286 c!
1 d800 400 fill ;

: cleanup ( bordercolor bgcolor cursorcolor -- )
40 28a c! \ key repeat off
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
428 400 398 cmove
line-dirty!
then
fit-curx-in-linelen ;

: cr= CR = ;
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
400 428 398 cmove>
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
20 cury @ 28 * 400 + 28 fill
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

: insert-start
1 to insert-active
[char] i set-status ;

: force-cur-right
linelen if 1 curx +! then ;

: append-start
force-cur-right
insert-start ;

: insert-stop
cur-left
0 to insert-active
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
eof @ editpos - cmove 
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
20 editpos 1- c! \ cr => space
else nipchar then

curlinestart ! curx ! cury ! ;

: backspace-sol
cury @ 0= if exit then
linelen cur-up eol linelen join-lines
( from-linelen to-linelen )
swap if if cur-right nipchar then 
else drop force-cur-right then ;

: backspace
curx @ if cur-left nipchar line-dirty!
else backspace-sol then ;

: del-char force-cur-right backspace ;

: insert-char
	dup CR <> linelen 26 > and if drop exit then

	editpos
	editpos 1+
	eof @ editpos -
	cmove>
	editpos c!
	1 curx +!
	1 eof +!
	0 eof @ c!
    line-dirty!
;

9d value LEFT
11 value DOWN
91 value UP
1d value RIGHT

: insert-right
curx @ linelen 1- = if
force-cur-right else cur-right then ;

: insert-handler
	dup a0 = if drop 20 then \ shift space => space

	dup
	case
    3 of drop endof \ run/stop
	5f of insert-stop drop endof \ leftarrow
	LEFT of cur-left drop endof
	DOWN of cur-down drop endof
	UP of cur-up drop endof
	RIGHT of insert-right drop endof
	14 of backspace drop endof \ inst
	94 of del-char drop endof \ del
	CR of insert-char cur-down sol show-page endof
	insert-char
	endcase
;

: del-word
line-dirty!
begin 
editpos c@ eol= if exit then
editpos c@ del-char space= if exit then
again ;

28 allot value clipboard
variable clipboard-count
0 clipboard-count !

: yank-line
linelen clipboard-count !
curlinestart @ clipboard linelen 
cmove ;

: del-line
sol 1 to need-refresh 
yank-line
( contract buffer )
curlinestart @ find-next-line
curlinestart @
2dup swap - -rot
eof @ curlinestart @ - cmove
eof +! ;

: del
[char] d set-status
key case
[char] w of del-word endof
[char] d of del-line endof
endcase clear-status ;

10 allot value search-buf

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

18 allot value drivebuf

: do-backup
	\ scratch old backup
	drivebuf
	[char] s over c! 1+
	[char] : over c! 1+
	[char] . over c! 1+
	dup
	filename swap filename-len c@ cmove
	filename-len c@ +
	CR swap c!

	drivebuf filename-len c@ 4 +
    f openw f closew

	\ rename to new backup
	drivebuf
	[char] r over c! 1+
	1+ \ colon already in place...
	[char] . over c! 1+
	filename-len c@ + \ filename ok
	[char] = over c! 1+
	dup
	filename swap filename-len c@ cmove
	filename-len c@ + \ filename ok
	CR swap c!

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
	[char] ! emit
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
	[char] w emit
	key
	case
	CR of write-file endof
	[char] ! of save-as endof
	endcase
;

: find-handler
	0 18 setcur
	clear-status
	[char] / emit
	0 ( count )
	begin
		key dup
		CR <> if
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
sol CR insert-char sol
insert-start
1 to need-refresh ;

: paste-line
open-line insert-stop
( make room for clipboard contents )
curlinestart @
dup clipboard-count @ +
eof @ 1+ curlinestart @ - cmove>
( copy from clipboard )
clipboard
curlinestart @
clipboard-count @ cmove
( update eof )
clipboard-count @ eof +! ;

: change-word
del-word
bl insert-char
cur-left
insert-start ;

: force-cur-down
editpos
cur-down
editpos = if
eol
force-cur-right
CR insert-char
cur-down
then ;

header maintable
key i c, ' insert-start ,
key a c, ' append-start ,
key / c, ' find-handler ,
( ctrl+u )
15 c, ' half-page-back ,
( ctrl+d )
4 c, ' half-page-fwd ,
key J c, ' join-lines ,
key g c, ' goto-start ,
key G c, ' goto-eof ,
key $ c, ' eol ,
key 0 c, ' sol ,
key r c, ' replace-char ,
key O c, ' open-line ,
key P c, ' paste-line ,
key x c, ' del-char ,
key X c, ' backspace ,
key b c, ' word-back ,
key w c, ' word-fwd ,
key d c, ' del ,
LEFT c, ' cur-left ,
RIGHT c, ' cur-right ,
UP c, ' cur-up ,
DOWN c, ' cur-down ,
key h c, ' cur-left ,
key l c, ' cur-right ,
key k c, ' cur-up ,
key j c, ' cur-down ,
0 c,

\ custom restore handler
\ "vi"
here key v c, key i c, d c, 0 c,
here cli, \ entry
swap dup \ asm vi vi 
\ compile-ram="vi"
lda,# compile-ram sta,
100/ lda,# compile-ram 1+ sta,
\ lores
9b lda,# d011 sta, 17 lda,# dd00 sta,
17 lda,# d018 sta,
318 @ jmp, \ jump to normal restore
: compile-run sei literal 318 ! cli
bufstart compile-ram ! ;

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

    [char] y of \ yy
     key [char] y = if
     yank-line
    then endof
	[char] o of force-cur-down open-line endof
	[char] p of force-cur-down paste-line endof
	[char] Z of
		key
		case
		[char] Z of write-file ffff exit endof
		endcase
	endof
	[char] : of 
		[char] : set-status
		key 
		case
		[char] w of colon-w endof
		[char] q of ffff exit endof
		endcase
		clear-status
	endof

	[char] c of
		key
		[char] w = if change-word then
	endof

	( cursor )
    \ eof should be 0 terminated!
    eof @ c@ 0= assert
    \ eof @ ae ! 
	88 of compile-run ffff exit endof \ f7

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

		insert-active if
			insert-handler
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
s" untitled" then then

init
go-to-file-start

\ store away filename
2dup ( str len str len )	
filename-len c!
filename f cmove

do-load
push-colors
show-page
main-loop
cleanup ;

loc vi
hide-to CR
hidden
