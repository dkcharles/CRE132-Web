using System.Globalization;
using System.Reflection;

namespace CRE132.Engine;

public sealed record RunResult(string Output, string? Error);

// Runs one compiled student program with the console redirected: output captured (bounded),
// input pre-supplied, culture pinned to invariant so 1.5 never prints as "1,5" on a dev
// machine (the browser already guarantees this via InvariantGlobalization). Restores
// everything afterwards, error or not.
public static class ProgramRunner
{
    public const int OutputLimit = 1_000_000;

    public static RunResult Run(Action program, string input, long budget = RunBudget.DefaultLimit)
    {
        var writer = new BoundedWriter(OutputLimit);
        TextWriter oldOut = Console.Out;
        // On browser WebAssembly the Console.In GETTER itself throws (no stdin exists to wrap),
        // so the save is conditional - and restoration below is too. Desktop keeps full restore.
        TextReader? oldIn;
        try { oldIn = Console.In; }
        catch (PlatformNotSupportedException) { oldIn = null; }
        CultureInfo oldCulture = CultureInfo.CurrentCulture;

        RunBudget.Reset(budget);
        Console.SetOut(writer);
        Console.SetIn(new StringReader(input));
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        try
        {
            program();
            return new RunResult(writer.Text, null);
        }
        catch (Exception ex)
        {
            // Entry points invoked via reflection wrap the real failure.
            Exception cause = ex is TargetInvocationException { InnerException: not null } tie
                ? tie.InnerException!
                : ex;

            string message = cause is BudgetExceededException or OutputLimitException
                ? cause.Message
                : $"Your program stopped with an error: {cause.GetType().Name}: {cause.Message}";

            return new RunResult(writer.Text, message);
        }
        finally
        {
            Console.SetOut(oldOut);
            if (oldIn is not null) Console.SetIn(oldIn);
            CultureInfo.CurrentCulture = oldCulture;
        }
    }
}
