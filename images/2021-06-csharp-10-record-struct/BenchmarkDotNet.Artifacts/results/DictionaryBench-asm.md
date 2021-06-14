## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; DictionaryBench.PlainStruct_DictionaryGet()
       sub       rsp,38
       vzeroupper
       mov       rdx,[rcx+78]
       vmovdqu   xmm0,xmmword ptr [rcx+8]
       vmovdqu   xmmword ptr [rsp+28],xmm0
       mov       rcx,rdx
       lea       rdx,[rsp+28]
       cmp       [rcx],ecx
       call      qword ptr [7FFF1E399948]
       nop
       add       rsp,38
       ret
; Total bytes of code 44
```
```assembly
; System.Collections.Generic.Dictionary`2[[PlainStruct, RecordStructBenchmark],[System.Int64, System.Private.CoreLib]].get_Item(PlainStruct)
       push      rsi
       sub       rsp,30
       vzeroupper
       mov       rsi,rdx
       vmovdqu   xmm0,xmmword ptr [rsi]
       vmovdqu   xmmword ptr [rsp+20],xmm0
       lea       rdx,[rsp+20]
       call      System.Collections.Generic.Dictionary`2[[PlainStruct, RecordStructBenchmark],[System.Int64, System.Private.CoreLib]].FindValue(PlainStruct)
       test      rax,rax
       je        short M01_L00
       mov       rax,[rax]
       add       rsp,30
       pop       rsi
       ret
M01_L00:
       vmovdqu   xmm0,xmmword ptr [rsi]
       vmovdqu   xmmword ptr [rsp+20],xmm0
       lea       rcx,[rsp+20]
       call      System.ThrowHelper.ThrowKeyNotFoundException[[PlainStruct, RecordStructBenchmark]](PlainStruct)
       int       3
; Total bytes of code 66
```

## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; DictionaryBench.EquatableStruct_DictionaryGet()
       sub       rsp,38
       vzeroupper
       mov       rdx,[rcx+80]
       vmovdqu   xmm0,xmmword ptr [rcx+18]
       vmovdqu   xmmword ptr [rsp+28],xmm0
       mov       rcx,rdx
       lea       rdx,[rsp+28]
       cmp       [rcx],ecx
       call      qword ptr [7FFF1E39AD30]
       nop
       add       rsp,38
       ret
; Total bytes of code 47
```
```assembly
; System.Collections.Generic.Dictionary`2[[EquatableStruct, RecordStructBenchmark],[System.Int64, System.Private.CoreLib]].get_Item(EquatableStruct)
       push      rsi
       sub       rsp,30
       vzeroupper
       mov       rsi,rdx
       vmovdqu   xmm0,xmmword ptr [rsi]
       vmovdqu   xmmword ptr [rsp+20],xmm0
       lea       rdx,[rsp+20]
       call      System.Collections.Generic.Dictionary`2[[EquatableStruct, RecordStructBenchmark],[System.Int64, System.Private.CoreLib]].FindValue(EquatableStruct)
       test      rax,rax
       je        short M01_L00
       mov       rax,[rax]
       add       rsp,30
       pop       rsi
       ret
M01_L00:
       vmovdqu   xmm0,xmmword ptr [rsi]
       vmovdqu   xmmword ptr [rsp+20],xmm0
       lea       rcx,[rsp+20]
       call      System.ThrowHelper.ThrowKeyNotFoundException[[EquatableStruct, RecordStructBenchmark]](EquatableStruct)
       int       3
; Total bytes of code 66
```

## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; DictionaryBench.HashStruct_DictionaryGet()
       sub       rsp,38
       vzeroupper
       mov       rdx,[rcx+88]
       vmovdqu   xmm0,xmmword ptr [rcx+28]
       vmovdqu   xmmword ptr [rsp+28],xmm0
       mov       rcx,rdx
       lea       rdx,[rsp+28]
       cmp       [rcx],ecx
       call      qword ptr [7FFF1E37C118]
       nop
       add       rsp,38
       ret
; Total bytes of code 47
```
```assembly
; System.Collections.Generic.Dictionary`2[[HashStruct, RecordStructBenchmark],[System.Int64, System.Private.CoreLib]].get_Item(HashStruct)
       push      rsi
       sub       rsp,30
       vzeroupper
       mov       rsi,rdx
       vmovdqu   xmm0,xmmword ptr [rsi]
       vmovdqu   xmmword ptr [rsp+20],xmm0
       lea       rdx,[rsp+20]
       call      System.Collections.Generic.Dictionary`2[[HashStruct, RecordStructBenchmark],[System.Int64, System.Private.CoreLib]].FindValue(HashStruct)
       test      rax,rax
       je        short M01_L00
       mov       rax,[rax]
       add       rsp,30
       pop       rsi
       ret
M01_L00:
       vmovdqu   xmm0,xmmword ptr [rsi]
       vmovdqu   xmmword ptr [rsp+20],xmm0
       lea       rcx,[rsp+20]
       call      System.ThrowHelper.ThrowKeyNotFoundException[[HashStruct, RecordStructBenchmark]](HashStruct)
       int       3
; Total bytes of code 66
```

## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; DictionaryBench.HashEquatableStruct_DictionaryGet()
       sub       rsp,38
       vzeroupper
       mov       rdx,[rcx+90]
       vmovdqu   xmm0,xmmword ptr [rcx+38]
       vmovdqu   xmmword ptr [rsp+28],xmm0
       mov       rcx,rdx
       lea       rdx,[rsp+28]
       cmp       [rcx],ecx
       call      qword ptr [7FFF1E37D500]
       nop
       add       rsp,38
       ret
; Total bytes of code 47
```
```assembly
; System.Collections.Generic.Dictionary`2[[HashEquatableStruct, RecordStructBenchmark],[System.Int64, System.Private.CoreLib]].get_Item(HashEquatableStruct)
       push      rsi
       sub       rsp,30
       vzeroupper
       mov       rsi,rdx
       vmovdqu   xmm0,xmmword ptr [rsi]
       vmovdqu   xmmword ptr [rsp+20],xmm0
       lea       rdx,[rsp+20]
       call      System.Collections.Generic.Dictionary`2[[HashEquatableStruct, RecordStructBenchmark],[System.Int64, System.Private.CoreLib]].FindValue(HashEquatableStruct)
       test      rax,rax
       je        short M01_L00
       mov       rax,[rax]
       add       rsp,30
       pop       rsi
       ret
M01_L00:
       vmovdqu   xmm0,xmmword ptr [rsi]
       vmovdqu   xmmword ptr [rsp+20],xmm0
       lea       rcx,[rsp+20]
       call      System.ThrowHelper.ThrowKeyNotFoundException[[HashEquatableStruct, RecordStructBenchmark]](HashEquatableStruct)
       int       3
; Total bytes of code 66
```

## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; DictionaryBench.ValueTuple_DictionaryGet()
       sub       rsp,38
       vzeroupper
       mov       rdx,[rcx+98]
       vmovdqu   xmm0,xmmword ptr [rcx+48]
       vmovdqu   xmmword ptr [rsp+28],xmm0
       mov       rcx,rdx
       lea       rdx,[rsp+28]
       cmp       [rcx],ecx
       call      qword ptr [7FFF1E38EAD8]
       nop
       add       rsp,38
       ret
; Total bytes of code 47
```
```assembly
; System.Collections.Generic.Dictionary`2[[System.ValueTuple`2[[System.__Canon, System.Private.CoreLib],[System.Int32, System.Private.CoreLib]], System.Private.CoreLib],[System.Int64, System.Private.CoreLib]].get_Item(System.ValueTuple`2<System.__Canon,Int32>)
       push      rdi
       push      rsi
       sub       rsp,38
       vzeroupper
       mov       [rsp+30],rcx
       mov       rdi,rcx
       mov       rsi,rdx
       mov       rcx,rdi
       vmovdqu   xmm0,xmmword ptr [rsi]
       vmovdqu   xmmword ptr [rsp+20],xmm0
       lea       rdx,[rsp+20]
       call      System.Collections.Generic.Dictionary`2[[System.ValueTuple`2[[System.__Canon, System.Private.CoreLib],[System.Int32, System.Private.CoreLib]], System.Private.CoreLib],[System.Int64, System.Private.CoreLib]].FindValue(System.ValueTuple`2<System.__Canon,Int32>)
       test      rax,rax
       je        short M01_L00
       mov       rax,[rax]
       add       rsp,38
       pop       rsi
       pop       rdi
       ret
M01_L00:
       mov       rcx,[rdi]
       mov       rdx,[rcx+30]
       mov       rdx,[rdx]
       cmp       qword ptr [rdx+50],0
       je        short M01_L01
       mov       rcx,[rcx+30]
       mov       rcx,[rcx]
       mov       rcx,[rcx+50]
       jmp       short M01_L02
M01_L01:
       mov       rdx,7FFF1E42B6E0
       call      CORINFO_HELP_RUNTIMEHANDLE_CLASS
       mov       rcx,rax
M01_L02:
       vmovdqu   xmm0,xmmword ptr [rsi]
       vmovdqu   xmmword ptr [rsp+20],xmm0
       lea       rdx,[rsp+20]
       call      System.ThrowHelper.ThrowKeyNotFoundException[[System.ValueTuple`2[[System.__Canon, System.Private.CoreLib],[System.Int32, System.Private.CoreLib]], System.Private.CoreLib]](System.ValueTuple`2<System.__Canon,Int32>)
       int       3
; Total bytes of code 127
```

## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; DictionaryBench.RecordStruct_DictionaryGet()
       sub       rsp,38
       vzeroupper
       mov       rdx,[rcx+0A0]
       vmovdqu   xmm0,xmmword ptr [rcx+58]
       vmovdqu   xmmword ptr [rsp+28],xmm0
       mov       rcx,rdx
       lea       rdx,[rsp+28]
       cmp       [rcx],ecx
       call      qword ptr [7FFF1E3806E0]
       nop
       add       rsp,38
       ret
; Total bytes of code 47
```
```assembly
; System.Collections.Generic.Dictionary`2[[RecordStruct, RecordStructBenchmark],[System.Int64, System.Private.CoreLib]].get_Item(RecordStruct)
       push      rsi
       sub       rsp,30
       vzeroupper
       mov       rsi,rdx
       vmovdqu   xmm0,xmmword ptr [rsi]
       vmovdqu   xmmword ptr [rsp+20],xmm0
       lea       rdx,[rsp+20]
       call      System.Collections.Generic.Dictionary`2[[RecordStruct, RecordStructBenchmark],[System.Int64, System.Private.CoreLib]].FindValue(RecordStruct)
       test      rax,rax
       je        short M01_L00
       mov       rax,[rax]
       add       rsp,30
       pop       rsi
       ret
M01_L00:
       vmovdqu   xmm0,xmmword ptr [rsi]
       vmovdqu   xmmword ptr [rsp+20],xmm0
       lea       rcx,[rsp+20]
       call      System.ThrowHelper.ThrowKeyNotFoundException[[RecordStruct, RecordStructBenchmark]](RecordStruct)
       int       3
; Total bytes of code 66
```

## .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```assembly
; DictionaryBench.HashEquatableRecordStruct_DictionaryGet()
       sub       rsp,38
       vzeroupper
       mov       rdx,[rcx+0A8]
       vmovdqu   xmm0,xmmword ptr [rcx+68]
       vmovdqu   xmmword ptr [rsp+28],xmm0
       mov       rcx,rdx
       lea       rdx,[rsp+28]
       cmp       [rcx],ecx
       call      qword ptr [7FFF1E3A1AC8]
       nop
       add       rsp,38
       ret
; Total bytes of code 47
```
```assembly
; System.Collections.Generic.Dictionary`2[[HashEquatableRecordStruct, RecordStructBenchmark],[System.Int64, System.Private.CoreLib]].get_Item(HashEquatableRecordStruct)
       push      rsi
       sub       rsp,30
       vzeroupper
       mov       rsi,rdx
       vmovdqu   xmm0,xmmword ptr [rsi]
       vmovdqu   xmmword ptr [rsp+20],xmm0
       lea       rdx,[rsp+20]
       call      System.Collections.Generic.Dictionary`2[[HashEquatableRecordStruct, RecordStructBenchmark],[System.Int64, System.Private.CoreLib]].FindValue(HashEquatableRecordStruct)
       test      rax,rax
       je        short M01_L00
       mov       rax,[rax]
       add       rsp,30
       pop       rsi
       ret
M01_L00:
       vmovdqu   xmm0,xmmword ptr [rsi]
       vmovdqu   xmmword ptr [rsp+20],xmm0
       lea       rcx,[rsp+20]
       call      System.ThrowHelper.ThrowKeyNotFoundException[[HashEquatableRecordStruct, RecordStructBenchmark]](HashEquatableRecordStruct)
       int       3
; Total bytes of code 66
```

