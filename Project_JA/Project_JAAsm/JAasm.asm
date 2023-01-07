.data
blue dd 0
green dd 0
red dd 0
counter dd 0
bitmapBytes dd 0
alfa dd 0

.code
MyProc1 proc
    ;skopiowanie bitmapBytes
    movq mm7, rcx

    ;zanegowanie i skopiowanie -blurSize
    neg r9
    movq mm5, r9

    ;skopiowanie blurSize + 1
    neg r9
    add r9, 1
    movq mm4, r9

    ;skopiowanie data.Width
    movq mm6, rbx

    mov rcx, r11

    mov rax, 0
    movq mm0, rax
    movq mm1, rax
    movq mm2, rax
    
    ;zmienna dy
        movq r12, mm5

            row_loop: 
            ;zmienna dx
            movq r14, mm5

              col_loop:
                ;obliczamy pozycje piksela w macierzy
                mov rax, rcx
                add rax, r14
                mov rbx, rax

                mov rax, r10 ;pobranie wartoœci y
                add rax, r12
                mov rdx, rax

                
                ;sprawdzamy warunek czy aktualna pozycje nie znajduje sie poza obrazem
                cmp rbx, 0
                jl col_loop_check
                movd rax, mm6
                cmp rbx, rax
                jae col_loop_check
                cmp rdx, 0
                jl col_loop_check
                cmp rdx, r8
                jae col_loop_check
              
                ;pobieramy adres aktualnego piksela
                mov rax, rdx
                movd r13, mm6
                imul r13, 4 ;obliczamy data.Stride(data.Width * 4)
                imul rax, r13
                mov rdx, rax;przechowujemy pomno¿on¹ wartoœæ w rdx
                mov rax, rbx
                imul rax, 4
                add rdx, rax; wartoœæ piksela znajdujê sie w rdx


                movq rbx, mm7
                mov eax, [rbx+rdx]
                movzx eax, al
                movq mm3, rax
                paddw mm0, mm3
                mov eax, [rbx+rdx]
                shr eax, 8 
                movzx eax, al
                movq mm3, rax
                paddw mm1, mm3
                mov eax, [rbx+rdx]
                shr eax, 16 
                movzx eax, al
                movq mm3, rax
                paddw mm2, mm3
                

                col_loop_check:
                    inc [counter]
                    movd mm3, [counter]
                    add r14, 1
                    movd rax, mm4 
                    cmp rax, r14
                    jne col_loop
            
            row_loop_check: 
                add r12, 1
                movd rax, mm4
                cmp rax, r12
                jne row_loop
    movd rax, mm0
	ret
MyProc1 endp

.code
MyProc2 proc
    movd rax, mm1
	ret
MyProc2 endp

.code
MyProc3 proc
    movd rax, mm2
	ret
MyProc3 endp
end