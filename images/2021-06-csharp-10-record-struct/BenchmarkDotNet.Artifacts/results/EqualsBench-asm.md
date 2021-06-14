## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; EqualsBench.PlainStruct_Equals()
       push      r14
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,20
       mov       rsi,rcx
       mov       rcx,offset MT_PlainStruct
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       lea       rbp,[rsi+8]
       add       rsi,78
       lea       rdi,[rbx+8]
       call      CORINFO_HELP_ASSIGN_BYREF
       movsq
       mov       rcx,offset MT_PlainStruct
       call      CORINFO_HELP_NEWSFAST
       mov       r14,rax
       lea       rdi,[r14+8]
       mov       rsi,rbp
       call      CORINFO_HELP_ASSIGN_BYREF
       movsq
       mov       rcx,r14
       mov       rdx,rbx
       mov       rax,[7FFF1E3A0E38]
       add       rsp,20
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       pop       r14
       jmp       rax
; Total bytes of code 108
```
```assembly
; System.ValueType.Equals(System.Object)
       push      r15
       push      r14
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,48
       vxorps    xmm4,xmm4,xmm4
       vmovdqa   xmmword ptr [rsp+30],xmm4
       xor       eax,eax
       mov       [rsp+40],rax
       mov       rdi,rcx
       mov       rsi,rdx
       test      rsi,rsi
       je        near ptr M01_L05
       mov       rcx,rdi
       call      00007FFF7DC60330
       mov       rbx,rax
       mov       rcx,rsi
       call      00007FFF7DC60330
       cmp       rax,rbx
       jne       near ptr M01_L05
       mov       rcx,rdi
       call      00007FFF7DC80780
       test      eax,eax
       je        short M01_L00
       mov       rcx,rdi
       mov       rdx,rsi
       call      System.ValueType.FastEqualsCheck(System.Object, System.Object)
       jmp       near ptr M01_L06
M01_L00:
       cmp       [rbx],ebx
       xor       edx,edx
       mov       [rsp+20],edx
       lea       rdx,[rsp+30]
       mov       rcx,rbx
       xor       r8d,r8d
       mov       r9d,34
       call      System.RuntimeType.GetFieldCandidates(System.String, System.Reflection.BindingFlags, Boolean)
       lea       rcx,[rsp+30]
       mov       rdx,offset MT_System.RuntimeType+ListBuilder`1[[System.Reflection.FieldInfo, System.Private.CoreLib]]
       call      System.RuntimeType+ListBuilder`1[[System.__Canon, System.Private.CoreLib]].ToArray()
       mov       rbx,rax
       xor       ebp,ebp
       mov       r14d,[rbx+8]
       test      r14d,r14d
       jle       short M01_L04
M01_L01:
       movsxd    rcx,ebp
       mov       rcx,[rbx+rcx*8+10]
       mov       rdx,rdi
       mov       rax,[rcx]
       mov       rax,[rax+58]
       call      qword ptr [rax]
       mov       r15,rax
       movsxd    rcx,ebp
       mov       rcx,[rbx+rcx*8+10]
       mov       rdx,rsi
       mov       rax,[rcx]
       mov       rax,[rax+58]
       call      qword ptr [rax]
       test      r15,r15
       jne       short M01_L02
       test      rax,rax
       je        short M01_L03
       jmp       short M01_L05
M01_L02:
       mov       rcx,r15
       mov       rdx,rax
       mov       rax,[r15]
       mov       rax,[rax+40]
       call      qword ptr [rax+10]
       test      eax,eax
       je        short M01_L05
M01_L03:
       inc       ebp
       cmp       r14d,ebp
       jg        short M01_L01
M01_L04:
       mov       eax,1
       add       rsp,48
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       pop       r14
       pop       r15
       ret
M01_L05:
       xor       eax,eax
       add       rsp,48
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       pop       r14
       pop       r15
       ret
M01_L06:
       movzx     eax,al
       add       rsp,48
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       pop       r14
       pop       r15
       ret
; Total bytes of code 295
```

## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; EqualsBench.EquatableStruct_Equals()
       push      rdi
       push      rsi
       sub       rsp,28
       cmp       [rcx],ecx
       lea       rsi,[rcx+18]
       add       rcx,88
       mov       rdx,[rcx]
       mov       edi,[rcx+8]
       mov       rcx,[rsi]
       call      System.Type.op_Equality(System.Type, System.Type)
       test      eax,eax
       je        short M00_L00
       mov       eax,[rsi+8]
       cmp       eax,edi
       sete      al
       movzx     eax,al
       jmp       short M00_L01
M00_L00:
       xor       eax,eax
M00_L01:
       add       rsp,28
       pop       rsi
       pop       rdi
       ret
; Total bytes of code 59
```
**Extern method**
System.Type.op_Equality(System.Type, System.Type)

## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; EqualsBench.HashStruct_Equals()
       push      r14
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,20
       mov       rsi,rcx
       mov       rcx,offset MT_HashStruct
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       lea       rbp,[rsi+28]
       add       rsi,98
       lea       rdi,[rbx+8]
       call      CORINFO_HELP_ASSIGN_BYREF
       movsq
       mov       rcx,offset MT_HashStruct
       call      CORINFO_HELP_NEWSFAST
       mov       r14,rax
       lea       rdi,[r14+8]
       mov       rsi,rbp
       call      CORINFO_HELP_ASSIGN_BYREF
       movsq
       mov       rcx,r14
       mov       rdx,rbx
       mov       rax,[7FFF1E3A0E38]
       add       rsp,20
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       pop       r14
       jmp       rax
; Total bytes of code 111
```
```assembly
; System.ValueType.Equals(System.Object)
       push      r15
       push      r14
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,48
       vxorps    xmm4,xmm4,xmm4
       vmovdqa   xmmword ptr [rsp+30],xmm4
       xor       eax,eax
       mov       [rsp+40],rax
       mov       rdi,rcx
       mov       rsi,rdx
       test      rsi,rsi
       je        near ptr M01_L05
       mov       rcx,rdi
       call      00007FFF7DC60330
       mov       rbx,rax
       mov       rcx,rsi
       call      00007FFF7DC60330
       cmp       rax,rbx
       jne       near ptr M01_L05
       mov       rcx,rdi
       call      00007FFF7DC80780
       test      eax,eax
       je        short M01_L00
       mov       rcx,rdi
       mov       rdx,rsi
       call      System.ValueType.FastEqualsCheck(System.Object, System.Object)
       jmp       near ptr M01_L06
M01_L00:
       cmp       [rbx],ebx
       xor       edx,edx
       mov       [rsp+20],edx
       lea       rdx,[rsp+30]
       mov       rcx,rbx
       xor       r8d,r8d
       mov       r9d,34
       call      System.RuntimeType.GetFieldCandidates(System.String, System.Reflection.BindingFlags, Boolean)
       lea       rcx,[rsp+30]
       mov       rdx,offset MT_System.RuntimeType+ListBuilder`1[[System.Reflection.FieldInfo, System.Private.CoreLib]]
       call      System.RuntimeType+ListBuilder`1[[System.__Canon, System.Private.CoreLib]].ToArray()
       mov       rbx,rax
       xor       ebp,ebp
       mov       r14d,[rbx+8]
       test      r14d,r14d
       jle       short M01_L04
M01_L01:
       movsxd    rcx,ebp
       mov       rcx,[rbx+rcx*8+10]
       mov       rdx,rdi
       mov       rax,[rcx]
       mov       rax,[rax+58]
       call      qword ptr [rax]
       mov       r15,rax
       movsxd    rcx,ebp
       mov       rcx,[rbx+rcx*8+10]
       mov       rdx,rsi
       mov       rax,[rcx]
       mov       rax,[rax+58]
       call      qword ptr [rax]
       test      r15,r15
       jne       short M01_L02
       test      rax,rax
       je        short M01_L03
       jmp       short M01_L05
M01_L02:
       mov       rcx,r15
       mov       rdx,rax
       mov       rax,[r15]
       mov       rax,[rax+40]
       call      qword ptr [rax+10]
       test      eax,eax
       je        short M01_L05
M01_L03:
       inc       ebp
       cmp       r14d,ebp
       jg        short M01_L01
M01_L04:
       mov       eax,1
       add       rsp,48
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       pop       r14
       pop       r15
       ret
M01_L05:
       xor       eax,eax
       add       rsp,48
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       pop       r14
       pop       r15
       ret
M01_L06:
       movzx     eax,al
       add       rsp,48
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       pop       r14
       pop       r15
       ret
; Total bytes of code 295
```

## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; EqualsBench.HashEquatableStruct_Equals()
       push      rdi
       push      rsi
       sub       rsp,28
       cmp       [rcx],ecx
       lea       rsi,[rcx+38]
       add       rcx,0A8
       mov       rdx,[rcx]
       mov       edi,[rcx+8]
       mov       rcx,[rsi]
       call      System.Type.op_Equality(System.Type, System.Type)
       test      eax,eax
       je        short M00_L00
       mov       eax,[rsi+8]
       cmp       eax,edi
       sete      al
       movzx     eax,al
       jmp       short M00_L01
M00_L00:
       xor       eax,eax
M00_L01:
       add       rsp,28
       pop       rsi
       pop       rdi
       ret
; Total bytes of code 59
```
**Extern method**
System.Type.op_Equality(System.Type, System.Type)

## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; EqualsBench.ValueTuple_Equals()
       push      rdi
       push      rsi
       sub       rsp,28
       lea       rdx,[rcx+48]
       mov       r8,[rdx]
       mov       esi,[rdx+8]
       add       rcx,0B8
       mov       rax,[rcx]
       mov       edi,[rcx+8]
       mov       rcx,208D25B79B8
       mov       rcx,[rcx]
       mov       rdx,r8
       mov       r8,rax
       call      qword ptr [7FFF1E392B00]
       test      eax,eax
       je        short M00_L00
       cmp       esi,edi
       sete      al
       movzx     eax,al
       jmp       short M00_L01
M00_L00:
       xor       eax,eax
M00_L01:
       add       rsp,28
       pop       rsi
       pop       rdi
       ret
; Total bytes of code 77
```
```assembly
; System.Collections.Generic.ObjectEqualityComparer`1[[System.__Canon, System.Private.CoreLib]].Equals(System.__Canon, System.__Canon)
       sub       rsp,28
       test      rdx,rdx
       je        short M01_L01
       test      r8,r8
       je        short M01_L00
       mov       [rsp+38],rdx
       mov       rcx,rdx
       mov       rdx,r8
       mov       rax,[rsp+38]
       mov       rax,[rax]
       mov       rax,[rax+40]
       call      qword ptr [rax+10]
       nop
       add       rsp,28
       ret
M01_L00:
       xor       eax,eax
       add       rsp,28
       ret
M01_L01:
       test      r8,r8
       je        short M01_L02
       xor       eax,eax
       add       rsp,28
       ret
M01_L02:
       mov       eax,1
       add       rsp,28
       ret
; Total bytes of code 75
```

## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; EqualsBench.RecordStruct_Equals()
       push      rdi
       push      rsi
       sub       rsp,28
       cmp       [rcx],ecx
       lea       rsi,[rcx+58]
       add       rcx,0C8
       mov       r8,[rcx]
       mov       edi,[rcx+8]
       mov       rdx,[rsi]
       mov       rcx,224A14179B8
       mov       rcx,[rcx]
       call      qword ptr [7FFF1E362B00]
       test      eax,eax
       je        short M00_L00
       mov       eax,[rsi+8]
       cmp       eax,edi
       sete      al
       movzx     eax,al
       jmp       short M00_L01
M00_L00:
       xor       eax,eax
M00_L01:
       add       rsp,28
       pop       rsi
       pop       rdi
       ret
; Total bytes of code 73
```
```assembly
; System.Collections.Generic.ObjectEqualityComparer`1[[System.__Canon, System.Private.CoreLib]].Equals(System.__Canon, System.__Canon)
       sub       rsp,28
       test      rdx,rdx
       je        short M01_L01
       test      r8,r8
       je        short M01_L00
       mov       [rsp+38],rdx
       mov       rcx,rdx
       mov       rdx,r8
       mov       rax,[rsp+38]
       mov       rax,[rax]
       mov       rax,[rax+40]
       call      qword ptr [rax+10]
       nop
       add       rsp,28
       ret
M01_L00:
       xor       eax,eax
       add       rsp,28
       ret
M01_L01:
       test      r8,r8
       je        short M01_L02
       xor       eax,eax
       add       rsp,28
       ret
M01_L02:
       mov       eax,1
       add       rsp,28
       ret
; Total bytes of code 75
```

## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; EqualsBench.HashEquatableRecordStruct_Equals()
       push      rdi
       push      rsi
       sub       rsp,28
       cmp       [rcx],ecx
       lea       rsi,[rcx+68]
       add       rcx,0D8
       mov       rdx,[rcx]
       mov       edi,[rcx+8]
       mov       rcx,[rsi]
       call      System.Type.op_Equality(System.Type, System.Type)
       test      eax,eax
       je        short M00_L00
       mov       eax,[rsi+8]
       cmp       eax,edi
       sete      al
       movzx     eax,al
       jmp       short M00_L01
M00_L00:
       xor       eax,eax
M00_L01:
       add       rsp,28
       pop       rsi
       pop       rdi
       ret
; Total bytes of code 59
```
**Extern method**
System.Type.op_Equality(System.Type, System.Type)

