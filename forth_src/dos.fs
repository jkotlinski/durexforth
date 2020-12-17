require io

\ send command string to drive and
\ print response
: send-cmd ( addr len -- )
0 0 15 15 open 
15 chkout type \ send command
clrch 15 chkin
refill source type cr \ print result
clrch refill 15 close ;

\ send remainder of line as dos command
\ and print response
: dos $d word count send-cmd ;
