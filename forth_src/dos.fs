\ File and disk commands, DOS and
\ otherwise, for durexforth
\ (c) 2020 Sergi Reyner
\ MIT licensed

\ NOTE: max command length is 40? chars

\ these words take parameters off the
\ stack, and are meant for programmatic
\ use
marker ---(dos)---

\ send a command to the current device
: (dos.cmd)  ( addr n -- )
  15 openw 15 closew ;

: i0cmd:  ( "name" c -- )
  create c,
  does>  ( drv -- )
    $ba c@ >r swap device
    here swap        c@ over c! 1+
    '0' over c! 1+  ':' swap c!
    here 2 (dos.cmd)
    r> device ;

: i1cmd:  ( "name" c -- )
  create c,
  does>  ( addr n drv -- )
    $ba c@ >r swap device
    here swap        c@ over c! 1+
    '0' over c! 1+  ':' over c! 1+
    2dup + >r       swap move r>
    here tuck - (dos.cmd)
    r> device ;

: i2cmd:
  create c,  ( "name" -- c )
  does>  ( saddr n daddr n drv -- )
    $ba c@ >r swap device
    here swap        c@ over c! 1+
    '0' over c! 1+  ':' over c! 1+
    2dup + >r       swap move r>
    '=' over c! 1+
    2dup + >r       swap move r>
    here tuck - (dos.cmd)
    r> device ;

'i' i0cmd: mount
'v' i0cmd: check-disk

's' i1cmd: delete-file
'n' i1cmd: format-disk

'c' i2cmd: copy-file
'r' i2cmd: rename-file


\ these are parsing words for
\ interactive use
marker ---dos---

\ DOS commands
: 0cmd:  ( "name" xt -- )
  create ,
  does>  ( -- ) $ba c@ swap @ execute ;

: 1cmd:  ( "name" xt -- )
  create ,
  does>  ( "file" -- )
    parse-name rot $ba c@ swap
    @ execute ;

: 2cmd:
  create , ( "name" xt )
  does>  ( "src" "dst" -- )
    @ >r parse-name parse-name
    $ba c@ r> execute ;

' mount 0cmd: fs.mount
' check-disk 0cmd: fs.check-disk

' delete-file 1cmd: fs.delete-file
' format-disk 1cmd: fs.format-disk

' copy-file 2cmd: fs.copy-file
' rename-file 2cmd: fs.rename-file


\ These words use the editor memory
\ past EOF as a buffer. With no file
\ loaded, they can safely operate on up
\ to ~90 blocks before running into the
\ hi-res colours area.

\ This makes them depend on the editor
\ being loaded, though...

: (dosbuf.setup)  ( -- oldeof )
  eof @
  dup 0= if bufstart 1+ eof ! then ;

: (dosbuf.tearup)  ( oldeof bufend -- )
  eof @ swap over - 0 fill
  eof ! ;


\ to be improved later:
\ (in other words, don't rely on these)

\ Copy file from one disk to another
: copy-to-disk  ( "name" src dst -- )
  (dosbuf.setup) -rot
  $ba c@ >r    \ store current device
  swap device
  parse-name 2dup >r >r
  eof @ loadb
  swap device
  eof @ over r> r> saveb
  r> device
  (dosbuf.tearup) ;

\ Join two files into a third one (can
\ be one of them)
: join-files  ( "src1" "src2" "dst" -- )
  (dosbuf.setup)
  parse-name parse-name parse-name
  >r >r >r >r
  eof @ loadb r> r> rot loadb
  eof @ swap dup -rot r> r> saveb
  (dosbuf.tearup) ;


\ to be implemented later:

\ copy-disk
\ rename-disk
