---
layout: post
title: ONNX Runtime - Build and benchmark with TensorRT, CUDA, DirectML 
execution providers on GeForce RTX 3070
---
TODO
Windows 64-bit

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
Link to details on that and brief summary

* NVidia CUDA/cuDNN
* NVidia TensorRT (depends on CUDA/cuDNN)
* Microsoft DirectML
* Intel DNNL

Some of these are supported out-of-the-box via git sub-modules, while NVidia providers
require downloading and installing these libraries separately. 
This is covered briefly next.

## Download NVidia Libraries
TODO

* CUDA
* cuDNN
* TensorRT

## Modifications
TODO

## Build
ONNX Runtime is build via CMake files and a `build.bat` script. 
Running `.\build.bat --help` display build script parameters.
Below is the parameters I used to build the ONNX Runtime with support
for the execution providers mentioned above.

```powershell
D:\oss\onnxruntime>./build.bat --config RelWithDebInfo --build_shared_lib --build_csharp --parallel `
  --use_cuda --cuda_version 11.2 --cuda_home "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v11.2" `
  --cudnn_home "C:\git\nvidia\cudnn-11.1-windows-x64-v8.0.5.39-cuda-11.1\cuda" `
  --use_tensorrt --tensorrt_home "C:\git\nvidia\TensorRT-7.2.2.3.Windows10.x86_64.cuda-11.1.cudnn8.0\TensorRT-7.2.2.3" `
  --use_dnnl --use_dml --cmake_generator "Visual Studio 16 2019" --skip_tests
```

## Output

## Issues
No more delay loading of CUDA dlls.

## Benchmarks
Quick bench