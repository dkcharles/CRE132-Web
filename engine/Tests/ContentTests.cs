using CRE132.Compiler;
using CRE132.Engine;
using Xunit;

namespace CRE132.Tests;

// Data-driven gates over content/: every sample matches its golden output, every challenge's
// own reference solution passes its own cases. A challenge whose solution fails its expected
// output fails HERE, at build time - never in front of a student.
public class ContentTests
{
    static string Content(params string[] parts) =>
        Path.Combine(new[] { RepoPaths.Root, "content" }.Concat(parts).ToArray());

    static readonly SourceCompiler Compiler = new(new DiskReferenceSource());

    public static TheoryData<string> SampleIds()
    {
        var data = new TheoryData<string>();
        foreach (string f in Directory.GetFiles(Content("samples"), "*.cs"))
            data.Add(Path.GetFileNameWithoutExtension(f));
        return data;
    }

    public static TheoryData<string> ChallengeIds()
    {
        var data = new TheoryData<string>();
        foreach (string f in Directory.GetFiles(Content("challenges"), "*.solution.cs"))
            data.Add(Path.GetFileName(f)[..^".solution.cs".Length]);
        return data;
    }

    // Set CRE132_UPDATE_GOLDENS=1 to (re)generate .frame.txt and frames.txt from the code
    // itself. They are still asserted afterwards, so a freshly generated golden always passes;
    // the point of committing them is that a human reviews the grid in the diff.
    static bool Update => Environment.GetEnvironmentVariable("CRE132_UPDATE_GOLDENS") == "1";

    static GameScript SampleScript(string id)
    {
        string file = Content("samples", id + ".game.json");
        if (!File.Exists(file)) return new GameScript(60, new[] { 60 });
        var script = System.Text.Json.JsonSerializer.Deserialize<GameScript>(File.ReadAllText(file),
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        return script with { Snapshot = new[] { script.Frames } };
    }

    [Theory]
    [MemberData(nameof(SampleIds))]
    public async Task Every_sample_produces_its_golden_output(string id)
    {
        string source = File.ReadAllText(Content("samples", id + ".cs"));
        string golden = File.ReadAllText(Content("samples", id + ".out.txt"));
        string stdinFile = Content("samples", id + ".in.txt");
        string stdin = File.Exists(stdinFile) ? File.ReadAllText(stdinFile) : "";
        string frameFile = Content("samples", id + ".frame.txt");

        CompiledBytes compiled = await Compiler.CompileToBytesAsync(source);
        Assert.True(compiled.Succeeded, string.Join("\n",
            compiled.Errors.Select(e => $"line {e.Line}: {e.Message}")));

        GameSession session = GameSession.StartHeadless(ProgramLoader.FromBytes(compiled.Bytes!), stdin);
        Assert.Null(session.Result.Error);
        if (!session.IsGame)
        {
            Assert.False(File.Exists(frameFile), $"{id}.frame.txt exists but {id}.cs never calls Game.Run.");
            Assert.Null(OutputComparer.FirstDifference(golden, session.Result.Output));
            return;
        }

        ScriptResult run = ScriptRunner.Run(session, SampleScript(id));
        Assert.Null(run.Error);
        string frame = run.Snapshots[^1].Text;
        if (Update || !File.Exists(frameFile)) File.WriteAllText(frameFile, frame + "\n");
        Assert.True(File.Exists(frameFile), $"{id} is a game sample and needs {id}.frame.txt:\n{frame}");
        Assert.Null(OutputComparer.FirstDifferentRow(File.ReadAllText(frameFile), frame));
        Assert.Null(OutputComparer.FirstDifference(golden, run.Console));
    }

    static IReadOnlyList<ChallengeCase> Cases(string id)
    {
        var errors = new List<string>();
        string frames = Content("challenges", id + ".frames.txt");
        var cases = CRE132.LessonDoc.ChallengeKit.LoadCases(
            Content("challenges", id + ".cases.json"), File.Exists(frames) ? frames : null, errors);
        Assert.True(errors.Count == 0, string.Join("\n", errors));
        Assert.NotEmpty(cases);
        return cases;
    }

    static async Task<byte[]> Compile(string path)
    {
        CompiledBytes compiled = await Compiler.CompileToBytesAsync(File.ReadAllText(path));
        Assert.True(compiled.Succeeded, string.Join("\n",
            compiled.Errors.Select(e => $"{Path.GetFileName(path)} line {e.Line}: {e.Message}")));
        return compiled.Bytes!;
    }

    [Theory]
    [MemberData(nameof(ChallengeIds))]
    public async Task Every_challenge_solution_passes_its_own_cases(string id)
    {
        byte[] solution = await Compile(Content("challenges", id + ".solution.cs"));
        string framesFile = Content("challenges", id + ".frames.txt");

        // Generate (or regenerate) frames.txt from the solution when asked or when missing.
        var raw = Cases0(id);
        if (raw.Any(c => c.Game is not null) && (Update || !File.Exists(framesFile)))
        {
            var generated = new Dictionary<int, IReadOnlyList<FrameSnapshot>>();
            for (int i = 0; i < raw.Count; i++)
            {
                if (raw[i].Game is null) continue;
                GameSession s = GameSession.StartHeadless(ProgramLoader.FromBytes(solution), raw[i].Input);
                Assert.True(s.IsGame, $"{id}.solution.cs never calls Game.Run but case {i + 1} is a game case.");
                ScriptResult r = ScriptRunner.Run(s, raw[i].Game!);
                Assert.Null(r.Error);
                generated[i + 1] = r.Snapshots;
            }
            File.WriteAllText(framesFile, FramesFile.Format(generated));
        }

        IReadOnlyList<CaseResult> results = ChallengeChecker.Check(solution, Cases(id));
        foreach (CaseResult r in results)
            Assert.True(r.Passed, $"{id} case {r.Index}: {r.Error ?? "output/frames differ"}"
                + (r.FirstDiffLine is int l ? $" (console line {l})" : "")
                + string.Concat((r.Frames ?? Array.Empty<FrameCheck>()).Where(f => !f.Passed)
                    .Select(f => $"\nframe {f.Frame} row {f.FirstDiffRow}:\n{string.Join("\n", f.ActualRows)}")));
    }

    // cases.json without snapshots attached (frames.txt may not exist yet).
    static IReadOnlyList<ChallengeCase> Cases0(string id)
    {
        var errors = new List<string>();
        var cases = CRE132.LessonDoc.ChallengeKit.LoadCases(Content("challenges", id + ".cases.json"), null, errors);
        Assert.True(errors.Count == 0, string.Join("\n", errors));
        return cases;
    }

    [Theory]
    [MemberData(nameof(ChallengeIds))]
    public async Task Every_challenge_starter_does_not_already_pass(string id)
    {
        byte[] starter = await Compile(Content("challenges", id + ".start.cs"));
        IReadOnlyList<CaseResult> results = ChallengeChecker.Check(starter, Cases(id));
        Assert.False(results.All(r => r.Passed), $"{id}: the starter already passes every case — the challenge asks for nothing.");
    }

    [Fact]
    public void Every_catalog_entry_has_a_lesson_file_and_every_lesson_file_a_catalog_entry()
    {
        // A typo'd NarrationFile deploys a lesson whose page permanently shows the retry
        // notice; an md with no catalog row is authored but unreachable. Both are build errors.
        var catalogNames = CRE132.Web.WebCatalog.Entries
            .Select(e => Path.GetFileNameWithoutExtension(e.NarrationFile))
            .OrderBy(n => n).ToList();
        var lessonNames = Directory.GetFiles(Content("lessons"), "*.md")
            .Where(f => CRE132.LessonDoc.NarrationParser.HasDirectives(File.ReadAllText(f)))
            .Select(f => Path.GetFileNameWithoutExtension(f)!)
            .OrderBy(n => n).ToList();
        Assert.Equal(lessonNames, catalogNames!);
    }
}
