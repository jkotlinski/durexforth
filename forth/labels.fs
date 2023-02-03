\ included into base.fs

( asm local labels.

n @: = label n
n @@ = branch to label n

...where n is in range[0, ff]

relative branches are resolved by ;code
- this allows for mixed forward and
backward references, but it is not
possible to branch over ;code

-- example --
code checkers
7f lda,# 0 ldy,# 1 @:
400 sta,y 500 sta,y
600 sta,y 700 sta,y
dey, 1 @@ bne, ;code )

( refs and locs are arrays of
2-byte address + 1-byte index )
variable refs 8 3 * 2 - allot \ 8 refs
variable locs 5 3 * 2 - allot \ 5 locs
variable locp variable refp

locs locp ! refs refp ! \ init

\ reference
: @@ ( index -- dummy )
here refp @ !
2 refp +! refp @ c! 1 refp +! 0 ;
\ label
: @: ( index -- )
here locp @ !
2 locp +! locp @ c! 1 locp +! ;
: ;code ;code
locs begin dup locp @ < while
refs begin dup refp @ < while
over 2+ c@ over 2+ c@ = if
over @ over @ 2+ - over @ 1+ c!
then 3 + repeat drop 3 + repeat drop
\ reset
locs locp ! refs refp ! ;

hide locs
hide locp
hide refs
hide refp
