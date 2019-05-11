d 7f8 c! e 7f9 c! f 7fa c!

: show ( sprite# addr )
over 40 * 340 + 40 move
dup sp-on 1 over sp-col!
dup a0 a0 rot sp-xy!
dup sp-2h sp-2w ;

0 x show
\ 1 x show
