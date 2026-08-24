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

    [Theory]
    [MemberData(nameof(SampleIds))]
    public async Task Every_sample_produces_its_golden_output(string id)
    {
        string source = File.ReadAllText(Content("samples", id + ".cs"));
        string golden = File.ReadAllText(Content("samples", id + ".out.txt"));
        // .in.txt is build-gated out of samples/ for now (LessonDoc rejects it) - this stays
        // ready to re-activate the moment the input panel lands in Phase 3.
        string stdinFile = Content("samples", id + ".in.txt");
        string stdin = File.Exists(stdinFile) ? File.ReadAllText(stdinFile) : "";

        CompiledBytes compiled = await Compiler.CompileToBytesAsync(source);
        Assert.True(compiled.Succeeded, string.Join("\n",
            compiled.Errors.Select(e => $"line {e.Line}: {e.Message}")));

        RunResult run = ProgramRunner.Run(ProgramLoader.FromBytes(compiled.Bytes!), stdin);
        Assert.Null(run.Error);
        Assert.Null(OutputComparer.FirstDifference(golden, run.Output));
    }

    [Theory]
    [MemberData(nameof(ChallengeIds))]
    public async Task Every_challenge_solution_passes_its_own_cases(string id)
    {
        string solution = File.ReadAllText(Content("challenges", id + ".solution.cs"));
        var cases = System.Text.Json.JsonSerializer.Deserialize<List<ChallengeCase>>(
            File.ReadAllText(Content("challenges", id + ".cases.json")),
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        Assert.NotEmpty(cases);

        CompiledBytes compiled = await Compiler.CompileToBytesAsync(solution);
        Assert.True(compiled.Succeeded, string.Join("\n",
            compiled.Errors.Select(e => $"line {e.Line}: {e.Message}")));

        foreach (ChallengeCase c in cases)
        {
            RunResult run = ProgramRunner.Run(ProgramLoader.FromBytes(compiled.Bytes!), c.Input);
            Assert.Null(run.Error);
            Assert.Null(OutputComparer.FirstDifference(c.Expected, run.Output));
        }
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
