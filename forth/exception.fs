variable handler 0 handler !

: catch
[ txa, ' pushya jsr, ] \ sp@
>r handler @ >r
[ w stx, tsx, txa, w ldx,
' pushya jsr, ] \ rp@
handler ! execute
r> handler ! r> drop 0 ;

: throw
?dup if handler @
[ w stx, lsb lda,x tax, txs,
w ldx, inx, ] \ rp!
r> handler ! r> swap >r
[ lsb lda,x tax, ] \ sp!
drop r> then ;
