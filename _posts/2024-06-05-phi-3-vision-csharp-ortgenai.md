---
layout: post
title: Phi-3-vision in 50 lines of C# with ONNX Runtime GenAI
---
Previously in [Phi-3-mini in 30 lines of C# with ONNX Runtime GenAI]({{
site.baseurl }}/2024/04/28/phi-3-mini-csharp-ortgenai/) I showed how easy it
was to run the [Phi-3-mini
model](https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx) locally in
just a few lines of C#. 

In this blog post I'll show how to run the [Phi-3-vision
model](https://huggingface.co/microsoft/Phi-3-vision-128k-instruct-onnx-cuda),
which is a multimodal model that supports text + image inputs, with .NET in a
similar fashion with version 0.3.0-rc2 of [ONNX Runtime
GenAI](https://github.com/microsoft/onnxruntime-genai) based on the [Phi-3
vision tutorial](https://onnxruntime.ai/docs/genai/tutorials/phi3-v.html) and
[phi3v.py](https://github.com/microsoft/onnxruntime-genai/blob/main/examples/python/phi3v.py).

## TLDR

![TLDR]({{ site.baseurl }}/images/2024-06-phi-3-vision-ortgenai/phi-3-vision-ortgenai.png)

## Prerequisites
 * Install [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).
 * Clone ONNX model repo (use git lfs)
   ```
   git lfs install
   git clone https://huggingface.co/microsoft/Phi-3-vision-128k-instruct-onnx-cuda
   ```
   Note how the repo dictates execution provider (here CUDA). CPU can be found
   in repo
   [Phi-3-vision-128k-instruct-onnx-cpu](https://huggingface.co/microsoft/Phi-3-vision-128k-instruct-onnx-cpu).
 * Checkout the model files you would like to use or all with:
   ```
   git lfs checkout
   ```
 * This repo has the following directories:
   ```
   ├───cuda-fp16
   └───cuda-int4-rtn-block-32
   ```
   Any of these directories can be used with ONNX Runtime GenAI. It does also
   mean there are large +2GB `onnx.data` weights file for each of these, and
   that you need to have each of these present if you want to run either of them
   depending on available hardware. The `cuda-int4-rtn-block-32` is used here
   since I only have a RTX 3070 GPU with 8 GB VRAM.
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
    <PackageReference Include="Microsoft.ML.OnnxRuntimeGenAI.Cuda" 
                      Version="0.3.0-rc2" />
  </ItemGroup>
</Project>
```
Then change `Program.cs` to:
```csharp
using Microsoft.ML.OnnxRuntimeGenAI;

var modelDirectory = args.Length == 2 ? args[1] :
    @"C:\git\oss\Phi-3-vision-128k-instruct-onnx-cuda\" +
    @"cuda-int4-rtn-block-32";

using var model = new Model(modelDirectory);
using var processor = new MultiModalProcessor(model);
using var tokenizerStream = processor.CreateStream();

while (true)
{
    Console.Write("Image Path (leave empty if no image): ");
    var imagePath = Console.ReadLine();
    var hasImage = !string.IsNullOrWhiteSpace(imagePath);

    Console.WriteLine(hasImage ? "Loading image..." : "No image");
    Images? images = hasImage ? Images.Load(imagePath) : null;

    Console.Write("Prompt: ");
    var text = Console.ReadLine();
    if (text == null) { continue; }

    var prompt = "<|user|>\n";
    prompt += hasImage ? "<|image_1|>\n" : "";
    prompt += text + "<|end|>\n<|assistant|>\n";

    Console.WriteLine($"Processing...");
    using var inputs = processor.ProcessImages(prompt, images);

    Console.WriteLine($"Generating response...");
    using var generatorParams = new GeneratorParams(model);
    generatorParams.SetInputs(inputs);
    generatorParams.SetSearchOption("max_length", 3072);
    using var generator = new Generator(model, generatorParams);

    Console.WriteLine("================  Output  ================");
    while (!generator.IsDone())
    {
        generator.ComputeLogits();
        generator.GenerateNextToken();
        var newTokens = generator.GetSequence(0);
        var output = tokenizerStream.Decode(newTokens[^1]);
        Console.Write(output);
    }
    Console.WriteLine();
    Console.WriteLine("==========================================");

    images?.Dispose();
}
```
And you are good to go and can run it with `dotnet run
.\OnnxRuntimeGenAiDemo.csproj`, which may output something like:
```
Image Path (leave empty if no image): table.png
Loading image...
Prompt: Convert the image to markdown
Processing...
Generating response...
================  Output  ================
```markdown
| Product             | Qtr 1    | Qtr 2    | Grand Total |
|---------------------|----------|----------|-------------|
| Chocolade           | $744.60  | $162.56  | $907.16     |
| Gummibarchen        | $5,079.60| $1,249.20| $6,328.80   |
| Scottish Longbreads | $1,267.50| $1,062.50| $2,330.00   |
| Sir Rodney's Scones | $1,418.00| $756.00  | $2,174.00   |
| Tarte au sucre      | $4,728.00| $4,547.92| $9,275.92   |
| Chocolate Biscuits  | $943.89  | $349.60  | $1,293.49   |
| Total               | $14,181.59| $8,127.78| $22,309.37  |

The table lists various products along with their sales figures 
for Qtr 1, Qtr 2, and the Grand Total. The products include 
Chocolade, Gummibarchen, Scottish Longbreads, Sir Rodney's Scones, 
Tarte au sucre, and Chocolate Biscuits. The Grand Total column 
sums up the sales for each product across the two quarters.
```
for `table.png`:

![table.png]({{ site.baseurl }}/images/2024-06-phi-3-vision-ortgenai/table.png)

or for `coffee.png`:

![coffee.png]({{ site.baseurl }}/images/2024-06-phi-3-vision-ortgenai/coffee.png)

output will be:
```
Image Path (leave empty if no image): coffee.png
Loading image...
Prompt: Describe the image
Processing...
Generating response...
================  Output  ================
The image shows a cup of coffee with a latte art design on top. The 
coffee is a light brown color, and the art is white with a leaf-like 
pattern. The cup is white and appears to be made of ceramic.
==========================================
```

That's all!

Full solution and project can be downloaded as 
[OnnxRuntimeGenAiDemo.zip]({{ site.baseurl }}/images/2024-06-phi-3-vision-ortgenai/OnnxRuntimeGenAiDemo.zip)
