using System.Text.Json;
using System.Text.Json.Serialization;
using CRE132.Compiler;
using CRE132.Engine;

namespace CRE132.LessonDoc;

// Runs at build time from the web project. Reads content/, writes wwwroot/lessons/*.json and
// wwwroot/samples/*.bin. Any bad reference or non-compiling sample returns 1, which fails the
// build - the alternative is a student meeting a 404 or a dead Run button.
static class Program
{
    static async Task<int> Main(string[] args)
    {
        string? lessons = Arg(args, "--lessons"), samplesDir = Arg(args, "--samples"),
                challengesDir = Arg(args, "--challenges"), figuresDir = Arg(args, "--figures"),
                outJson = Arg(args, "--out"), outBin = Arg(args, "--bin");
        if (lessons is null || samplesDir is null || challengesDir is null
            || figuresDir is null || outJson is null || outBin is null)
        {
            Console.Error.WriteLine(
                "usage: LessonDoc --lessons <dir> --samples <dir> --challenges <dir> --figures <dir> --out <dir> --bin <dir>");
            return 1;
        }

        var samples = Directory.Exists(samplesDir)
            ? Directory.GetFiles(samplesDir, "*.cs").ToDictionary(
                  f => Path.GetFileNameWithoutExtension(f),
                  f => SourceText.Normalise(File.ReadAllText(f)).TrimEnd() + "\n")
            : new Dictionary<string, string>();

        var figures = Directory.Exists(figuresDir)
            ? Directory.GetFiles(figuresDir, "*.svg").ToDictionary(
                  f => Path.GetFileNameWithoutExtension(f), File.ReadAllText)
            : new Dictionary<string, string>();

        var challenges = new Dictionary<string, ChallengeFiles>();
        bool failed = false;
        if (Directory.Exists(challengesDir))
            foreach (string starter in Directory.GetFiles(challengesDir, "*.start.cs"))
            {
                string id = Path.GetFileName(starter)[..^".start.cs".Length];
                string solution = Path.Combine(challengesDir, id + ".solution.cs");
                string casesFile = Path.Combine(challengesDir, id + ".cases.json");
                if (!File.Exists(solution) || !File.Exists(casesFile))
                {
                    Console.Error.WriteLine(
                        $"challenge '{id}': needs {id}.solution.cs and {id}.cases.json beside the starter.");
                    failed = true;
                    continue;
                }
                List<ChallengeCase> cases;
                try
                {
                    cases = JsonSerializer.Deserialize<List<ChallengeCase>>(
                        File.ReadAllText(casesFile),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                }
                catch (JsonException e)
                {
                    Console.Error.WriteLine($"challenge '{id}': cases.json is not valid JSON: {e.Message}");
                    failed = true;
                    continue;
                }
                challenges[id] = new ChallengeFiles(
                    SourceText.Normalise(File.ReadAllText(starter)).TrimEnd() + "\n", cases);
            }

        // Compile every sample with the SAME compiler and instrumentation the browser uses, so
        // precompiled and freshly-compiled behaviour cannot diverge. Shipped as .bin: some
        // enterprise proxies block .dll downloads.
        Directory.CreateDirectory(outBin);
        var compiler = new SourceCompiler(new BuildReferenceSource());
        foreach ((string id, string source) in samples)
        {
            if (File.Exists(Path.Combine(samplesDir, id + ".in.txt")))
            {
                Console.Error.WriteLine(
                    $"sample '{id}' has an .in.txt, but the site cannot feed stdin to samples yet — remove it (the input panel arrives in Phase 3).");
                failed = true;
            }
            CompiledBytes compiled = await compiler.CompileToBytesAsync(source);
            if (!compiled.Succeeded)
            {
                foreach (CompileError e in compiled.Errors)
                    Console.Error.WriteLine($"sample '{id}' line {e.Line}: {e.Message} ({e.Id})");
                failed = true;
                continue;
            }
            File.WriteAllBytes(Path.Combine(outBin, id + ".bin"), compiled.Bytes!);
        }

        Directory.CreateDirectory(outJson);
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        foreach (string path in Directory.GetFiles(lessons, "*.md"))
        {
            string name = Path.GetFileNameWithoutExtension(path);
            string markdown = File.ReadAllText(path);

            if (!NarrationParser.HasDirectives(markdown))
            {
                Console.WriteLine($"  narration: {name}.md has no directives - documentation, skipped");
                continue;
            }

            try
            {
                (IReadOnlyList<Block> blocks, IReadOnlyList<string> errors) =
                    NarrationValidator.Validate(NarrationParser.Parse(markdown), samples, figures, challenges);

                foreach (string e in errors)
                {
                    Console.Error.WriteLine($"{path}: {e}");
                    failed = true;
                }
                if (errors.Count > 0) continue;

                File.WriteAllText(Path.Combine(outJson, name + ".json"),
                                  JsonSerializer.Serialize(blocks, options));
                Console.WriteLine($"  narration: {name}.json ({blocks.Count} blocks)");
            }
            catch (NarrationException e)
            {
                Console.Error.WriteLine($"{path}: {e.Message}");
                failed = true;
            }
        }

        return failed ? 1 : 0;
    }

    static string? Arg(string[] args, string name)
    {
        int i = Array.IndexOf(args, name);
        return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
    }
}

// The same six facades the browser fetches, copied to this tool's output directory by the
// CopyLessonDocReferences target in LessonDoc.csproj.
sealed class BuildReferenceSource : IReferenceSource
{
    public Task<IReadOnlyList<byte[]>> GetAsync()
    {
        string refs = Path.Combine(AppContext.BaseDirectory, "refs");
        IReadOnlyList<byte[]> bytes = Directory.GetFiles(refs, "*.bin")
            .OrderBy(f => f).Select(File.ReadAllBytes).ToList();
        return Task.FromResult(bytes);
    }
}
