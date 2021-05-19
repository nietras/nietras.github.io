using System;
using System.Linq;
using Google.Protobuf;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Onnx;

var model = ModelProto.Parser.ParseFromFile("mnist-8.onnx"); //"squeezenet1.0-9.onnx");//"efficientnet-lite4-11.onnx"); /
const int batchSize = 4;
model.Graph.SetDim(dimIndex: 0, DimParamOrValue.New(4));
model.WriteToFile("mnist-8-fixed-4-batchsize-onnx");
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

//#define SET_DYNAMIC_BATCH_SIZE
//using System;
//using System.Diagnostics;
//using System.Linq;
//using Google.Protobuf;
//using Microsoft.ML.OnnxRuntime;
//using Microsoft.ML.OnnxRuntime.Tensors;
//using Onnx;

//var modelPath = "mnist-8.onnx";

//#if SET_DYNAMIC_BATCH_SIZE
//var model = ModelProto.Parser.ParseFromFile(modelPath);
//model.Graph.SetDim();
//var modelBytes = model.ToByteArray();

//using var inference = new InferenceSession(modelBytes);
//#else
//using var inference = new InferenceSession(modelPath);
//#endif

//const int batchSize = 1;

//var inputs = inference.InputMetadata.Select(p =>
//    NamedOnnxValue.CreateFromTensor(p.Key,
//        new DenseTensor<float>(SetBatchSize(p.Value.Dimensions, batchSize))))
//    .ToArray<NamedOnnxValue>();

//using var outputs = inference.Run(inputs);

//var output = outputs.Single();
//var outputTensor = output.AsTensor<float>();
//var arrayString = outputTensor.GetArrayString();

//Console.WriteLine($"N={batchSize} {arrayString}");

//static int[] SetBatchSize(int[] dimensions, int batchSize)
//{
//    dimensions[0] = batchSize;
//    return dimensions;
//}