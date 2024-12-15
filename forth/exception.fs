variable handler 0 handler !

code sp@ w stx, tsx, txa, w ldx,
w ldy, ' pushya jmp, end-code

: catch ( xt -- e|0 )
handler @ >r sp@ handler !
execute r> handler ! r> drop 0 ;
