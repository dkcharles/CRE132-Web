namespace CRE132.Engine;

// The instrumented safety net for infinite loops. The compiler injects a call to Step() into
// every loop body (and before every goto) in student code; ProgramRunner calls Reset() before
// each run. A while(true) then throws after the budget instead of freezing the browser tab
// forever - WebAssembly is single-threaded, so a runaway loop cannot be interrupted any other
// way.
//
// A step COUNT, not a clock: reading the time every iteration costs more than an integer
// decrement, and a count is deterministic across fast and slow machines - the same program
// either always exceeds the budget or never does.
public static class RunBudget
{
    public const long DefaultLimit = 50_000_000;

    static long remaining = long.MaxValue;

    public static void Reset(long limit = DefaultLimit) => remaining = limit;

    public static void Step()
    {
        if (--remaining < 0) throw new BudgetExceededException();
    }
}

public sealed class BudgetExceededException : Exception
{
    public BudgetExceededException()
        : base("Your program ran for too long — it was stopped. Look for a loop that never ends.") { }
}

public sealed class OutputLimitException : Exception
{
    public OutputLimitException()
        : base("Your program printed too much text — it was stopped. Look for a loop that keeps printing.") { }
}
