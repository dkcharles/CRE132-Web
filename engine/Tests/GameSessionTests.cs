using CRE132.Compiler;
using CRE132.Engine;
using CRE132.Game;
using Xunit;

namespace CRE132.Tests;

public class GameSessionTests
{
    static readonly SourceCompiler Compiler = new(new DiskReferenceSource());

    static async Task<Action> Program(string source)
    {
        CompiledBytes c = await Compiler.CompileToBytesAsync(source);
        Assert.True(c.Succeeded, string.Join("\n", c.Errors.Select(e => $"line {e.Line}: {e.Message}")));
        return ProgramLoader.FromBytes(c.Bytes!);
    }

    const string Ball = """
        double x = 0;
        void Setup() { Screen.Size(320, 160); Console.WriteLine("setup"); }
        void Draw() { Screen.Clear(); Screen.Circle(x, 80, 8, Colour.Red); x = x + 10; Console.WriteLine("frame " + Frame.Count); }
        Game.Run(Setup, Draw);
        """;

    [Fact]
    public async Task A_console_program_is_not_a_game_and_keeps_its_result()
    {
        GameSession s = GameSession.StartHeadless(await Program("Console.WriteLine(\"hi\");"), "");
        Assert.False(s.IsGame);
        Assert.StartsWith("hi", s.Result.Output);
        Assert.Null(s.Result.Error);
        Assert.Throws<InvalidOperationException>(() => s.Step(InputState.None));
    }

    [Fact]
    public async Task A_game_runs_setup_once_then_one_draw_per_step_with_frame_count_from_zero()
    {
        GameSession s = GameSession.StartHeadless(await Program(Ball), "");
        Assert.True(s.IsGame);
        Assert.Equal((320, 160), (s.Width, s.Height));
        Assert.Equal("setup" + Environment.NewLine, s.Result.Output);

        FrameResult f1 = s.Step(InputState.None);
        FrameResult f2 = s.Step(InputState.None);
        Assert.Equal(1, f1.FrameNumber);
        Assert.Equal(2, f2.FrameNumber);
        Assert.Equal(2, f1.Commands.Count);                                   // Clear + Circle
        Assert.Equal(0.0, f1.Commands[1].A);
        Assert.Equal(10.0, f2.Commands[1].A);
        Assert.Contains("frame 0", f1.Console);
        Assert.Contains("frame 1", f2.Console);
        Assert.Equal(2, s.FramesRun);
    }

    [Fact]
    public async Task An_exception_in_draw_stops_the_session_and_names_the_frame()
    {
        GameSession s = GameSession.StartHeadless(await Program("""
            int n = 0;
            void Setup() { }
            void Draw() { n++; if (n == 3) throw new InvalidOperationException("boom"); }
            Game.Run(Setup, Draw);
            """), "");
        s.Step(InputState.None); s.Step(InputState.None);
        FrameResult f = s.Step(InputState.None);
        Assert.StartsWith("Your program stopped at frame 3 with an error: InvalidOperationException: boom", f.Error);
        Assert.True(s.Stopped);
        FrameResult again = s.Step(InputState.None);
        Assert.Equal(f.Error, again.Error);
        Assert.Equal(3, s.FramesRun);
    }

    [Fact]
    public async Task An_endless_loop_inside_draw_is_caught_by_the_per_frame_budget()
    {
        GameSession s = GameSession.StartHeadless(await Program("""
            void Setup() { }
            void Draw() { while (true) { } }
            Game.Run(Setup, Draw);
            """), "");
        FrameResult f = s.Step(InputState.None);
        Assert.Equal("Frame 1: Your program ran for too long — it was stopped. Look for a loop that never ends.", f.Error);
    }

    [Fact]
    public async Task A_failing_setup_lands_in_result_and_stops_the_session()
    {
        GameSession s = GameSession.StartHeadless(await Program("""
            void Setup() { int.Parse("nope"); }
            void Draw() { }
            Game.Run(Setup, Draw);
            """), "");
        Assert.True(s.IsGame);
        Assert.Contains("stopped with an error", s.Result.Error);
        Assert.True(s.Stopped);
    }

    [Fact]
    public async Task Input_reaches_keys_and_mouse_with_edges_computed_between_steps()
    {
        GameSession s = GameSession.StartHeadless(await Program("""
            void Setup() { }
            void Draw() { Console.WriteLine(Keys.IsDown(Key.Right) + " " + Keys.WasPressed(Key.Right) + " " + Mouse.X); }
            Game.Run(Setup, Draw);
            """), "");
        var right = new InputState(new HashSet<Key> { Key.Right }, 50, 0, false);
        Assert.Contains("True True 50", s.Step(right).Console);
        Assert.Contains("True False 50", s.Step(right).Console);
        Assert.Contains("False False 0", s.Step(InputState.None).Console);
    }

    [Fact]
    public async Task Two_sessions_do_not_share_state()
    {
        Action program = await Program(Ball);
        GameSession a = GameSession.StartHeadless(program, "");
        GameSession b = GameSession.StartHeadless(program, "");
        a.Step(InputState.None); a.Step(InputState.None);
        FrameResult fb = b.Step(InputState.None);
        Assert.Equal(1, fb.FrameNumber);
        Assert.Equal(0.0, fb.Commands[1].A);
    }

    [Fact]
    public async Task Headless_sessions_share_a_seed_so_rand_repeats()
    {
        const string src = "void Setup() { } void Draw() { Console.WriteLine(Rand.Range(0, 1000000)); } Game.Run(Setup, Draw);";
        Action program = await Program(src);
        string a = GameSession.StartHeadless(program, "").Step(InputState.None).Console;
        string b = GameSession.StartHeadless(program, "").Step(InputState.None).Console;
        Assert.Equal(a, b);
    }
}
