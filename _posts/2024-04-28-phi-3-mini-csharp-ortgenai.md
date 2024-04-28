---
layout: post
title: Phi-3-mini in X lines of C# with ONNX Runtime GenAI
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
post. To try it out all you need is [.NET 8
SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

