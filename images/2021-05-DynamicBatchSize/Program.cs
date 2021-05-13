using System;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Onnx;

var modelPath = "mnist-8.onnx";
var inference = new InferenceSession(modelPath);

var inputs = inference.InputMetadata.Select(p => 
    NamedOnnxValue.CreateFromTensor(p.Key, new DenseTensor<float>(p.Value.Dimensions)))
    .ToArray();

using var outputs = inference.Run(inputs);

Console.WriteLine(string.Join(",", outputs.Select(o => $"{o.Name} {o.AsTensor<float>().Rank}")));
