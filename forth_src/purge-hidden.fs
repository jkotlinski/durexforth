

( Loading this file permanently
forgets every hidden word by removing
them from the linked word list. After
purge is done, the purge function will
be deleted. )

latest @
here @

var prev-hidden
var last-non-hidden
: purge ( -- )
0 prev-hidden !
latest @ last-non-hidden !
latest @
begin ?dup while
dup ?hidden if 
1 prev-hidden !
else
prev-hidden @ if
dup last-non-hidden @ !
then
dup last-non-hidden !
0 prev-hidden !
then @
repeat ; purge

here !
latest !
