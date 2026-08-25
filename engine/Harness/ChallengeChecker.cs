namespace CRE132.Engine;

public sealed record FrameCheck(int Frame, bool Passed, IReadOnlyList<string> ExpectedRows, IReadOnlyList<string> ActualRows, int? FirstDiffRow);

// A verdict panel needs both sides in the trimmed form the comparison actually used, or the
// highlighted "first differing line" can point at a line the student sees differently.
public sealed record CaseResult(
    int Index,
    bool Passed,
    string Input,
    IReadOnlyList<string> ExpectedLines,
    IReadOnlyList<string> ActualLines,
    string? Error,
    int? FirstDiffLine,
    IReadOnlyList<FrameCheck>? Frames = null);

// Runs one compiled program against every case. Each case loads a FRESH copy of the assembly
// (ProgramLoader.FromBytes), so static fields in student code start from zero every time -
// without that, a counter incremented in case 1 would still hold its value in case 2.
public static class ChallengeChecker
{
    public static IReadOnlyList<CaseResult> Check(byte[] program, IReadOnlyList<ChallengeCase> cases)
    {
        if (cases.Count == 0) throw new ArgumentException("a challenge needs at least one case", nameof(cases));

        var results = new List<CaseResult>(cases.Count);
        for (int i = 0; i < cases.Count; i++)
        {
            ChallengeCase c = cases[i];
            GameSession session = GameSession.StartHeadless(ProgramLoader.FromBytes(program), c.Input);
            results.Add(c.Game is null ? ConsoleCase(i + 1, c, session) : GameCase(i + 1, c, session));
        }
        return results;
    }

    static CaseResult ConsoleCase(int index, ChallengeCase c, GameSession session)
    {
        RunResult run = session.Result;
        int? diff = run.Error is null ? OutputComparer.FirstDifference(c.Expected, run.Output) : null;
        return new CaseResult(index, run.Error is null && diff is null, c.Input,
            OutputComparer.Lines(c.Expected), OutputComparer.Lines(run.Output), run.Error, diff);
    }

    static CaseResult GameCase(int index, ChallengeCase c, GameSession session)
    {
        string? error = session.Result.Error;
        if (error is null && !session.IsGame)
            error = "This challenge needs a game: end your program with Game.Run(Setup, Draw).";

        var frames = new List<FrameCheck>();
        string console = session.Result.Output;
        if (error is null)
        {
            ScriptResult r = ScriptRunner.Run(session, c.Game!);
            error = r.Error;
            console = r.Console;
            foreach (FrameSnapshot expected in c.Frames ?? Array.Empty<FrameSnapshot>())
            {
                FrameSnapshot? actual = r.Snapshots.FirstOrDefault(s => s.Frame == expected.Frame);
                string actualText = actual?.Text ?? "";
                int? row = OutputComparer.FirstDifferentRow(expected.Text, actualText);
                frames.Add(new FrameCheck(expected.Frame, row is null && actual is not null,
                    OutputComparer.Rows(expected.Text), OutputComparer.Rows(actualText), row ?? (actual is null ? 1 : null)));
            }
        }

        bool compareConsole = c.Expected.Length > 0;
        int? diff = error is null && compareConsole ? OutputComparer.FirstDifference(c.Expected, console) : null;
        bool passed = error is null && frames.All(f => f.Passed) && diff is null;
        return new CaseResult(index, passed, c.Input,
            OutputComparer.Lines(c.Expected), OutputComparer.Lines(console), error, diff, frames);
    }
}
