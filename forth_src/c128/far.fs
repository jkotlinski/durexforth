( routines for accessing memory across
banks on the commodore 128 )

code c!far ( value bank addr -- )
\ store addr in w
lsb lda,x w sta,
msb lda,x w 1+ sta, inx,
w lda,# $02b9 sta, \ stavec = w
lsb lda,x w2 1+ sta, inx, \ w2h = bank
lsb lda,x inx, \ a = value
w2 stx, \ w2l = x
w2 1+ ldx, \ x = bank
0 ldy,# \ y = offset = 0
$ff77 jsr, \ indsta
w2 ldx, \ restore x
;code

code c@far ( bank addr -- value )
\ store addr in w
lsb lda,x w sta,
msb lda,x w 1+ sta, inx,
w2 stx, \ w2l = x
lsb lda,x tax, \ x = bank
0 ldy,# \ y = offset = 0
w lda,# \ a = ptr to w
$ff74 jsr, \ indfet
w2 ldx, \ restore x
lsb sta,x 0 lda,# msb sta,x 
;code

code !far ( value bank addr -- )
\ store addr in w
lsb lda,x w sta,
msb lda,x w 1+ sta, inx,
w lda,# $02b9 sta, \ stavec = w
lsb lda,x w2 1+ sta, inx, \ w2h = bank

\ low byte
lsb lda,x \ a = value
w2 stx, \ w2l = x
w2 1+ ldx, \ x = bank
0 ldy,# \ offset = 0
$ff77 jsr, \ INDSTA

\ high byte
w2 ldx, \ restore x
msb lda,x inx, \ a = value
w2 stx, \ w2l = x
w2 1+ ldx, \ x = bank
1 ldy,# \ offset = 1
$ff77 jsr, \ indsta
w2 1+ ldx, \ restore x
;code

code @far ( bank addr -- value )
\ store addr in w
lsb lda,x w sta,
msb lda,x w 1+ sta, inx,
w2 stx, \ w2l = x
lsb lda,x tax, \ x = bank
w2 1+ stx, \ w2h = x
w lda,# \ a = ptr to w

\ low byte
0 ldy,# \ offset = 0
$ff74 jsr, \ INDFET
w2 ldx, lsb sta,x

w2 1+ ldx, \ x = bank
1 ldy,# \ offset = 1
w lda,# \ a = ptr to w
$ff74 jsr, \ INDFET
w2 ldx, msb sta,x
;code
