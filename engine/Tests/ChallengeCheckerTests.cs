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

    const string Mover = """
        double x = 0;
        void Setup() { }
        void Draw() { Screen.Clear(); if (Keys.IsDown(Key.Right)) x = x + 16; Screen.Rect(x, 0, 16, 16, Colour.White); Console.WriteLine("x=" + x); }
        Game.Run(Setup, Draw);
        """;

    static ChallengeCase GameCase(string frameText, string expected = "") =>
        new("", expected, new GameScript(3, new[] { 3 }, new Dictionary<string, string> { ["Right"] = "1-2" }),
            new[] { new FrameSnapshot(3, frameText) });

    static string Row0(string prefix) =>
        string.Join("\n", Enumerable.Range(0, 23).Select(r => r == 0 ? prefix.PadRight(40) : new string(' ', 40)));

    [Fact]
    public async Task A_game_case_passes_when_every_snapshot_matches()
    {
        var results = ChallengeChecker.Check(await Bytes(Mover), new[] { GameCase(Row0("  #")) });
        CaseResult r = Assert.Single(results);
        Assert.True(r.Passed, r.Error);
        Assert.Equal(3, Assert.Single(r.Frames!).Frame);
    }

    [Fact]
    public async Task A_game_case_fails_on_the_first_wrong_row_and_carries_both_grids()
    {
        var results = ChallengeChecker.Check(await Bytes(Mover), new[] { GameCase(Row0("   #")) });
        CaseResult r = results[0];
        Assert.False(r.Passed);
        FrameCheck f = Assert.Single(r.Frames!);
        Assert.False(f.Passed);
        Assert.Equal(1, f.FirstDiffRow);
        Assert.Equal(23, f.ExpectedRows.Count);
        Assert.Equal("  #", f.ActualRows[0].TrimEnd());
    }

    [Fact]
    public async Task A_game_case_with_expected_text_also_compares_the_console_and_a_non_game_fails_plainly()
    {
        var pass = ChallengeChecker.Check(await Bytes(Mover), new[] { GameCase(Row0("  #"), "x=16\nx=32\nx=32") });
        Assert.True(pass[0].Passed, pass[0].Error);
        var wrong = ChallengeChecker.Check(await Bytes(Mover), new[] { GameCase(Row0("  #"), "x=99") });
        Assert.False(wrong[0].Passed);
        Assert.Equal(1, wrong[0].FirstDiffLine);

        var notGame = ChallengeChecker.Check(await Bytes("Console.WriteLine(1);"), new[] { GameCase(Row0("  #")) });
        Assert.Contains("Game.Run(Setup, Draw)", notGame[0].Error);
    }
}
