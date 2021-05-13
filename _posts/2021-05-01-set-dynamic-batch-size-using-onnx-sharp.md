---
layout: post
title: Set Dynamic Batch Size in ONNX Models using OnnxSharp
---

Continuing from ![Introducing OnnxSharp and 'dotnet onnx']({{ site.baseurl }}/2021/03/20/introducing-onnxsharp/)
in this post I will look at using [OnnxSharp](https://github.com/nietras/OnnxSharp)
to set dynamic batch size in an ONNX model to allow the model to be
used for batch inference using [ONNX Runtime](https://github.com/microsoft/onnxruntime)
in a few steps:

 * Inference: Using [Microsoft.ML.OnnxRuntime](https://www.nuget.org/packages/Microsoft.ML.OnnxRuntime/)
 * Problem: Fixed Batch Size in Models
 * Solution: `SetDim`
 * Result: Batch Inference

## Inference: Using [Microsoft.ML.OnnxRuntime](https://www.nuget.org/packages/Microsoft.ML.OnnxRuntime/)

