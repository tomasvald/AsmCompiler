text segment
  assume cs:text, ds:text, ss:text
  org 100h


main:
; print "n: "
  mov dx, offset msg000
  mov ah,9
  int 21h
; read n
  call lecdec
  mov n,ax
  call saltaren
; i = 1
  mov ax,1
  push ax
  pop ax
  mov i,ax
; f = 1
  mov ax,1
  push ax
  pop ax
  mov f,ax
; while i<=n
e000:
; i <= n
  push i
  push n
  pop bx
  pop ax
  cmp ax,bx
  jle e001      
  mov ax,0
  jmp e002
e001: mov ax,1
e002: push ax
; verificacion del ciclo while
  pop ax
  jz e003       ; fin de while si no se cumle i<=n
; f= f*i
  push f
  push i
  pop bx
  pop ax
  mul bx
  push ax
  pop f
; i = i+1
  push i
  mov ax,1
  push ax
  pop bx
  pop ax
  add ax,bx
  push ax
  pop i
  jmp e000
; fin de while
e003:
; print "El factorial es: "  
  mov dx, offset msg001
  mov ah,9
  int 21h
; println f
  mov ax,f
  call impdec
  call saltaren
  int 20h

msg000 db 'n: $'
msg001 db 'El factorial es: $'
f dw 0
i dw 0

; impdec, saltaren, lecdec

text ends
end main
