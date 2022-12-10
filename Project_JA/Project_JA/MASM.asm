; power2.asm 
; x86 code for C interop
; Command line: ml /c /Cx /W3 /WX power2.asm 
        .686P
        .XMM
        .MODEL  flat

PUBLIC  _power2
; int power2(int num, int power);
; computes num x 2^power
_TEXT   SEGMENT
_power2 PROC
        push    ebp             ; save EBP
        mov     ebp, esp        ; Move ESP into EBP so we can refer
                                ;   to arguments on the stack
        mov     eax, [ebp+8]    ; load first argument
        mov     ecx, [ebp+12]   ; load second argument
        shl     eax, cl         ; compute result in EAX
        pop     ebp             ; restore EBP
        ret
_power2 ENDP
_TEXT   ENDS
END