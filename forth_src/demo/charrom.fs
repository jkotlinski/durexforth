( how to modify char ROM font )
: chardemo
( switch in char ROM )
[ sei, 2 base ! ]
1 c@ 11111000 and 11 or 1 c!

( copy char ROM to $7800 )
[ hex ] d800 7800 800 move

( switch back I/O + kernal )
[ 2 base ! ]
1 c@ 11111000 and 110 or 1 c!
[ cli, ]

( set vic bank to $4000-$7fff )
[ hex ] dd00 c@
[ 2 base ! ] 11111100 and 10 or
[ hex ] dd00 c!

( set vic text screen = $7400,
  vic character data = $7800 )
de d018 c!

( fill text screen )
7800 7400 do i dup c! loop

( invert font forever )
begin
8000 7800 do i c@ invert i c! loop
again ;
