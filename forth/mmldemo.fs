require mml
cr .( Frere Jaques)
: frere-jaques
s" o3l4fgaffgafab->c&c<ab->c&cl8cdc<b-
l4af>l8cdc<b-l4affcf&ffcf&f"
s" r1r1o3l4fgaffgafab->c&c<ab->c&cl8cd
c<b-l4af>l8cdc<b-l4affcf&ffcf&f"
s" " play-mml ;
frere-jaques

\ l" is like s", but supports strings
\ longer than 255 bytes.
: litl ( -- addr len )
r> 1+ dup 2+ swap @ 2dup + 1- >r ;
: l" ( -- addr len )
postpone litl here 0 , 0
begin getc dup '"' <>
while c, 1+ repeat
drop swap ! ; immediate

cr .( Sarias Song)
: sarias-song
l" l16o3f8o4crcrcro3f8o4crcrcro3f8o4crc
rcro3f8o4crcro3cre8o4crcrcro3e8o4
crcrcro3e8o4crcrcro3e8o4crcro3c8f8o4crc
rcro3f8o4crcrcro3f8o4crcrcro3f8o4
crcro3cro3e8o4crcrcro3e8o4crcrcro3e8o4c
rcrcro3e8o4crcrc8o3drardraro2gro3
gro2gro3grcro4cro3cro4cro2aro3aro2aro3a
ro3drardraro2gro3gro2gro3grcro4cr
o3cro4cro2aro3aro2aro3aro3drardraro2gr
o3gro2gro3grcro4cro3cro4cro2aro3ar
o2aro3aro3drararrrdrararrrcrbrbrrrcrbrb
rrrerarrrarerarrrarerg+rg+rg+rg+r
rre&er"
s" l16o5frarb4frarb4frarbr>erd4<b8>cr<b
rgre2&e8drergre2&e4frarb4frarb4fr
arbr>erd4<b8>crer<brg2&g8brgrdre2&e4r1
r1frgra4br>crd4e8frg2&g4r1r1<f8er
a8grb8ar>c8<br>d8cre8drf8er<b>cr<ab1&b2
r4e&er"
s" l16r1r1r1r1r1r1r1r1o4drerf4grarb4>c8
<bre2&e4drerf4grarb4>c8dre2&e4<dr
erf4grarb4>c8<bre2&e4d8crf8erg8fra8grb8
ar>c8<br>d8crefrde1&e2r4"
play-mml ; sarias-song cr
