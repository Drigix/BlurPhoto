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
    ;wyzerowanie licznika pêtli
    xor rcx, rcx

     iterate_row:
        ;inicjowanie danych
        mov rax, 0
        movd mm0, rax
        mov rax, 0
        movd mm1, rax
        mov rax, 0
        movd mm2, rax
        mov [counter], 0

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

                mov rax, r10
                movd r13, mm6
                imul r13, 4 ;obliczamy data.Stride(data.Width * 4)
                imul rax, r13
                 mov rdx, rax;przechowujemy pomno¿on¹ wartoœæ w rdx
                mov rax, rcx
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
                mov eax, [rbx+rdx]
                shr eax, 24 
                movzx eax, al
                mov [alfa], eax

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
        
        ;sprawdzam warunek czy punkty z najduj¹ siê w wybranym okrêgu   
        movd r13, mm6
        shr r13, 1
        mov rax, rcx
        sub rax, r13
        imul rax, rax
        jns positive_x
        neg rax
        positive_x:
        mov rbx, rax

        mov r13, r8
        shr r13, 1
        mov rax, r10
        sub rax, r13
        imul rax, rax
        jns positive_y
        neg rax
        positive_y:
        add rbx, rax

        mov rax, 200
        imul rax, rax

        cmp rbx, rax
        jle iterate_row_check

        ;dzielimy wartoœcix
        cvtpi2ps xmm0, mm0
        cvtpi2ps xmm1, mm1
        cvtpi2ps xmm2, mm2
        cvtpi2ps xmm3, mm3
        divss xmm0, xmm3
        divss xmm1, xmm3
        divss xmm2, xmm3
        cvtps2pi mm0, xmm0
        cvtps2pi mm1, xmm1
        cvtps2pi mm2, xmm2
       
        
        ;ustawiamy wartoœæi 
         mov rax, r10
         movd r13, mm6
         imul r13, 4 ;obliczamy data.Stride(data.Width * 4)
         imul rax, r13
         mov r14, rax;przechowujemy pomno¿on¹ wartoœæ w rdx
         mov rax, rcx
         imul rax, 4
         add r14, rax; wartoœæ piksela znajdujê sie w rdx

         movq rbx, mm7
         movd edx, mm0 ; Przenosi pierwsze 32 bity z rejestru MM0 do rejestru EDX
         movd ebx, mm1 ; Przenosi pierwsze 32 bity z rejestru MM1 do rejestru ECX
         shl ebx, 8 ; Przesuwa bity w ECX o 8 pozycji w lewo
         or edx, ebx ; £¹czy wartoœæ z ECX z EDX
         movd ebx, mm2 ; Przenosi pierwsze 32 bity z rejestru MM3 do rejestru ECX
         shl ebx, 16 ; Przesuwa bity w ECX o 16 pozycji w lewo
         or edx, ebx ; £¹czy wartoœæ z ECX z EDX
         mov ebx, [alfa] ; Przenosi pierwsze 32 bity z rejestru MM3 do rejestru ECX
         shl ebx, 24 ; Przesuwa bity w ECX o 16 pozycji w lewo
         or edx, ebx ; £¹czy wartoœæ z ECX z EDX
         mov eax, edx ; £¹czy wartoœæ z EDX z EAX

         movq rbx, mm7
         mov [rbx+r14], eax

        iterate_row_check:
            add rcx, 1
            movd rax, mm6 
            cmp rax, rcx
            jne iterate_row
    movd rax, mm1
	ret
MyProc1 endp
end