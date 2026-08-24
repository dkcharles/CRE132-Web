using CRE132.Engine;
using Xunit;

namespace CRE132.Tests;

public class ProgramRunnerTests
{
    [Fact]
    public void Captures_console_output()
    {
        RunResult r = ProgramRunner.Run(() => Console.WriteLine("hi"), "");
        Assert.Equal("hi" + Environment.NewLine, r.Output);
        Assert.Null(r.Error);
    }

    [Fact]
    public void Feeds_stdin_lines_to_ReadLine()
    {
        RunResult r = ProgramRunner.Run(
            () => Console.WriteLine("Hello " + Console.ReadLine()), "Ada\n");
        Assert.StartsWith("Hello Ada", r.Output);
    }

    [Fact]
    public void ReadLine_past_the_supplied_input_returns_null_not_a_hang()
    {
        RunResult r = ProgramRunner.Run(
            () => Console.WriteLine(Console.ReadLine() ?? "(no input)"), "");
        Assert.StartsWith("(no input)", r.Output);
    }

    [Fact]
    public void A_runtime_exception_becomes_a_friendly_error_with_partial_output_kept()
    {
        RunResult r = ProgramRunner.Run(() =>
        {
            Console.WriteLine("before");
            throw new InvalidOperationException("boom");
        }, "");
        Assert.StartsWith("before", r.Output);
        Assert.Contains("Your program stopped with an error", r.Error);
        Assert.Contains("boom", r.Error);
    }

    [Fact]
    public void A_spent_budget_surfaces_the_endless_loop_message()
    {
        RunResult r = ProgramRunner.Run(() =>
        {
            while (true) RunBudget.Step(); // what instrumented student code does
        }, "", budget: 1000);
        Assert.Contains("loop that never ends", r.Error);
    }

    [Fact]
    public void Unbounded_printing_is_stopped_with_the_output_message()
    {
        RunResult r = ProgramRunner.Run(() =>
        {
            while (true) Console.WriteLine("spam");
        }, "");
        Assert.Contains("loop that keeps printing", r.Error);
        Assert.NotEmpty(r.Output);
    }

    [Fact]
    public void Console_is_restored_after_a_run()
    {
        var before = Console.Out;
        ProgramRunner.Run(() => Console.WriteLine("x"), "");
        Assert.Same(before, Console.Out);
    }

    [Fact]
    public void Doubles_format_with_a_dot_whatever_the_machine_culture()
    {
        RunResult r = ProgramRunner.Run(() => Console.WriteLine(1.5), "");
        Assert.StartsWith("1.5", r.Output);
    }
}
