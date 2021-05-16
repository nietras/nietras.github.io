---
layout: post
title: Set Dynamic Batch Size in ONNX Models using OnnxSharp
---

Continuing from [Introducing OnnxSharp and 'dotnet onnx']({{ site.baseurl }}/2021/03/20/introducing-onnxsharp/)
in this post I will look at using [OnnxSharp](https://github.com/nietras/OnnxSharp)
to set dynamic batch size in an ONNX model to allow the model to be
used for batch inference using [ONNX Runtime](https://github.com/microsoft/onnxruntime)
in a few steps:

 * **Setup**: Inference using [Microsoft.ML.OnnxRuntime](https://www.nuget.org/packages/Microsoft.ML.OnnxRuntime/)
 * **Problem**: Fixed Batch Size in Models
 * **Solution**: [OnnxSharp](https://github.com/nietras/OnnxSharp) `SetDim`
 * **Result**: Batch Inference
 * **How**: Don't Forget Reshapes

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
[Introducing OnnxSharp and 'dotnet onnx']({{ site.baseurl }}/2021/03/20/introducing-onnxsharp/).

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
models are exported from different ML frameworks. Most models are in fact defined
with a dynamic batch size, since that is how they are trained, but when exporting
to ONNX the exporter does not always handle this and instead simply outputs 1.

In the above the input tensor `Input3` shape is given as `1x1x28x28`.
In this case this shape defines `NCHW` where:

 * `N = 1` is the batch size
 * `C = 1` is the number of channels e.g. gray (1) or RGB (3).
 * `H = 28` is the height
 * `W = 28` is the width

To fix this we need to change the `N` from `1` to, well, `N` in fact 
(that is any string) as that is how the ONNX format defines a dimension:
```json
message Dimension {
  oneof value {
    int64 dim_value = 1;
    string dim_param = 2;
  };
};
```
as can be seen in the ONNX protobuf schema 
[onnx.proto3](https://github.com/onnx/onnx/blob/master/onnx/onnx.proto3).
That is, a dimension can be either a 64-bit signed integer or a string.
When it is a string it is considered "dynamic" and the same string e.g. `N`
can be used to indicate the same dimension flowing through the graph from inputs
to outputs.

So how can we change this?

## Solution: [OnnxSharp](https://github.com/nietras/OnnxSharp) `SetDim`
With [OnnxSharp](https://github.com/nietras/OnnxSharp) it is very easy to
change the fixed leading dimension using `SetDim` as shown below:
```csharp
using System;
using System.Linq;
using Google.Protobuf;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Onnx;

var model = ModelProto.Parser.ParseFromFile("mnist-8.onnx");
model.Graph.SetDim();
var modelBytes = model.ToByteArray();

using var inference = new InferenceSession(modelBytes);

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
which simply loads the ONNX model first via `OnnxSharp` calls `SetDim()` on
the graph which defaults to changing the leading dimension to `N`. This also
has an overload allowing for customization e.g.:
```csharp
public static void SetDim(this GraphProto graph, 
    int dimIndex, DimParamOrValue dimParamOrValue);
```
After changing the dimension the model can either be converted to 
a byte array or saved to file. In this case, I'm simply converting to
a byte array which we can forward directory the `InferenceSession` 
constructor.

You can also use the `dotnet onnx` tool to do the same if you prefer that with:
```powershell
dotnet onnx setdim mnist-8.onnx mnist-8-setdim.onnx
```

## Result: Batch Inference
Running this code will then output:
```
N=2 {
    {-0.044856027,0.007791661,0.06810082,0.02999374,-0.12640963,0.14021875,-0.055284902,-0.049383815,0.08432205,-0.054540414},
    {-0.044856027,0.007791661,0.06810082,0.02999374,-0.12640963,0.14021875,-0.055284902,-0.049383815,0.08432205,-0.054540414}
}
```
Success! We can also change the batch size to `8`:
```
N=8 {
    {-0.044856027,0.007791661,0.06810082,0.02999374,-0.12640963,0.14021875,-0.055284902,-0.049383815,0.08432205,-0.054540414},
    {-0.044856027,0.007791661,0.06810082,0.02999374,-0.12640963,0.14021875,-0.055284902,-0.049383815,0.08432205,-0.054540414},
    {-0.044856027,0.007791661,0.06810082,0.02999374,-0.12640963,0.14021875,-0.055284902,-0.049383815,0.08432205,-0.054540414},
    {-0.044856027,0.007791661,0.06810082,0.02999374,-0.12640963,0.14021875,-0.055284902,-0.049383815,0.08432205,-0.054540414},
    {-0.044856027,0.007791661,0.06810082,0.02999374,-0.12640963,0.14021875,-0.055284902,-0.049383815,0.08432205,-0.054540414},
    {-0.044856027,0.007791661,0.06810082,0.02999374,-0.12640963,0.14021875,-0.055284902,-0.049383815,0.08432205,-0.054540414},
    {-0.044856027,0.007791661,0.06810082,0.02999374,-0.12640963,0.14021875,-0.055284902,-0.049383815,0.08432205,-0.054540414},
    {-0.044856027,0.007791661,0.06810082,0.02999374,-0.12640963,0.14021875,-0.055284902,-0.049383815,0.08432205,-0.054540414}
}
```

## How: Don't Forget Reshapes

![mnist-8 fixed reshape leading dimension 1]({{ site.baseurl }}/images/2021-05-DynamicBatchSize/set-dynamic-batch-size-reshape-before.png)

![mnist-8 dynamic reshape leading dimension -1]({{ site.baseurl }}/images/2021-05-DynamicBatchSize/set-dynamic-batch-size-reshape-after.png)
