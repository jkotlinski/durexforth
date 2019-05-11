: initgfx
10 clrcol ;

variable row
0 row !

: material
." Material Declaration" cr cr
." Enter one material per line" cr
." (e.g. '100% Cotton')" cr
." Finish with empty line" cr cr
begin here 28 accept
?dup 0= if exit then
0 swap row @ swap here swap text
1 row +! again ;

: vattentvatt
." Wash Mode?" cr cr
." 1: Do Not Wash" cr
." 2: Hand Wash" cr
." 3: Machine Wash 30C" cr
." 4: Machine Wash 40C" cr
." 5: Machine Wash 60C" cr
." 6: Machine Wash 95C" cr
key case
1 of endof
2 of endof
3 of endof
4 of endof
5 of endof
6 of endof
endcase ;

: wizard
initgfx
material
\ vattentvatt
hires ;

wizard
