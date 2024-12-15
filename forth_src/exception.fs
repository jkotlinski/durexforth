variable handler 0 handler !

code sp@ txa, tay, tsx, txa,
['] pushya jsr, tya, tax, rts, end-code

: catch ( xt -- e|0 )
handler @ >r sp@ handler !
execute r> handler ! r> drop 0 ;
