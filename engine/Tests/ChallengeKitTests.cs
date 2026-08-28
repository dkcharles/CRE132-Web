using CRE132.Engine;
using CRE132.LessonDoc;
using Xunit;

namespace CRE132.Tests;

public class ChallengeKitTests : IDisposable
{
    readonly string dir = Path.Combine(Path.GetTempPath(), "cre132-kit-" + Guid.NewGuid().ToString("N"));

    public ChallengeKitTests() => Directory.CreateDirectory(dir);
    public void Dispose() => Directory.Delete(dir, recursive: true);

    void Put(string name, string text) => File.WriteAllText(Path.Combine(dir, name), text);

    void ConsoleKit(string id)
    {
        Put(id + ".start.cs", "// start\n"); Put(id + ".solution.cs", "Console.WriteLine(1);\n");
        Put(id + ".cases.json", "[ { \"input\": \"\", \"expected\": \"1\" } ]");
    }

    [Fact]
    public void A_console_kit_loads_as_before()
    {
        ConsoleKit("c1");
        var (kit, errors) = ChallengeKit.Load(dir, "c1");
        Assert.Empty(errors);
        Assert.Equal("// start\n", kit!.Starter);
        Assert.Null(kit.Cases[0].Game);
    }

    [Fact]
    public void A_game_kit_attaches_snapshots_from_frames_txt()
    {
        Put("c2.start.cs", "// s\n"); Put("c2.solution.cs", "// s\n");
        Put("c2.cases.json", "[ { \"game\": { \"frames\": 3, \"snapshot\": [1, 3], \"keys\": { \"Right\": \"1-2\" } } } ]");
        string grid = string.Join("\n", Enumerable.Repeat(new string(' ', 40), 23));
        Put("c2.frames.txt", FramesFile.Format(new Dictionary<int, IReadOnlyList<FrameSnapshot>>
        {
            [1] = new[] { new FrameSnapshot(1, grid), new FrameSnapshot(3, grid) }
        }));
        var (kit, errors) = ChallengeKit.Load(dir, "c2");
        Assert.Empty(errors);
        Assert.Equal(new[] { 1, 3 }, kit!.Cases[0].Frames!.Select(f => f.Frame));
    }

    [Theory]
    [InlineData("[ { \"game\": { \"frames\": 0, \"snapshot\": [1] } } ]", "frames")]
    [InlineData("[ { \"game\": { \"frames\": 5 } } ]", "snapshot")]
    [InlineData("[ { \"game\": { \"frames\": 5, \"snapshot\": [6] } } ]", "snapshot")]
    [InlineData("[ { \"game\": { \"frames\": 5, \"snapshot\": [5], \"keys\": { \"RightArrow\": \"1\" } } } ]", "RightArrow")]
    [InlineData("[ { \"game\": { \"frames\": 5, \"snapshot\": [5], \"keys\": { \"Right\": \"9-1\" } } } ]", "9-1")]
    [InlineData("[ { \"game\": { \"frames\": 5, \"snapshot\": [5], \"mouse\": { \"down\": \"x\" } } } ]", "x")]
    public void A_bad_script_is_reported_with_the_offending_value(string cases, string mention)
    {
        Put("c3.start.cs", ""); Put("c3.solution.cs", ""); Put("c3.cases.json", cases);
        var (_, errors) = ChallengeKit.Load(dir, "c3");
        Assert.Contains(errors, e => e.Contains(mention));
    }

    [Fact]
    public void Frames_txt_must_exist_iff_a_case_is_a_game_and_must_cover_every_game_case_and_snapshot()
    {
        Put("c4.start.cs", ""); Put("c4.solution.cs", "");
        Put("c4.cases.json", "[ { \"game\": { \"frames\": 2, \"snapshot\": [2] } } ]");
        var (_, missing) = ChallengeKit.Load(dir, "c4");
        Assert.Contains(missing, e => e.Contains("c4.frames.txt"));

        Put("c4.frames.txt", "=== case 1 ===\n--- frame 1 ---\n\n");     // wrong frame number
        var (_, wrong) = ChallengeKit.Load(dir, "c4");
        Assert.Contains(wrong, e => e.Contains("frame 2"));

        ConsoleKit("c5");
        Put("c5.frames.txt", "=== case 1 ===\n");
        var (_, stray) = ChallengeKit.Load(dir, "c5");
        Assert.Contains(stray, e => e.Contains("c5.frames.txt"));
    }

    [Fact]
    public void Bootstrapping_turns_a_missing_frames_txt_into_a_warning_but_a_stray_one_is_still_an_error()
    {
        Put("c6.start.cs", ""); Put("c6.solution.cs", "");
        Put("c6.cases.json", "[ { \"game\": { \"frames\": 2, \"snapshot\": [2] } } ]");

        var (kit, errors) = ChallengeKit.Load(dir, "c6");
        Assert.Null(kit);
        Assert.Contains(errors, e => e.Contains("c6.frames.txt"));

        var (bootKit, bootMessages) = ChallengeKit.Load(dir, "c6", bootstrapping: true);
        Assert.NotNull(bootKit);
        Assert.Null(bootKit!.Cases[0].Frames);
        string message = Assert.Single(bootMessages);
        Assert.StartsWith("warning: ", message);

        ConsoleKit("c7");
        Put("c7.frames.txt", "=== case 1 ===\n");
        var (strayKit, strayErrors) = ChallengeKit.Load(dir, "c7", bootstrapping: true);
        Assert.Null(strayKit);
        Assert.Contains(strayErrors, e => e.Contains("c7.frames.txt"));
    }

    [Fact]
    public void Bootstrapping_turns_a_stale_frames_txt_missing_a_case_into_a_warning_but_keeps_the_covered_case()
    {
        Put("c8.start.cs", ""); Put("c8.solution.cs", "");
        Put("c8.cases.json",
            "[ { \"game\": { \"frames\": 2, \"snapshot\": [2] } }, " +
            "{ \"game\": { \"frames\": 2, \"snapshot\": [2] } } ]");
        string grid = string.Join("\n", Enumerable.Repeat(new string(' ', 40), 23));
        Put("c8.frames.txt", FramesFile.Format(new Dictionary<int, IReadOnlyList<FrameSnapshot>>
        {
            [1] = new[] { new FrameSnapshot(2, grid) }
        }));

        var (kit, errors) = ChallengeKit.Load(dir, "c8");
        Assert.Null(kit);
        Assert.Contains(errors, e => e.Contains("case 2"));

        var (bootKit, bootErrors) = ChallengeKit.Load(dir, "c8", bootstrapping: true);
        Assert.NotNull(bootKit);
        Assert.Null(bootKit!.Cases[1].Frames);
        Assert.Contains(bootErrors, e => e.StartsWith("warning: ") && e.Contains("case 2"));
        Assert.NotNull(bootKit.Cases[0].Frames);
    }

    [Fact]
    public void A_kit_ships_its_solution_raw_and_highlighted_and_its_hint_when_there_is_one()
    {
        ConsoleKit("c6");
        var (kit, errors) = ChallengeKit.Load(dir, "c6");
        Assert.Empty(errors);
        Assert.Equal("Console.WriteLine(1);\n", kit!.Solution);
        Assert.Contains("<span", kit.SolutionHtml);
        Assert.Null(kit.Hint);

        Put("c6.hint.md", "Try `x`.\n");
        var (withHint, hintErrors) = ChallengeKit.Load(dir, "c6");
        Assert.Empty(hintErrors);
        Assert.Equal("<p>Try <code>x</code>.</p>", withHint!.Hint);
    }

    [Theory]
    [InlineData("  \n")]
    [InlineData(":::key\nno directives in a hint\n:::\n")]
    public void An_empty_or_directive_bearing_hint_fails_the_kit(string hint)
    {
        ConsoleKit("c7"); Put("c7.hint.md", hint);
        var (kit, errors) = ChallengeKit.Load(dir, "c7");
        Assert.Null(kit);
        Assert.Contains(errors, e => e.Contains("c7.hint.md"));
    }
}
