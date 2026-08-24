using CRE132.Compiler;
using CRE132.Engine;
using Xunit;

namespace CRE132.Tests;

public class ChallengeCheckerTests
{
    static readonly SourceCompiler Compiler = new(new DiskReferenceSource());

    static async Task<byte[]> Bytes(string source)
    {
        CompiledBytes c = await Compiler.CompileToBytesAsync(source);
        Assert.True(c.Succeeded);
        return c.Bytes!;
    }

    [Fact]
    public async Task A_correct_program_passes_all_cases()
    {
        byte[] program = await Bytes(
            "string name = Console.ReadLine();\nConsole.WriteLine(\"Hi \" + name);");
        var results = ChallengeChecker.Check(program, new[]
        {
            new ChallengeCase("Ada\n", "Hi Ada"),
            new ChallengeCase("Bo\n", "Hi Bo")
        });
        Assert.All(results, r => Assert.True(r.Passed));
    }

    [Fact]
    public async Task A_wrong_output_reports_the_first_differing_line_and_both_sides()
    {
        byte[] program = await Bytes("Console.WriteLine(\"a\");\nConsole.WriteLine(\"WRONG\");");
        var results = ChallengeChecker.Check(program, new[]
        {
            new ChallengeCase("", "a\nb")
        });
        CaseResult r = Assert.Single(results);
        Assert.False(r.Passed);
        Assert.Equal(2, r.FirstDiffLine);
        Assert.Equal(new[] { "a", "b" }, r.ExpectedLines);
        Assert.Equal(new[] { "a", "WRONG" }, r.ActualLines);
    }

    [Fact]
    public async Task A_crashing_program_fails_with_its_error_not_an_exception()
    {
        byte[] program = await Bytes("int x = int.Parse(\"nope\");");
        var results = ChallengeChecker.Check(program, new[] { new ChallengeCase("", "anything") });
        Assert.False(results[0].Passed);
        Assert.Contains("stopped with an error", results[0].Error);
    }

    [Fact]
    public async Task Static_state_does_not_leak_between_cases()
    {
        byte[] program = await Bytes("""
            class Program
            {
                static int n = 0;
                static void Main() { n++; Console.WriteLine(n); }
            }
            """);
        var results = ChallengeChecker.Check(program, new[]
        {
            new ChallengeCase("", "1"),
            new ChallengeCase("", "1")
        });
        Assert.All(results, r => Assert.True(r.Passed));
    }
}
