: initgfx
10 clrcol ;

variable row 0 row !

: getlines
." Finish with empty line" cr cr
begin here 20 accept cr ?dup while
0 swap row @ swap here swap text
1 row +! repeat ;

: material page
." Material Declaration" cr cr
." Enter one material per line" cr
." (e.g. '100% Cotton')" cr
getlines ;

: extra page
." Additional Instructions" cr
getlines ;

: sel-gentle page
." Dry Clean Gentleness" cr cr
." 1: Very Gentle" cr
." 2: Gentle" cr
." 3: Normal" cr
key case
'1' of very-gentle endof
'2' of gentle endof
'3' of endof
endcase ;

: sel-chem page new-symbol
." Professional Cleaning" cr cr
." 1: Do Not Dry Clean" cr
." 2: Water Clean" cr
." 3: PCE Only" cr
." 4: Hydrocarbon Only" cr
." 5: Any Solvent" cr
key case
'1' of circle x endof
'2' of chem-w sel-gentle endof
'3' of chem-p sel-gentle endof
'4' of chem-f sel-gentle endof
'5' of chem-a sel-gentle endof
endcase ;

: sel-iron page new-symbol
." Ironing" cr cr
." 1: Do Not Iron" cr
." 2: Low Temperature" cr
." 3: Medium Temperature" cr
." 4: High Temperature" cr
iron key case
'1' of x endof
'2' of dot1 endof
'3' of dot2 endof
'4' of dot3 endof
endcase sel-chem ;

: sel-natural page
." Natural Drying" cr cr
." 1: Line Dry" cr
." 2: Drip Dry" cr
." 3: Dry Flat" cr
." 4: Line Dry in Shade" cr
." 5: Drip Dry in Shade" cr
." 6: Dry Flat in Shade" cr
." 7: Any of the Above" cr
key case
'1' of new-symbol dry-line endof
'2' of new-symbol dry-drip endof
'3' of new-symbol dry-flat endof
'4' of new-symbol dry-line shade endof
'5' of new-symbol dry-drip shade endof
'6' of new-symbol dry-flat shade endof
'7' of endof
endcase sel-iron ;

: sel-tumble page new-symbol
." Tumble Drying" cr cr
." 1: Do Not Tumble Dry" cr
." 2: No Heat" cr
." 3: Low Temperature" cr
." 4: Medium Temperature" cr
." 5: High Temperature" cr
box circle key case
'1' of x sel-natural endof
'2' of circle-full sel-iron endof
'3' of dot1 sel-iron endof
'4' of dot2 sel-iron endof
'5' of dot3 sel-iron endof
endcase ;

: sel-bleach page new-symbol
." Bleaching" cr cr
." 1: Do Not Bleach" cr
." 2: Non-Chlorine Bleach" cr
." 3: Any Bleach" cr
key case
'1' of bleach x endof
'2' of bleach-ncl endof
'3' of bleach endof
endcase sel-tumble ;

: sel-agitation page
." Wash Agitation" cr cr
." 1: Low (Silk/Wool)" cr
." 2: Medium (Synthetics)" cr
." 3: Max (Cotton)" cr
key case
'1' of very-gentle endof
'2' of gentle endof
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
'1' of wash x sel-bleach endof
'2' of handwash sel-bleach endof
'3' of wash t30 sel-agitation endof
'4' of wash t40 sel-agitation endof
'5' of wash t60 sel-agitation endof
'6' of wash t95 sel-agitation endof
endcase ;

: wizard
initgfx 10 clrcol
material
row @ spritey !
vattentvatt 3 row +!
extra hires key ;

wizard
