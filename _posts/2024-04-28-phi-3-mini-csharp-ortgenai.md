---
layout: post
title: Phi-3-mini in <30 lines of C# with ONNX Runtime GenAI
---
As part of the Phi-3 launch ([Introducing Phi-3: Redefining what’s possible with
SLMs](https://azure.microsoft.com/en-us/blog/introducing-phi-3-redefining-whats-possible-with-slms/))
Microsoft has released optimized ONNX models as detailed in [ONNX Runtime
supports Phi-3 mini models across platforms and
devices](https://onnxruntime.ai/blogs/accelerating-phi-3) and published the
models on HuggingFace 🤗 at [Phi-3 Mini-4K-Instruct ONNX
models](https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx) for
consumption in for example the [ONNX Runtime
GenAI](https://github.com/microsoft/onnxruntime-genai). This makes it very easy
to run this model locally in just a few lines of C# as I'll show in this blog
post. 

## Prerequisites
 * Install [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).
 * Clone ONNX model repo (use git lfs)
   ```
   git lfs install
   git clone https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx
   ```
 * Checkout the model files you would like to use or all with:
   ```
   git lfs checkout
   ```
 * This repo has the following directories:
   ```
   ├───cpu_and_mobile
   │   ├───cpu-int4-rtn-block-32
   │   └───cpu-int4-rtn-block-32-acc-level-4
   ├───cuda
   │   ├───cuda-fp16
   │   └───cuda-int4-rtn-block-32
   └───directml
       └───directml-int4-awq-block-128
   ```
   Any of these directories can be used with ONNX Runtime GenAI. It does also
   mean there is a large ~2GB `onnx.data` weights file for each of these, and
   that you need to have each of these present if you want to run either of them
   depending on available hardware.
 * CUDA is required for the `cuda` models, which I'll use here, and you need to
   have a compatible GPU and drivers installed.

## Code
Create a new console app with `dotnet new console -n OnnxRuntimeGenAiDemo` and change to:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.ML.OnnxRuntimeGenAI.Cuda" Version="0.2.0-rc4" />
  </ItemGroup>
</Project>
```
Then change `Program.cs` to:
```csharp
using Microsoft.ML.OnnxRuntimeGenAI;
var modelDirectory = args.Length == 1 ? args[0] :
    @"C:\git\oss\Phi-3-mini-4k-instruct-onnx\cuda\cuda-int4-rtn-block-32";
using var model = new Model(modelDirectory);
using var tokenizer = new Tokenizer(model);
while (true)
{
    Console.Write("Prompt: ");
    var line = Console.ReadLine();
    if (line == null) { continue; }

    using var tokens = tokenizer.Encode(line);

    using var generatorParams = new GeneratorParams(model);
    generatorParams.SetSearchOption("max_length", 2048);
    generatorParams.SetInputSequences(tokens);

    using var generator = new Generator(model, generatorParams);

    while (!generator.IsDone())
    {
        generator.ComputeLogits();
        generator.GenerateNextToken();
        var newTokens = generator.GetSequence(0);
        var output = tokenizer.Decode(newTokens.Slice(newTokens.Length - 1, 1));
        Console.Write(output);
    }
    Console.WriteLine();
}
```
And you are good to go and can run it with `dotnet run
.\OnnxRuntimeGenAiDemo.csproj`, which may output something like:
```
[0;93m2024-04-28 09:30:55.0208088 [W:onnxruntime:onnxruntime-genai, session_state.cc:1166 onnxruntime::VerifyEachNodeIsAssignedToAnEp] Some nodes were not assigned to the preferred execution providers which may or may not have an negative impact on performance. e.g. ORT explicitly assigns shape related ops to CPU to improve perf.[m
[0;93m2024-04-28 09:30:55.0262077 [W:onnxruntime:onnxruntime-genai, session_state.cc:1168 onnxruntime::VerifyEachNodeIsAssignedToAnEp] Rerunning with verbose output on a non-minimal build will show node assignments.[m
Prompt: What is the capital of Denmark?

 The capital of Denmark is Copenhagen.

It is the largest city in Denmark and is located on the eastern coast of the
island of Zealand. Copenhagen is not only the political and cultural center of
Denmark but also a major economic hub. The city is known for its beautiful
architecture, historic sites, and vibrant cultural scene. The Copenhagen Opera
House, Tivoli Gardens, and the Nyhavn canal area are just a few of the
attractions that draw millions of visitors each year.
```
That's all!

The warnings are from ONNX Runtime and it is not unusual to see such depending
on the ONNX model. If you want to use CPU or DirectML you have to switch both
nuget package and adjust code accordingly. I could not get the DirectML working
in a quick trial of that. 

Generally ONNX runtime and GenAI packaging and modularization of these is
lacking, despite the architecture and design of ONNX runtime being highly
modular and extensible. Ideally you should be able to pick and choose one or
multiple execution providers and run on whatever provider is available. This is
what we do for our ML computer vision needs but it has required us to custom
built and custom package native packages for the ONNX Runtime. This is part of
the `NtvLibs` packages I have published on nuget and for which I created a .NET
tool to create. Something I have yet to detail in a blog post. If I'll ever find
the time.

It's fairly easy to get Phi-3-mini to output garbage or loop forever. Some of
this can be controlled via various parameters (via
`generatorParams.SetSearchOption`) of which some are detailed in the [Python
example
code](https://github.com/microsoft/onnxruntime-genai/blob/main/examples/python/model-qa.py);