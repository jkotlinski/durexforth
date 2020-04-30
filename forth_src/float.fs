( BASIC floating point words.

NB: If running from cartridge, these
words will crash because BASIC ROM
cannot be accessed!

Example:

s" .5" strf .5
s" .8" strf .8
.5 fac! .8 fac* fac.

...prints .4! )

: bsys \ system call to BASIC ROM
1 c@ dup 3 or 1 c! swap sys 1 c! ;
: fac, $bbca bsys
$57 here 5 move here 5 + to here ;
\ 5-byte float word from string
: strf ( str strlen -- )
ar ! $22 ! $b7b5 bsys create fac, ;
\ 5-byte float word from signed int
: intf ( s -- ) create
dup 100/ ar ! yr ! $b391 bsys fac, ;
: fac! ( faddr -- )
dup 100/ yr ! ar ! $bba2 bsys ;
: fac* ( faddr -- )
dup 100/ yr ! ar ! $ba28 bsys ;
: fac. $bddd bsys $b487 bsys $ab21 bsys ;
© 2020 GitHub, Inc.
