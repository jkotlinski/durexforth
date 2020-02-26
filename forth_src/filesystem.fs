\ Filesystem commands
marker ---filesystem---

: (sendcmd)  ( addr n -- )
  15 openw 15 closew ;

\ NOTE: max command length is 40 chars
\ DOS commands

: 0cmd:  ( "name" c -- )
  create c,
  does>  ( -- )
    here swap        c@ over c! 1+
    '0' over c! 1+  ':' over c! 1+
    here 2 (sendcmd) ;

: 1cmd:  ( "name" c -- )
  create c,
  does>  ( "file" -- )
    here swap        c@ over c! 1+
    '0' over c! 1+  ':' over c! 1+
    dup parse-name
    rot swap dup >r move r>
    + here - here swap (sendcmd) ;

: 2cmd:
  create c,
  does>  ( "src" "dst" -- )
    here swap        c@ over c! 1+
    '0' over c! 1+  ':' over c! 1+
    dup parse-name >r >r parse-name
    rot swap dup >r move r> +
    '=' over c! 1+
    dup r> r>
    rot swap dup >r move r>
    + here - here swap (sendcmd) ;

'i' 0cmd: fs.init
'v' 0cmd: fs.validate

's' 1cmd: fs.rm
'n' 1cmd: fs.format

'c' 2cmd: fs.copy
'r' 2cmd: fs.rename


\ These words use the editor memory
\ past EOF as a buffer. With no file
\ loaded, they can safely operate on up
\ to ~90 blocks before running into the
\ hi-res colours area.

\ This makes them depend on the editor
\ being loaded, though...

: (buf.setup)  ( -- oldeof )
  eof @
  dup 0= if bufstart 1+ eof ! then ;

: (buf.tearup)  ( oldeof bufend -- )
  eof @ swap over - 0 fill
  eof ! ;

\ Copy file from one disk to another
: fs.d2d  ( "name" src dst -- )
  (buf.setup) -rot
  $ba c@ >r    \ store current device
  swap device
  parse-name 2dup >r >r
  eof @ loadb
  swap device
  eof @ over r> r> saveb
  r> device
  (buf.tearup) ;

\ Join two files into a third one (can
\ be one of them)
: fs.join  ( "src1" "src2" "dst" -- )
  (buf.setup)
  parse-name parse-name parse-name
  >r >r >r >r
  eof @ loadb r> r> rot loadb
  eof @ swap dup -rot r> r> saveb
  (buf.tearup) ;
