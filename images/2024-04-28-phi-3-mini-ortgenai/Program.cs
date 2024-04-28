using Microsoft.ML.OnnxRuntimeGenAI;
var modelDirectory = args.Length == 2 ? args[1] :
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
