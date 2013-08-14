

# asm local labels

18 allot value refs # 8 refs
f allot value locs # 5 locs
var locp var refp
: :asm :asm
locs locp ! refs refp !
0 refs 18 fill 0 locs f fill ;
: @@
refp @ [ refs 18 + ] literal < assert
here refp @ !
2 refp +! refp @ c! 1 refp +! 0 ;
: @: 
locp @ [ locs f + ] literal < assert
here locp @ !
2 locp +! locp @ c! 1 locp +! ;
: ;asm ;asm
locs begin dup [ locs f + ] literal < while
refs begin dup [ refs 18 + ] literal < while
dup @ if over @ if
2dup 2+ c@ swap 2+ c@ = if
over @ over @ 2+ - over @ 1+ c!
then then then
3 + repeat drop 3 + repeat drop ;
