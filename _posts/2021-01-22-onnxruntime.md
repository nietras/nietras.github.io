---
layout: post
title: ONNX Runtime - Build with TensorRT, CUDA, DirectML execution providers and benchmark on GeForce RTX 3070 via C#
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

* CUDA
* cuDNN (requires login)
* TensorRT (requires login)

## Modifications
CMake cuda architectures parameters not working ... TODO


## Build
ONNX Runtime is build via CMake files and a `build.bat` script. 
Running `.\build.bat --help` displays build script parameters.
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

## Issues
No more delay loading of CUDA dlls.

## Benchmarks
Quick bench