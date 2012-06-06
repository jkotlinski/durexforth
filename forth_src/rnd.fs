

var seed
# Generates random number.
# Poor code from Starting FORTH.
: rnd ( -- u ) seed @
7abd * 1b0f + dup seed ! ;
