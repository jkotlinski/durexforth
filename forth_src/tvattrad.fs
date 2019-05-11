: initgfx
10 clrcol ;

variable mat-text
variable mat-rows
0 to mat-rows

: material
." Material Declaration" cr cr
." Enter one material per line" cr
." (e.g. '100% Cotton')" cr
." Finish with empty line" cr cr
here mat-text !
begin here 10 accept cr
?dup 0= if exit then
1 mat-rows +!
allot d here c! 1 allot again ;

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
;

wizard
