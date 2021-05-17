
org 100h

;a >= 100 

push a          ; SS : a
mov ax,100                                                      
push ax         ; SS : a 100
pop bx
pop ax          ; bx = 100   ax = a
cmp ax,bx       
jge e000
mov cx,0 		; cx guardda el valor de la comparacion
jmp e001
e000: mov cx,1
e001: push cx

b <= 200
push b
mov ax,200
push ax
pop bx
pop ax
cmp ax,bx
jle e002
mov cx,0
jmp e003
e002: mov cx,1
e003: push cx

	a >= 100 && b <= 200
pop bx
pop ax
cmp ax,0
je e004
mov cx,1
jmp e005
e004: mov cx,0
e005: push cx


ret


