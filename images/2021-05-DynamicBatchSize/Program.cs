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

const int batchSize = 8;

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