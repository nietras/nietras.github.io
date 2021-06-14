## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; GetHashCodeBench.PlainStruct_GetHashCode()
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,20
       mov       rsi,rcx
       mov       rcx,offset MT_PlainStruct
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       add       rsi,8
       lea       rdi,[rbx+8]
       call      CORINFO_HELP_ASSIGN_BYREF
       movsq
       mov       rcx,rbx
       add       rsp,20
       pop       rbx
       pop       rsi
       pop       rdi
       jmp       near ptr 00007FFF7DC808C0
; Total bytes of code 58
```

## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; GetHashCodeBench.EquatableStruct_GetHashCode()
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,20
       mov       rsi,rcx
       mov       rcx,offset MT_EquatableStruct
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       add       rsi,18
       lea       rdi,[rbx+8]
       call      CORINFO_HELP_ASSIGN_BYREF
       movsq
       mov       rcx,rbx
       add       rsp,20
       pop       rbx
       pop       rsi
       pop       rdi
       jmp       near ptr 00007FFF7DC808C0
; Total bytes of code 58
```

## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; GetHashCodeBench.HashStruct_GetHashCode()
       cmp       [rcx],ecx
       add       rcx,28
       jmp       near ptr HashStruct.GetHashCode()
; Total bytes of code 11
```
```assembly
; HashStruct.GetHashCode()
       push      rsi
       sub       rsp,20
       mov       rsi,rcx
       mov       rcx,[rsi]
       mov       rax,[rcx]
       mov       rax,[rax+40]
       call      qword ptr [rax+18]
       imul      eax,0A5555529
       mov       edx,[rsi+8]
       add       eax,edx
       add       rsp,20
       pop       rsi
       ret
; Total bytes of code 38
```

## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; GetHashCodeBench.HashEquatableStruct_GetHashCode()
       cmp       [rcx],ecx
       add       rcx,38
       jmp       near ptr HashEquatableStruct.GetHashCode()
; Total bytes of code 11
```
```assembly
; HashEquatableStruct.GetHashCode()
       push      rsi
       sub       rsp,20
       mov       rsi,rcx
       mov       rcx,[rsi]
       mov       rax,[rcx]
       mov       rax,[rax+40]
       call      qword ptr [rax+18]
       imul      eax,0A5555529
       mov       edx,[rsi+8]
       add       eax,edx
       add       rsp,20
       pop       rsi
       ret
; Total bytes of code 38
```

## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; GetHashCodeBench.ValueTuple_GetHashCode()
       sub       rsp,38
       xor       eax,eax
       mov       [rsp+28],rax
       mov       [rsp+30],rax
       add       rcx,48
       mov       rdx,[rcx]
       mov       [rsp+28],rdx
       mov       ecx,[rcx+8]
       mov       [rsp+30],ecx
       lea       rcx,[rsp+28]
       mov       rdx,offset MT_System.ValueTuple`2[[System.Type, System.Private.CoreLib],[System.Int32, System.Private.CoreLib]]
       call      System.ValueTuple`2[[System.__Canon, System.Private.CoreLib],[System.Int32, System.Private.CoreLib]].GetHashCode()
       nop
       add       rsp,38
       ret
; Total bytes of code 61
```
```assembly
; System.ValueTuple`2[[System.__Canon, System.Private.CoreLib],[System.Int32, System.Private.CoreLib]].GetHashCode()
       push      rsi
       sub       rsp,30
       xor       eax,eax
       mov       [rsp+28],rax
       mov       rsi,rcx
       mov       rcx,rsi
       cmp       qword ptr [rsp+28],0
       jne       short M01_L00
       mov       rcx,[rsi]
       mov       [rsp+28],rcx
       lea       rcx,[rsp+28]
       cmp       qword ptr [rsp+28],0
       jne       short M01_L00
       xor       ecx,ecx
       jmp       short M01_L01
M01_L00:
       mov       rcx,[rcx]
       mov       rax,[rcx]
       mov       rax,[rax+40]
       call      qword ptr [rax+18]
       mov       ecx,eax
M01_L01:
       add       rsi,8
       mov       edx,[rsi]
       call      System.HashCode.Combine[[System.Int32, System.Private.CoreLib],[System.Int32, System.Private.CoreLib]](Int32, Int32)
       nop
       add       rsp,30
       pop       rsi
       ret
; Total bytes of code 84
```

## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; GetHashCodeBench.RecordStruct_GetHashCode()
       cmp       [rcx],ecx
       add       rcx,58
       jmp       near ptr RecordStruct.GetHashCode()
; Total bytes of code 11
```
```assembly
; RecordStruct.GetHashCode()
       push      rsi
       sub       rsp,20
       mov       rsi,rcx
       mov       rdx,[rsi]
       mov       rcx,1F43DD779B8
       mov       rcx,[rcx]
       call      qword ptr [7FFF1E392B08]
       imul      eax,0A5555529
       mov       edx,[rsi+8]
       add       eax,edx
       add       rsp,20
       pop       rsi
       ret
; Total bytes of code 47
```

## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; GetHashCodeBench.HashEquatableRecordStruct_GetHashCode()
       cmp       [rcx],ecx
       add       rcx,68
       jmp       near ptr HashEquatableRecordStruct.GetHashCode()
; Total bytes of code 11
```
```assembly
; HashEquatableRecordStruct.GetHashCode()
       push      rsi
       sub       rsp,20
       mov       rsi,rcx
       mov       rcx,[rsi]
       mov       rax,[rcx]
       mov       rax,[rax+40]
       call      qword ptr [rax+18]
       imul      eax,0A5555529
       mov       edx,[rsi+8]
       add       eax,edx
       add       rsp,20
       pop       rsi
       ret
; Total bytes of code 38
```

