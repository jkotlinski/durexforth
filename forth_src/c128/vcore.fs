code rom-kernal ;code
code ram-kernal ;code
: v-startup 
$d7 c@ if
abort" v only supports 40-column mode.
press esc-x to switch modes."
then ;
