namespace CRE132.Engine;

// A verdict panel needs both sides in the trimmed form the comparison actually used, or the
// highlighted "first differing line" can point at a line the student sees differently.
public sealed record CaseResult(
    int Index,
    bool Passed,
    string Input,
    IReadOnlyList<string> ExpectedLines,
    IReadOnlyList<string> ActualLines,
    string? Error,
    int? FirstDiffLine);

// Runs one compiled program against every case. Each case loads a FRESH copy of the assembly
// (ProgramLoader.FromBytes), so static fields in student code start from zero every time -
// without that, a counter incremented in case 1 would still hold its value in case 2.
public static class ChallengeChecker
{
    public static IReadOnlyList<CaseResult> Check(byte[] program, IReadOnlyList<ChallengeCase> cases)
    {
        var results = new List<CaseResult>(cases.Count);
        for (int i = 0; i < cases.Count; i++)
        {
            ChallengeCase c = cases[i];
            RunResult run = ProgramRunner.Run(ProgramLoader.FromBytes(program), c.Input);
            int? diff = run.Error is null ? OutputComparer.FirstDifference(c.Expected, run.Output) : null;
            bool passed = run.Error is null && diff is null;
            results.Add(new CaseResult(
                i + 1,
                passed,
                c.Input,
                OutputComparer.Lines(c.Expected),
                OutputComparer.Lines(run.Output),
                run.Error,
                diff));
        }
        return results;
    }
}
