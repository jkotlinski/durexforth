

# examples from c64 step by step
# programming, gfx book 3, phil cornes

s" rnd" load
s" sin" load

: lineweb
hires 7 clrcol
5 begin
dup 140 < while
dup 0 plot 96 c8 line
dup c7 plot 96 0 line
a + repeat drop ;

: rndline
hires 10 clrcol
80 begin ?dup while
rnd ab / 20 -
rnd f8 / 20 - line
1- repeat ;

: radiant
hires d0 clrcol
100 begin
?dup while
32 64 plot
dup cos 12c 2* d* drop 32 12c - +
over sin 12c 2* d* drop 64 12c - +
line
1- repeat ;
