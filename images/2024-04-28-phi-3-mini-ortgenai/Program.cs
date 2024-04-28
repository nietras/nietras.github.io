using Microsoft.ML.OnnxRuntimeGenAI;
// Python: https://github.com/microsoft/onnxruntime-genai/blob/main/examples/python/model-qa.py
//if __name__ == "__main__":
//    parser = argparse.ArgumentParser(argument_default = argparse.SUPPRESS, description = "End-to-end AI Question/Answer example for gen-ai")
//    parser.add_argument('-m', '--model', type = str, required = True, help = 'Onnx model folder path (must contain config.json and model.onnx)')
//    parser.add_argument('-i', '--min_length', type = int, help = 'Min number of tokens to generate including the prompt')
//    parser.add_argument('-l', '--max_length', type = int, help = 'Max number of tokens to generate including the prompt')
//    parser.add_argument('-ds', '--do_random_sampling', action = 'store_true', help = 'Do random sampling. When false, greedy or beam search are used to generate the output. Defaults to false')
//    parser.add_argument('-p', '--top_p', type = float, help = 'Top p probability to sample with')
//    parser.add_argument('-k', '--top_k', type = int, help = 'Top k tokens to sample from')
//    parser.add_argument('-t', '--temperature', type = float, help = 'Temperature to sample with')
//    parser.add_argument('-r', '--repetition_penalty', type = float, help = 'Repetition penalty to sample with')
//    parser.add_argument('-v', '--verbose', action = 'store_true', default = False, help = 'Print verbose output and timing information. Defaults to false')
//    parser.add_argument('-s', '--system_prompt', type = str, default = '', help = 'Prepend a system prompt to the user input prompt. Defaults to empty')
//    parser.add_argument('-g', '--timings', action = 'store_true', default = False, help = 'Print timing information for each generation step. Defaults to false')
//    args = parser.parse_args()
//    main(args)
//
//    search_options = {name:getattr(args, name) for name in ['do_sample', 'max_length', 'min_length', 'top_p', 'top_k', 'temperature', 'repetition_penalty'] if name in args}

var modelDirectory = args.Length == 1 ? args[0] :
    @"C:\git\oss\Phi-3-mini-4k-instruct-onnx\cuda\cuda-int4-rtn-block-32";

using var model = new Model(modelDirectory);
using var tokenizer = new Tokenizer(model);

while (true)
{
    Console.Write("Prompt: ");
    var line = Console.ReadLine();
    if (line == null) { continue; }

    var tokens = tokenizer.Encode(line);

    using var generatorParams = new GeneratorParams(model);
    generatorParams.SetSearchOption("max_length", 2048);
    generatorParams.SetInputSequences(tokens);
    using var generator = new Generator(model, generatorParams);

    Console.WriteLine("================  Output  ================");
    while (!generator.IsDone())
    {
        generator.ComputeLogits();
        generator.GenerateNextToken();
        var newTokens = generator.GetSequence(0);
        var output = tokenizer.Decode(newTokens.Slice(newTokens.Length - 1, 1));
        Console.Write(output);
    }
    Console.WriteLine();
    Console.WriteLine("==========================================");
}
