variable row 0 row !

: getlines
." Finish with empty line" cr cr
begin here 10 accept cr ?dup while
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
cr ." Press 1-3" cr
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
cr ." Press 1-5" cr
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
cr ." Press 1-4" cr
iron key case
'1' of iron x endof
'2' of iron-lo endof
'3' of iron-mid endof
'4' of iron-hi endof
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
cr ." Press 1-7" cr
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
cr ." Press 1-5" cr
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
cr ." Press 1-3" cr
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
cr ." Press 1-3" cr
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
cr ." Press 1-6" cr
key case
'1' of wash x sel-bleach endof
'2' of handwash sel-bleach endof
'3' of wash t30 sel-agitation endof
'4' of wash t40 sel-agitation endof
'5' of wash t60 sel-agitation endof
'6' of wash t95 sel-agitation endof
endcase ;

\ maxwidth: 3 * 6 = 18 chars

0 value srcy
: print
4 device
0 0 4 openw
row @ 8 * 0 do
8 emit \ bit printing mode
90 0 do
kernal-out
80
i j peek if 1+ then
i j 1+ peek if 2 + then
i j 2 + peek if 4 + then
i j 3 + peek if 8 + then
i j 4 + peek if 10 + then
i j 5 + peek if 20 + then
i j 6 + peek if 40 + then
kernal-in emit loop
cr f emit 7 +loop
4 closew ;

code brk brk, ;code

: wizard
1 clrcol
material
row @ spritey !
vattentvatt 3 row +!
extra
hires 1 d021 c!
9 #24 s" Press any key to print" text
key print brk ;
