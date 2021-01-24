---
layout: post
title: Building ONNX Runtime with TensorRT, CUDA, DirectML execution providers and quick benchmarks on GeForce RTX 3070 via C#
---
In this blog post I detail how to build [ONNX Runtime](https://github.com/microsoft/onnxruntime)
on Windows (10 64-bit). Most importantly with support for the NVidia TensorRT library,
which can give significant performance improvements compared to vanilla CUDA/cuDNN.

## Pre-requisites
* Windows 10 64-bit
* Visual Studio 2019 16.8 or later with **Desktop development with C++** workload installed
* PowerShell or similar with git and VS2019 integration

## Clone
```powershell
D:\oss>git clone https://github.com/microsoft/onnxruntime.git
```
or in my case:
```powershell
D:\oss>git clone https://github.com/nietras/onnxruntime.git
```
since I am building from my own fork of the project with some slight modifications.

## Execution Providers
See [Execution Providers](https://github.com/microsoft/onnxruntime/tree/master/docs/execution_providers)
for more details on this concept.

Some execution providers are linked statically into `onnxruntime.dll` while others
are separate dlls. 

Hopefully, the modular approach with separate dlls for each execution
provider prevails and nuget packages will be similarly modular, so it should no longer
be required to build ONNX Runtime yourself to get the execution providers you want.

* NVidia [CUDA](https://developer.nvidia.com/cuda-toolkit)/[cuDNN](https://developer.nvidia.com/CUDNN)
* NVidia [TensorRT](https://developer.nvidia.com/TensorRT) (depends on CUDA/cuDNN)
* Microsoft DirectML - [https://github.com/microsoft/DirectML](https://github.com/microsoft/DirectML)
* Intel DNNL (now called oneDNN) - [https://github.com/oneapi-src/oneDNN](https://github.com/oneapi-src/oneDNN)

Some of these are supported out-of-the-box via git sub-modules, while NVidia providers
require downloading and installing these libraries separately. 
This is covered briefly next.

## Download NVidia Libraries
TODO

* CUDA - 11.2.0
* cuDNN (requires login) - 8.0.5.39 for CUDA 11.1 (works on 11.2 too)
* TensorRT (requires login) - 7.2.2.3 for CUDA 11.1 (works on 11.2 too)


## Modifications
CMake cuda architectures parameters not working ... TODO


## Build
ONNX Runtime is build via CMake files and a `build.bat` script. 
Running `.\build.bat --help` displays build script parameters.
Building is also covered in [Building ONNX Runtime](https://github.com/microsoft/onnxruntime/blob/master/BUILD.md)
and documentation is generally very nice and worth a read.

Below is the parameters I used to build the ONNX Runtime with support
for the execution providers mentioned above.

```powershell
D:\oss\onnxruntime>./build.bat --config RelWithDebInfo `
  --build_shared_lib --build_csharp --parallel `
  --use_cuda --cuda_version 11.2 --cuda_home "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v11.2" `
  --cudnn_home "C:\git\nvidia\cudnn-11.1-windows-x64-v8.0.5.39-cuda-11.1\cuda" `
  --use_tensorrt --tensorrt_home "C:\git\nvidia\TensorRT-7.2.2.3.Windows10.x86_64.cuda-11.1.cudnn8.0\TensorRT-7.2.2.3" `
  --use_dnnl --use_dml --cmake_generator "Visual Studio 16 2019" --skip_tests
```

Building takes a while... depending on your dev machine of course. 
Enough for a lunch break on mine.

During build you can verify that CUDA code is compiled as specified above.
As for example can be seen below.
```
C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v11.2\bin\nvcc.exe" 
  -gencode=arch=compute_52,code=\"sm_52,compute_52\" 
  -gencode=arch=compute_61,code=\"sm_61,compute_61\" 
  -gencode=arch=compute_70,code=\"sm_70,compute_70\" 
  -gencode=arch=compute_75,code=\"sm_75,compute_75\" 
  -gencode=arch=compute_86,code=\"sm_86,compute_86\" 
  -gencode=arch=compute_86,code=\"compute_86,compute_86\" 
  (...full parameters omitted for brevity...)
  "D:\oss\onnxruntime\onnxruntime\core\providers\cuda\math\cumsum_impl.cu"
```
Once this completes you'll get something like:
```
[TIMESTAMP] build [INFO] - Build complete
```
It's common that there are a ton of warnings during the build. 
Ignore them like most C++ devs do apparently.

## Output
The build output can then be found in:
```
D:\oss\onnxruntime\build\Windows\RelWithDebInfo\RelWithDebInfo
```
Hence, using:
```
gci D:\oss\onnxruntime\build\Windows\RelWithDebInfo\RelWithDebInfo\*.dll `
 | Format-Table -Property Length,Name
```
The usable dlls are:
```
   Length Name
   ------ ----
    22016 custom_op_library.dll
  1299328 DirectML.Debug.dll
 13410184 DirectML.dll
 30925824 dnnl.dll
154482688 onnxruntime.dll
   352768 onnxruntime_providers_dnnl.dll
     9728 onnxruntime_providers_shared.dll
  1599488 onnxruntime_providers_tensorrt.dll
```
As can be seen the DNNL and TensorRT providers are available as separate dlls.
Note also that both DNNL and DirectML are compiled as part of ONNX runtime,
with source via git sub-modules. The CUDA/cuDNN dlls need to be retrieved from
their respective locations.

Collected all this into one location to be able to run ONNX runtime without
having to install or setup any environment variable paths or similar means having
something like this next to the executable:
```
   Length Name
   ------ ----
107368448 cublas64_11.dll
173154304 cublasLt64_11.dll
   464896 cudart64_110.dll
   222720 cudnn64_8.dll
146511360 cudnn_adv_infer64_8.dll
 95296512 cudnn_adv_train64_8.dll
705361408 cudnn_cnn_infer64_8.dll
 81943552 cudnn_cnn_train64_8.dll
323019776 cudnn_ops_infer64_8.dll
 37118464 cudnn_ops_train64_8.dll
    22016 custom_op_library.dll
  1299328 DirectML.Debug.dll
 13410184 DirectML.dll
 30925824 dnnl.dll
  4660736 myelin64_1.dll
   315392 nvblas64_11.dll
632996864 nvinfer.dll
 15790592 nvinfer_plugin.dll
  1924608 nvonnxparser.dll
  2469888 nvparsers.dll
  5204992 nvrtc-builtins64_111.dll
  5542912 nvrtc-builtins64_112.dll
 24423424 nvrtc64_111_0.dll
 31984128 nvrtc64_112_0.dll
154482688 onnxruntime.dll
   352768 onnxruntime_providers_dnnl.dll
     9728 onnxruntime_providers_shared.dll
  1599488 onnxruntime_providers_tensorrt.dll
```
Note that the total size of this is a whopping `~2500 MB`. 
cuBLAS, cuDNN and TensorRT (`nvinfer*.dll`) being
huge. If your only running CNNs you can remove the `cudnn_adv*.dl` files.

This is an artefact of how NVidia has decided to distribute and package dlls with
cubin code for multiple SM versions all together in the individual dlls.
A more sensible approach in my view would be to do what Intel has done for years
for Integrated Performance Primitives ([IPP](https://software.intel.com/content/www/us/en/develop/tools/oneapi/components/ipp.html))
and split these into dlls for each SM version e.g. `cublas64_11_sm86.dll`.
And clean up the whole not forwards compatible version naming etc. That's enough 
ranting though. TensorRT is a must for best performance machine learning inference,
it's very good, which we will get to in a moment.

## Issues
One issue is that the `onnxruntime.dll` no longer delay loads the dependent CUDA dlls.
This means you have to have these in your path even if your are only running with
the DirectML execution provider, for the way ONNX runtime is build here.

In earlier versions the dlls where delay loaded and you can pick and choose.
I've filed an issue regarding this and in that issue it was commented that a solution
for this is upcoming. Hopefully, this means all execution providers will be "pluggable"
as separate dlls, fulfilling the true potential of the ONNX runtime. 
The issue can be found at:

[https://github.com/microsoft/onnxruntime/issues/6350](https://github.com/microsoft/onnxruntime/issues/6350)

## Benchmarks
Quick bench

```
Selected Device: GeForce RTX 3070
Compute Capability: 8.6
SMs: 46
Compute Clock Rate: 1.83 GHz
Device Global Memory: 8192 MiB
Shared Memory per SM: 100 KiB
Memory Bus Width: 256 bits (ECC disabled)
Memory Clock Rate: 7.001 GHz
```

TODO FORMAT
```
===== 'resnet152-v2-7.onnx' with execution provider 'Tensorrt' =====
Inputs 'data'
Outputs 'resnetv27_dense0_fwd'
Average time     5.797 ms

===== 'resnet152-v2-7.onnx' with execution provider 'CUDA' =====
Inputs 'data'
Outputs 'resnetv27_dense0_fwd'
Average time     9.052 ms

===== 'resnet152-v2-7.onnx' with execution provider 'DirectML' =====
Inputs 'data'
Outputs 'resnetv27_dense0_fwd'
Average time     7.795 ms

===== 'resnet152-v2-7.onnx' with execution provider 'Dnnl' =====
Inputs 'data'
Outputs 'resnetv27_dense0_fwd'
Average time    36.924 ms

===== 'resnet152-v2-7.onnx' with execution provider 'CPU' =====
Inputs 'data'
Outputs 'resnetv27_dense0_fwd'
Average time    37.012 ms

===== 'resnet152-v2-7.onnx' with execution provider 'None' =====
Inputs 'data'
Outputs 'resnetv27_dense0_fwd'
Average time    37.472 ms
````