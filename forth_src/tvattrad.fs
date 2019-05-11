: initgfx
10 clrcol ;

variable mat-text
variable mat-rows
0 to mat-rows
variable extra-text
variable extra-rows
0 to extra-rows

: material page
." Material Declaration" cr cr
." Enter one material per line" cr
." (e.g. '100% Cotton')" cr
." Finish with empty line" cr cr
here mat-text !
begin here 10 accept cr
?dup 0= if exit then
1 mat-rows +!
allot d here c! 1 allot again ;

: extra page
." Additional Instructions" cr
." Finish with empty line" cr cr
here extra-text !
begin here 10 accept cr
?dup 0= if exit then
1 extra-rows +!
allot d here c! 1 allot again ;

: sel-gentle page
." Dry Clean Gentleness" cr cr
." 1: Very Gentle" cr
." 2: Gentle" cr
." 3: Normal" cr
key case
'1' of endof
'2' of endof
'3' of endof
endcase ;

: sel-chem page
." Professional Cleaning" cr cr
." 1: Do Not Dry Clean" cr
." 2: Water Clean" cr
." 3: PCE Only" cr
." 4: Hydrocarbon Only" cr
." 5: Any Solvent" cr
key case
'1' of endof
'2' of sel-gentle endof
'3' of sel-gentle endof
'4' of sel-gentle endof
'5' of sel-gentle endof
endcase ;

: sel-iron page
." Ironing" cr cr
." 1: Do Not Iron" cr
." 2: Low Temperature" cr
." 3: Medium Temperature" cr
." 4: High Temperature" cr
key case
'1' of endof
'2' of endof
'3' of endof
'4' of endof
endcase sel-chem ;

: sel-natural page
." Natural Drying" cr cr
." 1: Line Dry" cr
." 2: Drip Dry" cr
." 3: Dry Flat" cr
." 4: Line Dry in Shade" cr
." 5: Drip Dry in Shade" cr
." 6: Dry Flat in Shade" cr
key case
'1' of endof
'2' of endof
'3' of endof
'4' of endof
'5' of endof
'6' of endof
endcase sel-iron ;

: sel-tumble page
." Tumble Drying" cr cr
." 1: Do Not Tumble Dry" cr
." 2: Low Temperature" cr
." 3: Medium Temperature" cr
." 4: High Temperature" cr
key case
'1' of sel-natural endof
'2' of sel-iron endof
'3' of sel-iron endof
'4' of sel-iron endof
endcase ;

: sel-bleach page
." Bleaching" cr cr
." 1: Do Not Bleach" cr
." 2: Non-Chlorine Bleach" cr
." 3: Any Bleach" cr
key case
'1' of endof
'2' of endof
'3' of endof
endcase sel-tumble ;

: sel-agitation page
." Wash Agitation" cr cr
." 1: Low (Silk/Wool)" cr
." 2: Medium (Synthetics)" cr
." 3: Max (Cotton)" cr
key case
'1' of endof
'2' of endof
'3' of endof
endcase sel-bleach ;

: vattentvatt page
." Wash Mode" cr cr
." 1: Do Not Wash" cr
." 2: Hand Wash" cr
." 3: Machine Wash 30C" cr
." 4: Machine Wash 40C" cr
." 5: Machine Wash 60C" cr
." 6: Machine Wash 95C" cr
key case
'1' of sel-bleach endof
'2' of sel-bleach endof
'3' of sel-agitation endof
'4' of sel-agitation endof
'5' of sel-agitation endof
'6' of sel-agitation endof
endcase ;

: wizard
initgfx
material
vattentvatt
extra ;

wizard
