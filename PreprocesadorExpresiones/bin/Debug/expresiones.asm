; --------------------------------------------
; Programa:	expresiones.asm
; Funcion:	Efectuar operaciones de lectura, escritura
; y operaciones aritméticas y lógicas
; --------------------------------------------

text	segment
		assume cs:text, ds:text, sstext
		org 100h

main:
; impresion cadena
  mov dx,offset cad000
  mov ah,9
  int 21h
  call saltaren
; leer(a)
  call lecdec
  mov a,ax
  call saltaren
; impresion cadena
  mov dx,offset cad001
  mov ah,9
  int 21h
  call saltaren
; leer(b)
  call lecdec
  mov b,ax
  call saltaren
; ASIGNACION
  mov ax,0
  push ax
  pop ax
  mov q,ax
; while a >= b
e000: 
; condicion
  push a
  push b
; operRelac ]
  pop bx
  pop ax
  cmp ax,bx
  jge e001
  mov cx,0
  jmp e002
  e001: mov cx,1
  e002: push cx
  pop ax
  cmp ax,0
  jz e003
; ASIGNACION
  push q
  mov ax,1
  push ax
; operAritm +
  pop bx
  pop ax
  add ax,bx
  push ax
  pop ax
  mov q,ax
; ASIGNACION
  push a
  push b
; operAritm -
  pop bx
  pop ax
  sub ax,bx
  push ax
  pop ax
  mov a,ax
  jmp e000
e003: 
; impresion cadena
  mov dx,offset cad002
  mov ah,9
  int 21h
  call saltaren
; imprimir(q)
  mov ax,q
  call impdec
  call saltaren
; impresion cadena
  mov dx,offset cad003
  mov ah,9
  int 21h
  call saltaren
; imprimir(a)
  mov ax,a
  call impdec
  call saltaren

int 20h


impdec	proc near
  push ax
  push bx
  push cx
  push dx
  xor cx,cx
  mov bx,10
impdec0: xor dx, dx
  div bx
  add dl,'0'
  push dx
  inc cx
  cmp ax,0
  jne impdec0
impdec1: pop dx
  mov ah,2
  int 21h
  loop impdec1
  pop dx
  pop cx
  pop bx
  pop ax
  ret
impdec	endp

lecdec	proc near
  push bx
  push cx
  push dx
  push si
  mov dx, offset buffer
  mov ah,10
  int 21h
  xor ax,ax
  mov bx,10
  mov si, offset buffer
  inc si
  mov cl,[si]
  xor ch, ch
lecdec0: inc si
  mul bx
  mov dl,[si]
  sub dl,'0'
  xor dh, dh
  add ax, dx
  loop lecdec0
  pop si
  pop dx
  pop cx
  pop bx
  ret
lecdec	endp

saltaren proc near
  push ax
  push dx
  mov dl,13
  mov ah,2
  int 21h
  mov dl,10
  mov ah,2
  int 21h
  pop dx
  pop ax
  ret
saltaren endp

msgLec	db 'Ingrese valor de '
var1	db 0
	db ': ','$'
msgImp	db 'El valor de '
var2 	db 0
	db ' es: $'

cad000 db 'n: $'
cad001 db 'b: $'
cad002 db 'Cocientes: $'
cad003 db 'Residuos: $'

buffer db 6,0,6 dup(?)

a dw 0
b dw 0
c dw 0
d dw 0
e dw 0
f dw 0
g dw 0
h dw 0
i dw 0
j dw 0
k dw 0
l dw 0
m dw 0
n dw 0
o dw 0
p dw 0
q dw 0
r dw 0
s dw 0
t dw 0
u dw 0
v dw 0
w dw 0
x dw 0
y dw 0
z dw 0

text	ends
		end main
