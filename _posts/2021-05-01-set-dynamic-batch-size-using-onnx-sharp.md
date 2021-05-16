---
layout: post
title: Set Dynamic Batch Size in ONNX Models using OnnxSharp
---

Continuing from ![Introducing OnnxSharp and 'dotnet onnx']({{ site.baseurl }}/2021/03/20/introducing-onnxsharp/)
in this post I will look at using [OnnxSharp](https://github.com/nietras/OnnxSharp)
to set dynamic batch size in an ONNX model to allow the model to be
used for batch inference using [ONNX Runtime](https://github.com/microsoft/onnxruntime)
in a few steps:

 * Setup: Inference using [Microsoft.ML.OnnxRuntime](https://www.nuget.org/packages/Microsoft.ML.OnnxRuntime/)
 * Problem: Fixed Batch Size in Models
 * Solution: OnnxSharp `SetDim`
 * Result: Batch Inference

## Setup: Inference using [Microsoft.ML.OnnxRuntime](https://www.nuget.org/packages/Microsoft.ML.OnnxRuntime/)
To run inference using the ONNX Runtime I've create a small C# console project with
the following files:
```
DynamicBatchSize.sln
DynamicBatchSize.csproj
Program.cs
mnist-8.onnx
```
where `mnist-8-onnx` is the same model as discussed in the previous blog post
![Introducing OnnxSharp and 'dotnet onnx']({{ site.baseurl }}/2021/03/20/introducing-onnxsharp/).

The project file `DymamicBatchSize.csproj` contains:
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net5.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.ML.OnnxRuntime" Version="1.7.0" />
    <PackageReference Include="OnnxSharp" Version="0.2.0" />
  </ItemGroup>

  <ItemGroup>
    <None Update="mnist-8.onnx">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>

</Project>
```

and `Program.cs` in the new C# 9 [top-level program](https://devblogs.microsoft.com/dotnet/c-9-0-on-the-record/#top-level-programs)
style contains:
```csharp
using System;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

using var inference = new InferenceSession("mnist-8.onnx");

var inputs = inference.InputMetadata.Select(p =>
    NamedOnnxValue.CreateFromTensor(p.Key,
        new DenseTensor<float>(p.Value.Dimensions)))
    .ToArray<NamedOnnxValue>();

using var outputs = inference.Run(inputs);

var output = outputs.Single();
var outputTensor = output.AsTensor<float>();
var arrayString = outputTensor.GetArrayString();

Console.WriteLine(arrayString);
```
which when run outputs:
```
{
    {-0.044856027,0.007791661,0.06810082,0.02999374,-0.12640963,0.14021875,-0.055284902,-0.049383815,0.08432205,-0.054540414}
}
```
as you may have noticed this will be the output for a zero tensor of batch size 1 e.g.
a black image. Recalling the input and outputs are defined as:

|Name  |Type      |ElemType|Shape    |SizeInFile|
|:-----|:---------|:-------|--------:|---------:|
|Input3|TensorType|Float   |1x1x28x28|        32|


|Name            |Type      |ElemType|Shape|SizeInFile|
|:---------------|:---------|:-------|----:|---------:|
|Plus214_Output_0|TensorType|Float   | 1x10|        34|

## Problem: Fixed Batch Size in Models
To run with a different batch size we can change the code to the below.
As can be seen we are simply changing the leading dimension of the
input tensor from 1 to `batchSize`.
```csharp
using System;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

using var inference = new InferenceSession("mnist-8.onnx");

const int batchSize = 2;

var inputs = inference.InputMetadata.Select(p =>
    NamedOnnxValue.CreateFromTensor(p.Key,
        new DenseTensor<float>(SetBatchSize(p.Value.Dimensions, batchSize))))
    .ToArray<NamedOnnxValue>();

using var outputs = inference.Run(inputs);

var output = outputs.Single();
var outputTensor = output.AsTensor<float>();
var arrayString = outputTensor.GetArrayString();

Console.WriteLine($"N={batchSize} {arrayString}");

static int[] SetBatchSize(int[] dimensions, int batchSize)
{
    dimensions[0] = batchSize;
    return dimensions;
}
```
However, when running this will throw an exception.
```
Unhandled exception. Microsoft.ML.OnnxRuntime.OnnxRuntimeException: [ErrorCode:InvalidArgument] Got invalid dimensions for input: Input3 for the following indices
 index: 0 Got: 2 Expected: 1
 Please fix either the inputs or the model.
   at Microsoft.ML.OnnxRuntime.NativeApiStatus.VerifySuccess(IntPtr nativeStatus)
   at Microsoft.ML.OnnxRuntime.InferenceSession.RunImpl(RunOptions options, IntPtr[] inputNames, IntPtr[] inputValues, IntPtr[] outputNames, DisposableList`1 cleanupList)
   at Microsoft.ML.OnnxRuntime.InferenceSession.Run(IReadOnlyCollection`1 inputs, IReadOnlyCollection`1 outputNames, RunOptions options)
   at Microsoft.ML.OnnxRuntime.InferenceSession.Run(IReadOnlyCollection`1 inputs, IReadOnlyCollection`1 outputNames)
   at Microsoft.ML.OnnxRuntime.InferenceSession.Run(IReadOnlyCollection`1 inputs)
   at <Program>$.<Main>$(String[] args) in Program.cs:line 15
```
which has a clear message `index: 0 Got: 2 Expected: 1` indicating the model
appears to be expecting the leading dimension or batch size to be 1.

This is a common problem with ONNX models, and can be an artifact of how ONNX
models are exporting from different ML frameworks. Most models are in fact defined
with a dynamic batch size, since that is how they are trained, but when exporting
to ONNX the exporter does not always handle this and instead simply outputs 1.



