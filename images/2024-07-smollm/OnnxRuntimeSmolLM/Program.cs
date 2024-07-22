using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.Tokenizers;

var modelDirectory = args.Length >= 2 ? args[1] :
    @"C:\git\oss\SmolLM-135M-Instruct\";
var modelFileName = args.Length >= 3 ? args[2] :
    @"onnx\model_int8.onnx";

// https://github.com/ggerganov/llama.cpp/pull/8609/files
// https://github.com/ggerganov/llama.cpp/blob/525e78936a5980e15a07727e5cc477ce7ff471f0/src/llama.cpp#L15655
// special_tokens_map.json
//var tokenizer = Tokenizer.CreateLlama(File.OpenRead(modelDirectory + "tokenizer.json"));
//Tokenizer.CreateCodeGen(modelDirectory + "vocab.json", modelDirectory + "merges.txt",
//    modelDirectory + "special_tokens_map.json", modelDirectory + "tokenizer_codegen.cs");
var tokenizer = new Bpe(modelDirectory + "vocab.json", modelDirectory + "merges.txt",
    unknownToken: "<|endoftext|>");

var prompt =
"""
<|im_start|>user
List the steps to bake a chocolate cake from scratch.<|im_end|>

""";

var ids = tokenizer.Encode(prompt, out var _);
Console.WriteLine(string.Join(",", ids.Select(id => id.Id)));

using var session = new InferenceSession(modelDirectory + modelFileName);

Console.WriteLine(string.Join("\n", session.InputMetadata));


