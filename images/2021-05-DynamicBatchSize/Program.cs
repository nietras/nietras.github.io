#define SET_DYNAMIC_BATCH_SIZE
using System;
using System.Diagnostics;
using System.Linq;
using Google.Protobuf;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Onnx;

var modelPath = "mnist-8.onnx";

#if SET_DYNAMIC_BATCH_SIZE
var model = ModelProto.Parser.ParseFromFile(modelPath);
model.Graph.SetDim();
var modelBytes = model.ToByteArray();

using var inference = new InferenceSession(modelBytes);
#else
using var inference = new InferenceSession(modelPath);
#endif

foreach (var batchSize in new[] { 1, 2, 4, 8 })
{
    var inputs = inference.InputMetadata.Select(p =>
        NamedOnnxValue.CreateFromTensor(p.Key,
            new DenseTensor<float>(SetBatchSize(p.Value.Dimensions, batchSize))))
        .ToArray<NamedOnnxValue>();

    const int iterations = 10;
    const int skipFirst = 3;
    var sum_ms = 0.0;
    for (int i = 0; i < iterations; i++)
    {
        var before = Stopwatch.GetTimestamp();

        using var outputs = inference.Run(inputs);

        if (i >= skipFirst)
        {
            var after = Stopwatch.GetTimestamp();
            var time_ms = 1000.0 * (after - before) / Stopwatch.Frequency;
            sum_ms += time_ms;
            if (i == iterations - 1)
            {
                var output = outputs.Single();
                var outputTensor = output.AsTensor<float>();
                var arrayString = outputTensor.GetArrayString();
                var mean_ms = sum_ms / (iterations - skipFirst);
                Console.WriteLine($"N={batchSize} T={time_ms:F3}ms {arrayString}");
            }
        }
    }
}

static int[] SetBatchSize(int[] dimensions, int batchSize)
{
    dimensions[0] = batchSize;
    return dimensions;
}