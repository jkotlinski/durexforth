

( asm local labels.

n @: = label n
n @@ = branch to label n

...where n is in range[0, ff]

relative branches are resolved by ;asm
- this allows for mixed forward and
backward references, but it is not
possible to branch over ;asm

-- example --
:asm checkers
7f lda,# 0 ldy,# 1 @:
400 sta,y 500 sta,y
600 sta,y 700 sta,y
dey, 1 @@ bne, ;asm )

( refs and locs are arrays of
2-byte address + 1-byte index )
18 allot value refs # 8 refs
f allot value locs # 5 locs
var locp var refp

locs locp ! refs refp ! # init

# reference
: @@ ( index -- dummy )
refp @ [ refs 18 + ] literal < assert
here refp @ !
2 refp +! refp @ c! 1 refp +! 0 ;
# label
: @: ( index -- )
locp @ [ locs f + ] literal < assert
here locp @ !
2 locp +! locp @ c! 1 locp +! ;
: ;asm ;asm
locs begin dup locp @ < while
refs begin dup refp @ < while
over 2+ c@ over 2+ c@ = if
over @ over @ 2+ - over @ 1+ c!
then 3 + repeat drop 3 + repeat drop
# reset
locs locp ! refs refp ! ;

hide locs hide refs hide locp hide refp
