using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Onnx;

Action<string> log = Console.WriteLine;
const int BatchSize = 1;
const int Iterations = 7;

var modelPath = "mnist-8.onnx";
using var inference = new InferenceSession(modelPath);

var inputs = inference.InputMetadata.Select(p =>
    NamedOnnxValue.CreateFromTensor(p.Key,
        new DenseTensor<float>(SetBatchSize(p.Value.Dimensions, BatchSize))))
    .ToArray<NamedOnnxValue>();

for (int i = 0; i < Iterations; i++)
{
    var before = Stopwatch.GetTimestamp();

    using var outputs = inference.Run(inputs);

    var after = Stopwatch.GetTimestamp();
    var time_ms = 1000.0 * (after - before) / Stopwatch.Frequency;

    var output = outputs.Single();
    var outputTensor = output.AsTensor<float>();

    log($"N={BatchSize,1} Tb={time_ms:F3}ms Ts={time_ms/BatchSize:F3}ms " +
        $"{string.Join(",", outputTensor.Select(v => $"{v:F3}"))}");
}

static int[] SetBatchSize(int[] dimensions, int batchSize)
{
    dimensions[0] = batchSize;
    return dimensions;
}