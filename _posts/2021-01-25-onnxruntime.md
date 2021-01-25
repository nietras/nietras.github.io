---
layout: post
title: Building ONNX Runtime with TensorRT, CUDA, DirectML execution providers and quick benchmarks on GeForce RTX 3070 via C#
---
I recently got a new [Ampere](https://en.wikipedia.org/wiki/Ampere_(microarchitecture))
based RTX 3070 card. Unfortunately, using an older version of the ONNX runtime on this
was simply not feasible since it would be way too slow to both startup and run, so much for
[forwards compatibility](https://docs.nvidia.com/cuda/ampere-compatibility-guide/index.html)
of PTX and the real practicalities around that.

Unfortunately, that is a common issue with GPUs and binaries for them not being 
practically "forwards compatible". It's not like CPUs, where you can often
run a 20 year old Windows executable on the latest x86 CPU and Windows version 
without issue.

Hence, this blog post details how to build [ONNX Runtime](https://github.com/microsoft/onnxruntime)
on Windows 10 64-bit using Visual Studio 2019 >=16.8 with currently available libraries
supporting Ampere.

This includes support for the NVidia TensorRT library,
which can give significant performance improvements compared to plain CUDA/cuDNN,
as a quick benchmark shows on a RTX 3070 GPU.

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
These are for this build:

* NVidia [CUDA](https://developer.nvidia.com/cuda-toolkit)/[cuDNN](https://developer.nvidia.com/CUDNN)
* NVidia [TensorRT](https://developer.nvidia.com/TensorRT) (depends on CUDA/cuDNN)
* Microsoft DirectML - [https://github.com/microsoft/DirectML](https://github.com/microsoft/DirectML)
* Intel DNNL (now called oneDNN) - [https://github.com/oneapi-src/oneDNN](https://github.com/oneapi-src/oneDNN)

Some of these are supported out-of-the-box via git sub-modules, while NVidia providers
require downloading and installing these libraries separately. 
This is covered briefly next.

## Download NVidia Libraries
The specific NVidia libraries are:

* CUDA - [11.2.0](https://developer.download.nvidia.com/compute/cuda/11.2.0/local_installers/cuda_11.2.0_460.89_win10.exe)
* cuDNN (requires login) - [8.0.5.39 for CUDA 11.1 (works on 11.2 too)](https://developer.nvidia.com/compute/machine-learning/cudnn/secure/8.0.5/11.1_20201106/cudnn-11.1-windows-x64-v8.0.5.39.zip)
* TensorRT (requires login) - [7.2.2.3 for CUDA 11.1 (works on 11.2 too)](https://developer.nvidia.com/compute/machine-learning/tensorrt/secure/7.2.2/zip/TensorRT-7.2.2.3.Windows10.x86_64.cuda-11.1.cudnn8.0.zip)


## Modifications
ONNX runtime uses [CMake](https://cmake.org/) for building. By default for ONNX runtime
this is setup to built NVidia CUDA code for compute capability (SM) versions 
that are server variants  e.g. `sm80`. However, for my use case GPUs are consumer variants.
Additionally, there were many build warnings due to build targeting quite old SM versions,
so I wanted to customize this for my use case.
The wikipedia page on [CUDA](https://en.wikipedia.org/wiki/CUDA) is pretty great and
summarizes the different versions in a table.

CMake does have a [`CMAKE_CUDA_ARCHITECTURES`](https://cmake.org/cmake/help/git-stage/variable/CMAKE_CUDA_ARCHITECTURES.html#variable:CMAKE_CUDA_ARCHITECTURES)
variable that was supposed to allow one
to customize the build without changing the `CMakeLists.txt` file. However, I could not
get this working and many people online seemed to have the same issue.

Instead, I directly changed the lines regarding this in the ONNX runtime `CMakeLists.txt`
file to:

```cmake
  if (NOT CMAKE_CUDA_ARCHITECTURES)
    if(CMAKE_LIBRARY_ARCHITECTURE STREQUAL "aarch64-linux-gnu")
      # Support for Jetson/Tegra ARM devices
      set(CMAKE_CUDA_FLAGS "${CMAKE_CUDA_FLAGS} -gencode=arch=compute_53,code=sm_53") # TX1, Nano
      set(CMAKE_CUDA_FLAGS "${CMAKE_CUDA_FLAGS} -gencode=arch=compute_62,code=sm_62") # TX2
      set(CMAKE_CUDA_FLAGS "${CMAKE_CUDA_FLAGS} -gencode=arch=compute_72,code=sm_72") # AGX Xavier, NX Xavier
    else()
      # the following compute capabilities are removed in CUDA 11 Toolkit
      if (CMAKE_CUDA_COMPILER_VERSION VERSION_LESS 11)
        set(CMAKE_CUDA_FLAGS "${CMAKE_CUDA_FLAGS} -gencode=arch=compute_30,code=sm_30") # K series
        # 37, 50 still work in CUDA 11 but are marked deprecated and will be removed in future CUDA version.
        set(CMAKE_CUDA_FLAGS "${CMAKE_CUDA_FLAGS} -gencode=arch=compute_37,code=sm_37") # K80
        set(CMAKE_CUDA_FLAGS "${CMAKE_CUDA_FLAGS} -gencode=arch=compute_50,code=sm_50") # M series
      endif()

      set(CMAKE_CUDA_FLAGS "${CMAKE_CUDA_FLAGS} -gencode=arch=compute_52,code=sm_52") # M60
      #set(CMAKE_CUDA_FLAGS "${CMAKE_CUDA_FLAGS} -gencode=arch=compute_60,code=sm_60") # P series
      set(CMAKE_CUDA_FLAGS "${CMAKE_CUDA_FLAGS} -gencode=arch=compute_61,code=sm_61") # P series (consumer)
      set(CMAKE_CUDA_FLAGS "${CMAKE_CUDA_FLAGS} -gencode=arch=compute_70,code=sm_70") # V series
      set(CMAKE_CUDA_FLAGS "${CMAKE_CUDA_FLAGS} -gencode=arch=compute_75,code=sm_75") # T series
      if (CMAKE_CUDA_COMPILER_VERSION VERSION_GREATER_EQUAL 11)
        #set(CMAKE_CUDA_FLAGS "${CMAKE_CUDA_FLAGS} -gencode=arch=compute_80,code=sm_80") # A series
        set(CMAKE_CUDA_FLAGS "${CMAKE_CUDA_FLAGS} -gencode=arch=compute_86,code=sm_86") # A series (consumer series 30)
        set(CMAKE_CUDA_FLAGS "${CMAKE_CUDA_FLAGS} -gencode=arch=compute_86,code=compute_86") # A series (consumer series 30) PTX!
      endif()
    endif()
  endif()
```

Meaning compute capabilities 5.2, 6.1, 7.0, 7.5, 8.6 are used. Note this is not
incredible important when using TensorRT, cuDNN etc. since these contain they own code
for all supported compute capabilities. It is only for the set of CUDA code that is
part of ONNX runtime directly.


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
Enough for a lunch break on my PC.

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
Ignore them like most C++ devs do, apparently.


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
Note also that both DNNL and DirectML are compiled as part of ONNX runtime
via git sub-modules. The CUDA/cuDNN dlls need to be retrieved from
their respective locations.

Collecting all this into one location to be able to run ONNX runtime without
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
187880960 cufft64_10.dll
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
Note that the total size of this is a whopping `~2600 MB`. 
cuBLAS, cuDNN and TensorRT (`nvinfer*.dll`) being
huge. If you are only running CNNs you can remove the `cudnn_adv*.dll` files.
Additionally, the `cudnn*train64_8.dll` files can be removed since these
are only for training.

This is an artifact of how NVidia has decided to distribute and package dlls with
cubin code for multiple SM versions all together in the individual dlls.
A more sensible approach, in my view, would be to do what Intel has done for years
for Integrated Performance Primitives ([IPP](https://software.intel.com/content/www/us/en/develop/tools/oneapi/components/ipp.html))
and split these into dlls for each SM version e.g. `cublas64_11_sm86.dll`.
And clean up the whole not forwards compatible version naming etc. That's enough 
ranting though. TensorRT is a must for best performance machine learning inference.
Which I will get to in a moment.

## Issues
One issue is that the `onnxruntime.dll` no longer delay loads the CUDA dll dependencies.
This means you have to have these in your path even if your are only running with
the DirectML execution provider for example. In the way ONNX runtime is build here.

In earlier versions the dlls where delay loaded.
I've filed an issue regarding this and in that issue it was commented that a solution
for this is upcoming. Hopefully, this means all execution providers will be "pluggable"
as separate dlls, fulfilling the true potential of the ONNX runtime. 
The issue can be found at:

[https://github.com/microsoft/onnxruntime/issues/6350](https://github.com/microsoft/onnxruntime/issues/6350)

## Benchmarks
Finally, based on the build I ran a quick set of benchmarks on my developer PC with:
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
I then downloaded an example model 
[resnet152-v1-7.onnx](https://github.com/onnx/models/blob/master/vision/classification/resnet/model/resnet152-v1-7.onnx)
and ran this for each of the execution providers using the C# API.
Results are summarized below. Note that `Dnnl` is pretty much the same
as CPU here since I have no Intel graphics that can accelerate here.

Times are average execution time for **batch size 1**
(cannot run with anything else currently) and based on a couple hundred iterations 
with some warmup before.

|Execution Provider| Time [ms]  |Ratio  |Speedup |
|:-----------------|-----------:|------:|-------:|
|TensorRT          |       5.797|   0.64|    1.56|
|DirectML          |       7.795|   0.86|    1.16|
|CUDA              |       9.052|   1.00|    1.00|
|Dnnl              |      36.924|   4.08|    0.25|
|None              |      37.472|   4.14|    0.24|

As can be seen in this particular case we see a speedup 
of 1.56x using TensorRT over CUDA. It is common to see 2x or more speedups in the
models I've used. DirectML proves to be a pretty good option and importantly this
has only a small dependency. If best performance isn't necessary for you
I would recommend using that if targeting Windows and it is supported.

Hopefully, one day building from source will be unnecessary 
and there will be a modular set of nuget packages from which you can 
pick and chose for each of the execution providers you like.
