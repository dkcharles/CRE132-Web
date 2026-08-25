using CRE132.Compiler;
using CRE132.Engine;
using CRE132.Game;
using Xunit;

namespace CRE132.Tests;

public class ScriptRunnerTests
{
    static readonly SourceCompiler Compiler = new(new DiskReferenceSource());

    static async Task<GameSession> Session(string source)
    {
        CompiledBytes c = await Compiler.CompileToBytesAsync(source);
        Assert.True(c.Succeeded, string.Join("\n", c.Errors.Select(e => e.Message)));
        return GameSession.StartHeadless(ProgramLoader.FromBytes(c.Bytes!), "");
    }

    const string Paddle = """
        double x = 0;
        void Setup() { }
        void Draw()
        {
            Screen.Clear();
            if (Keys.IsDown(Key.Right)) x = x + 16;
            if (Keys.WasPressed(Key.Space)) Console.WriteLine("space at " + Frame.Count);
            if (Mouse.IsDown) Console.WriteLine("mouse " + Mouse.X + "," + Mouse.Y);
            Screen.Rect(x, 0, 16, 16, Colour.White);
        }
        Game.Run(Setup, Draw);
        """;

    [Fact]
    public void Frame_ranges_parse_inclusive_ranges_and_single_frames()
    {
        Assert.Equal(new FrameRange(10, 30), FrameRange.Parse("10-30"));
        Assert.Equal(new FrameRange(5, 5), FrameRange.Parse(" 5 "));
        Assert.True(FrameRange.Parse("10-30").Contains(30));
        Assert.False(FrameRange.Parse("10-30").Contains(31));
        Assert.Throws<FormatException>(() => FrameRange.Parse("30-10"));
        Assert.Throws<FormatException>(() => FrameRange.Parse("ten"));
        Assert.Throws<FormatException>(() => ScriptRunner.ParseKey("RightArrow"));
        Assert.Throws<FormatException>(() => ScriptRunner.ParseKey("3"));
        Assert.Equal(Key.D3, ScriptRunner.ParseKey("D3"));
    }

    [Fact]
    public async Task Keys_are_held_over_their_range_and_snapshots_are_taken_at_the_named_frames()
    {
        var script = new GameScript(5, new[] { 1, 3, 5 }, new Dictionary<string, string> { ["Right"] = "2-3" });
        ScriptResult r = ScriptRunner.Run(await Session(Paddle), script);
        Assert.Null(r.Error);
        Assert.Equal(new[] { 1, 3, 5 }, r.Snapshots.Select(s => s.Frame));
        string[] f1 = r.Snapshots[0].Text.Split('\n'), f3 = r.Snapshots[1].Text.Split('\n'), f5 = r.Snapshots[2].Text.Split('\n');
        Assert.Equal('#', f1[0][0]);          // not yet moved
        Assert.Equal('#', f3[0][2]);          // moved twice (frames 2 and 3)
        Assert.Equal('#', f5[0][2]);          // key released: stays
    }

    [Fact]
    public async Task WasPressed_fires_on_the_first_frame_of_a_range_and_the_mouse_script_applies()
    {
        var script = new GameScript(4, new[] { 4 },
            new Dictionary<string, string> { ["Space"] = "2-3" }, new MouseScript(100, 50, "4"));
        ScriptResult r = ScriptRunner.Run(await Session(Paddle), script);
        Assert.Contains("space at 1", r.Console);       // Frame.Count 1 == script frame 2
        Assert.DoesNotContain("space at 2", r.Console);
        Assert.Contains("mouse 100,50", r.Console);
    }

    [Fact]
    public async Task A_crash_mid_script_returns_the_error_and_the_snapshots_so_far()
    {
        GameSession s = await Session("""
            void Setup() { }
            void Draw() { Screen.Rect(0, 0, 16, 16, Colour.White); if (Frame.Count == 2) throw new Exception("bang"); }
            Game.Run(Setup, Draw);
            """);
        ScriptResult r = ScriptRunner.Run(s, new GameScript(10, new[] { 1, 5 }));
        Assert.Single(r.Snapshots);
        Assert.Contains("at frame 3", r.Error);
    }
}
