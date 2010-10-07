  : CR d ;
: clrscr e544 jsr-wrap ;

: bufstart 5000 ;

var eof ( ram eof )
0 eof !

var homerow
var curlinestart

( cursor screen pos )
var currow
var curx
var cury
0 value need-refresh
0 value need-refresh-line
0 value insert-active

400 dup value maxrows 
cells allot value rowptrs
var rowcount

: homepos
homerow @ cells rowptrs + @ ;

10 allot dup 
value filename-len
1+ value filename

: max ( a b - c )
	2dup ( a b a b )
	< if swap then
	drop
;

: min ( a b - c )
	2dup ( a b a b )
	> if swap then
	drop
;

: editpos curlinestart @ curx @ + ;

: linelen
	0
	curlinestart @ ( count addr )
	begin
		dup c@ d = if
			drop exit
		then

		1+
		swap 1+ swap
	again
;

: cursor-scr-pos
	cury @ 28 *
	curx @ linelen min +
	400 + ( addr )
;

: hide-cursor
	cursor-scr-pos
	dup @ 7f and
	swap c!
;

: show-cursor
	insert-active 0= if
		curx @ linelen 1- min curx c!
	then

	cursor-scr-pos
	dup @ 80 or
	swap c!
;

: do-load
	bufstart loadb

	if # file error?
		bufstart 1+ eof !
		d bufstart c!
		0 eof @ c!
		exit
	then

ae @ eof !
0 eof @ c!

# init rowptrs - should be faster

0 rowptrs maxrows cells fill
0 rowcount !

bufstart 0 # src row
begin
2dup # src row src row
maxrows < # src row src notmax
swap # src row notmax src
c@ # src row notmax char
land # src row break?
while

# add src to rowptrs
dup cells rowptrs + # src row rowptr
rot # row rowptr src
dup # row rowptr src src
-rot # row src rowptr src
swap ! # row src

# advance src past lf
begin
dup c@ d <>
while
1+
repeat
1+

swap 1+ # advance row

repeat

rowcount ! drop ;

: go-to-file-start
0 curx ! 0 cury ! 0 currow !
0 homerow !
bufstart curlinestart ! ;

: status-pos 7c0 ;

:asmsub foundeol
zptmp ldy, 0 sty,x
zptmp 1+ ldy, 1 sty,x
;asm

:asm print-line
0 ldy,x zptmp sty,
1 ldy,x zptmp 1+ sty,
0 ldy,#
here @
zptmp lda,(y)
e716 jsr, # putchar - slow
zptmp inc,
2 bne,
zptmp 1+ inc,
0 cmp,#
foundeol -branch beq,
d cmp,#
foundeol -branch beq,
jmp,

: show-page
status-pos c@
clrscr
status-pos c!
# 0 0 setcur
homepos 18 begin
swap print-line swap
1- ?dup 0= until drop ;

: clear-status ( -- )
	bl status-pos 18 fill
;

: set-status ( c -- )
	clear-status
	status-pos c!
;

: init
	0 compile ! # to enable editor start from base.src
	0 blink
	80 28a c! # key repeat on

	( disable input buffering )
	0 linebuf c! 

	clear-status
;

: push-colors
	d020 c@
	d021 c@
	286 c@

	a d021 c!
	2 d020 c!
	1 286 c!
	1 d800 400 fill
;

: cleanup ( bordercolor bgcolor cursorcolor -- )
	1 linebuf c! # enable buffering

	1 blink
	40 28a c! # key repeat off
	286 c! # cursor col
	d021 c! d020 c!
	93 emit # clrscr
;

: adjust-home
cury @ 8000 and if
1 to need-refresh
cury @ homerow +!
0 cury ! then

cury @ 17 > if
1 to need-refresh
cury @ 17 - homerow +!
17 cury ! then ;

: fit-curx-in-linelen
linelen curx @ min curx ! ;

: is-eof-or-CR
dup 0= swap d = or ;

: cur-right
editpos c@ is-eof-or-CR
editpos 1+ c@ is-eof-or-CR
or if exit then
1 curx +! ;

: curaddr currow @ cells rowptrs + ;

: move-down curaddr 2+ @
if 1 currow +! 1 cury +! then ;

: tidy-up-row
curaddr @ curlinestart !
fit-curx-in-linelen adjust-home ;

: cur-down-n ( rows -- )
begin ?dup while move-down 1- repeat
tidy-up-row ;

: cur-down 1 cur-down-n ;

: cur-up
	curlinestart @ bufstart = if
		exit ( already at top )
	then

	curlinestart @ ( addr )
	1- ( skip first CR plz )
	begin
		1- ( addr )
		dup c@ ( addr char )
		d = ( addr CR? )

		over ( addr CR? addr )
		bufstart < ( addr CR? sof? )
		or ( addr bool )
	until
	( addr )

	1+
	bufstart max
	curlinestart !
	ffff cury +!

	fit-curx-in-linelen

	adjust-home
;

: cur-left
	curx @
	0= if exit then

	ffff curx +!
;

: is-whitespace
	dup d = swap bl = or
;

: eol
	linelen 
	dup 0> if 1- then
	curx !
;

( left, or up + eol if we're at xpos 0 )
: rewind-cur
		curx @ 0= if
			bufstart editpos <> if
				cur-up eol
			then
		else
			cur-left
		then
;

: sol
	0 curx !
;

: word-back
	rewind-cur
	begin
		editpos bufstart =
		editpos 1- c@ is-whitespace
		editpos c@ is-whitespace not land
		or not
	while
		rewind-cur
	repeat
;

# right, or down + sol if we're at EOL. ret 1 if we cant advance
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
		editpos 1- c@ is-whitespace
		editpos c@ is-whitespace not land
		not
	while
		advance-cur if exit then
	repeat
;

: half-page-back
	c begin cur-up 1- dup 0= until drop
;

: half-page-fwd c cur-down-n ;

: find-prev-CR ( addr -- new-addr )
	begin
		1-
		dup bufstart <=
		over c@ d =
		or
	until
	bufstart max
;

: goto-eof ( can be much optimized... )
rowcount @ 1- currow @ - # diff
rowcount @ 1- currow !
cury +!
tidy-up-row ;

: goto-start
0 curx ! 0 cury ! 0 currow ! 0 homerow !
bufstart curlinestart !
1 to need-refresh ;

: insert-start
	1 to insert-active
	[ char i ] literal set-status
;

: force-cur-right
	linelen 0> if
		1 curx +!
	then
;

: append-start
	force-cur-right
	insert-start
;

: insert-stop
	curx @ if
		ffff curx +!
	then
	0 to insert-active
	clear-status
;

: show-location
exit
	dup ( loc sol )
	begin
		dup c@ d = if
			1+ ( loc sol )
			tuck ( sol loc sol )
			- curx !
			0 cury !
			# dup homepos ! broken
			curlinestart !
			1 to need-refresh
			clear-status
			exit
		then
		1-
	again
;

: refresh-line
	curx @
	sol
	0 cury @ setcur
	27 linelen - ( spaces )
	linelen ( spaces chars )
	begin
		?dup 0>
	while
		editpos @ emit
		1 curx +!
		1-
	repeat
	begin
		?dup 0>
	while
		bl emit
		1-
	repeat
	curx !
	curx @ cury @ setcur
;

: replace-char
	key editpos c!
	1 to need-refresh-line
;

: backspace
	curx @ 0= if exit then
	ffff curx +!
	editpos 1+
	editpos
	eof @ editpos - 1+
	ffff eof +!
	cmove
	1 to need-refresh-line
;

: del-char force-cur-right backspace ;

: join-lines
	cury @
	curx @
	curlinestart @

	editpos
	cur-down
	editpos = if 2drop drop exit then
	sol
	editpos ( src )
	editpos 1- ( dst )
	eof @ editpos - 1+

	cmove

	ffff eof +!

	curlinestart !
	curx !
	cury !

	1 to need-refresh
;

: insert-char
	dup CR <> linelen 26 > and if drop exit then

	eof @ editpos - ( u )
	dup 1- editpos + ( u src )
	over 1- editpos + 1+ ( u src dst )
	rot ( src dst u )
	cmove>
	editpos c!
	1 curx +!
	1 eof +!
	0 eof @ c!
	1 to need-refresh-line
;

: LEFT 9d ;
: DOWN 11 ;
: UP 91 ;
: RIGHT 1d ;

: insert-right
curx @ linelen 1- = if
	force-cur-right
else
	cur-right
then
;

: insert-handler
	dup a0 = if drop 20 then # shift space => space

	dup
	case
	5f of insert-stop drop endof # leftarrow
	LEFT of cur-left drop endof
	DOWN of cur-down drop endof
	UP of cur-up drop endof
	RIGHT of insert-right drop endof
	14 of backspace drop endof # inst
	94 of del-char drop endof # del
	CR of insert-char cur-down sol show-page endof
	insert-char
	endcase
;

: push-cursor
	curx @ cury @ curlinestart @
;

: pop-cursor
	curlinestart ! cury ! curx !
;

: del-word
	1 to need-refresh-line
	begin
		editpos c@ 20 = if
			del-char exit
		then
		editpos c@ d = if exit then
		del-char
	again
;

28 allot value clipboard
var clipboard-count
0 clipboard-count !

# this can be much optimized by using cmove
: del-line
	0 clipboard-count !
	sol
	begin
		linelen 0>
	while
        # copy to clipboard
		editpos c@
		clipboard clipboard-count @ +
		c!
		1 clipboard-count +!

		del-char
	repeat
	join-lines
	1 to need-refresh
;

: delete-handler
	[ char d ] literal set-status

	key

	case
	[ char w ] literal of del-word endof
	[ char d ] literal of del-line endof
	endcase

	clear-status
;

10 allot value search-buf

: are-equal ( len a1 a2 -- equal? )
	rot ( a1 a2 len )
	>r ( a1 a2 )
	begin
		r@ ( a1 a2 len )
		0= if ( is len 0? )
			( matches! )
			2drop
			rdrop
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
			rdrop
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
	# scratch old backup
	drivebuf
	dup [ char s ] literal swap c! 1+
	dup [ char : ] literal swap c! 1+
	dup [ char . ] literal swap c! 1+
	dup
	filename swap filename-len c@ cmove
	filename-len c@ +
	CR swap c!

	drivebuf filename-len c@ 4 + openw
	closew

	# rename to new backup
	drivebuf
	dup [ char r ] literal swap c! 1+
	1+ # colon already in place...
	dup [ char . ] literal swap c! 1+
	filename-len c@ + # filename ok
	dup [ char = ] literal swap c! 1+
	dup
	filename swap filename-len c@ cmove
	filename-len c@ + # filename ok
	CR swap c!

	drivebuf filename-len c@ 2 * 5 + openw
	closew
;

: write-file
	do-backup

	bufstart
	eof @
	filename filename-len c@
	saveb
	1 to need-refresh
;

: save-as
	[ char ! ] literal emit
	0 ( len )
	filename ( len filename )
	begin
		key

		dup 5f = if # leftarrow
			2drop
			drop
			exit
		then
		dup CR = if
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
	[ char w ] literal emit
	key
	case
	CR of write-file endof
	[ char ! ] literal of save-as endof
	endcase
;

: find-handler
	0 18 setcur
	clear-status
	[ char / ] literal emit
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
	1 to need-refresh
;

: paste-line
	open-line
	clipboard-count @
	0
	begin
		2dup <> 
	while
		clipboard over + @
		insert-char
		1+
	repeat
	2drop
	insert-stop
	sol
;

: change-word
	del-word 
	bl insert-char
	cur-left
	insert-start 
;

: force-cur-down
	editpos
	cur-down
	editpos = if
		eol 
		force-cur-right
		CR insert-char
		cur-down
	then
;

create maintable
char i c, loc insert-start >cfa ,
char a c, loc append-start >cfa ,
char / c, loc find-handler >cfa ,
char U c, loc half-page-back >cfa ,
char D c, loc half-page-fwd >cfa ,
char J c, loc join-lines >cfa ,
char g c, loc goto-start >cfa ,
char G c, loc goto-eof >cfa ,
char $ c, loc eol >cfa ,
char 0 c, loc sol >cfa ,
char r c, loc replace-char >cfa ,
char O c, loc open-line >cfa ,
char P c, loc paste-line >cfa ,
char x c, loc del-char >cfa ,
char X c, loc backspace >cfa ,
char b c, loc word-back >cfa ,
char w c, loc word-fwd >cfa ,
char d c, loc delete-handler >cfa ,
LEFT c, loc cur-left >cfa ,
RIGHT c, loc cur-right >cfa ,
UP c, loc cur-up >cfa ,
DOWN c, loc cur-down >cfa ,
char h c, loc cur-left >cfa ,
char l c, loc cur-right >cfa ,
char k c, loc cur-up >cfa ,
char j c, loc cur-down >cfa ,
0 c,

: main-handler ( key -- quit? )
	['] maintable ( key tableptr )

	begin
		( key tableptr )
		2dup ( key tableptr key tableptr )
		c@ = if
			( key tableptr )
			1+ @ 
			exec
			drop 0 exit
		then
		3 +

		dup c@ 0=
	until
	
	drop

	case ( key )

	[ char o ] literal of force-cur-down open-line endof
	[ char p ] literal of force-cur-down paste-line endof
	[ char Z ] literal of
		key
		case
		[ char Z ] literal of write-file ffff exit endof
		endcase
	endof
	[ char : ] literal of 
		[ char : ] literal set-status
		key 
		case
		[ char w ] literal of colon-w endof
		[ char q ] literal of ffff exit endof
		endcase
		clear-status
	endof

	[ char c ] literal of
		key
		[ char w ] literal = if change-word then
	endof

	( cursor )
	88 of bufstart compile ! eof @ ae ! ffff exit endof # f7

	endcase
	0
;

: main-loop
	begin
		0 to need-refresh
		0 to need-refresh-line

		sp@ # stack check...

		show-cursor
		key
		hide-cursor

		insert-active if
			insert-handler
		else
			main-handler if 
				drop
				exit
			then
		then

		need-refresh if
			show-page
		else
			need-refresh-line if
				refresh-line
			then
		then

		sp@ 2+ <> if begin 1 d020 +! again then # warn if stack changed
	again
;

: vi
	init
	go-to-file-start

	# store away filename
	2dup ( str len str len )	
	filename-len c!
	filename f cmove

	do-load

	push-colors

	show-page

	main-loop

	cleanup
;

: fg # bring back editor
	eof @ 0= if
		." no buffer"
		cr
		exit
	then
	init
	push-colors
	show-page
	main-loop
	cleanup
;

: edit vi ;

# loc fg loc vi
# hide-to CR
# hidden hidden

