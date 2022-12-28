variable seed
\ Random number generator from
\ Starting Forth. MSB has better
\ randomness than LSB, use 100/
\ when getting bytes.
: rnd ( -- u ) seed @
$7abd * $1b0f + dup seed ! ;
