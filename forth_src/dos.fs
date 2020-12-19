require io

\ send command string to drive and
\ print response
: send-cmd ( addr len -- )
$f $f open clrchn $f chkin
begin chrin emit readst until
clrchn $f close cr ;

\ send remainder of line as dos command
\ and print response
: dos source >in @ /string 
dup >in +! \ consume buffer
send-cmd ;
