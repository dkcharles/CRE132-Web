using CRE132.Game;

namespace CRE132.Engine;

public sealed record FrameResult(int FrameNumber, IReadOnlyList<DrawCommand> Commands, string Console, string? Error);

// One run of one student program, whether or not it turns out to be a game. Start runs Main
// under the ProgramRunner redirection; if Main called Game.Run, Setup runs too and the caller
// then drives Draw with Step, one frame per call - the browser from a timer, the checker and the
// tests from a loop. Nothing here blocks: single-threaded WebAssembly stays responsive between
// frames. The game's state (screen size, frame count, input, random sequence) is owned by this
// object and installed as GameHost.Active only while student code runs, so a stage playing on
// the page and the checker running underneath it never see each other.
public sealed class GameSession
{
    public const long FrameBudget = 5_000_000;
    public const int HeadlessSeed = 12345;

    readonly GameState state;
    readonly BoundedWriter writer = new(ProgramRunner.OutputLimit);
    readonly StringReader stdin;
    string? stopReason;

    public bool IsGame { get; }
    public RunResult Result { get; }
    public int Width => state.Width;
    public int Height => state.Height;
    public string ConsoleText => writer.Text;
    public bool Stopped => stopReason is not null;
    public string? StopReason => stopReason;
    public int FramesRun { get; private set; }

    GameSession(Action main, string input, int seed)
    {
        state = new GameState(seed);
        stdin = new StringReader(input);

        string? error = Under(() => ProgramRunner.Invoke(main, stdin, writer, RunBudget.DefaultLimit));
        if (error is null && state.Draw is not null)
        {
            IsGame = true;
            error = Under(() => ProgramRunner.Invoke(state.Setup!, stdin, writer, RunBudget.DefaultLimit));
            if (error is not null) stopReason = error;
        }
        Result = new RunResult(writer.Text, error);
    }

    public static GameSession Start(Action main, string input) => new(main, input, Environment.TickCount);
    public static GameSession StartHeadless(Action main, string input) => new(main, input, HeadlessSeed);

    public FrameResult Step(InputState input)
    {
        if (!IsGame) throw new InvalidOperationException("Step is only valid when IsGame is true.");
        if (stopReason is not null) return new FrameResult(FramesRun, Array.Empty<DrawCommand>(), writer.Text, stopReason);

        state.Previous = state.Current;
        state.Current = input;
        state.FrameCount = FramesRun;
        state.Frame = new List<DrawCommand>();
        string? error = Under(() => ProgramRunner.Invoke(state.Draw!, stdin, writer, FrameBudget));
        List<DrawCommand> commands = state.Frame;
        state.Frame = null;
        FramesRun++;

        if (error is not null) stopReason = WithFrame(error, FramesRun);
        return new FrameResult(FramesRun, commands, writer.Text, stopReason);
    }

    // Installs this session's state for the duration of one call into student code.
    T Under<T>(Func<T> call)
    {
        GameState? previous = GameHost.Active;
        GameHost.Active = state;
        try { return call(); }
        finally { GameHost.Active = previous; }
    }

    const string ErrorPrefix = "Your program stopped with an error:";

    static string WithFrame(string error, int frame) =>
        error.StartsWith(ErrorPrefix, StringComparison.Ordinal)
            ? $"Your program stopped at frame {frame} with an error:" + error[ErrorPrefix.Length..]
            : $"Frame {frame}: {error}";
}
