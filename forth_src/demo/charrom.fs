

( how to modify char ROM font )
: chardemo
sei

( switch in char ROM )
[ 2 to base ]
1 c@ dup 11111000 and 11 or 1 c!

( copy char ROM to $5800 )
[ hex ] d800 5800 1000 cmove

( switch back I/O + kernal )
[ 2 to base ]
1 c@ dup 11111000 and 110 or 1 c!

cli

( set vic bank to $4000-$7fff )
[ hex ] dd00 c@
[ 2 to base ] 11111100 and 10 or
[ hex ] dd00 c!

( fill text screen )
4800 4400 do i dup c! loop

( invert font forever )
begin
6000 5800 do i c@ invert i c! loop
again ;
