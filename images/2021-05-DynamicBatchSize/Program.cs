using System;
using System.Linq;
using Google.Protobuf;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Onnx;

var model = ModelProto.Parser.ParseFromFile("mnist-8.onnx");
const int batchSize = 2;
model.Graph.SetDim();
var modelBytes = model.ToByteArray();

using var inference = new InferenceSession(modelBytes);

var inputs = inference.InputMetadata.Select(p =>
        CreateNamedOnnxValueTensor(p.Key, p.Value, batchSize))
    .ToArray<NamedOnnxValue>();

using var outputs = inference.Run(inputs);

var output = outputs.Single();
var outputTensor = output.AsTensor<float>();
var arrayString = outputTensor.GetArrayString();

Console.WriteLine($"N={batchSize} {arrayString}");

static NamedOnnxValue CreateNamedOnnxValueTensor(
    string name, NodeMetadata node, int batchSize)
{
    var dimensions = node.Dimensions;
    dimensions[0] = batchSize;
    var tensor = new DenseTensor<float>(dimensions);
    return NamedOnnxValue.CreateFromTensor(name, tensor);
}