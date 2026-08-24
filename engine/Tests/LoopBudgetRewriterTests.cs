using CRE132.Compiler;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace CRE132.Tests;

public class LoopBudgetRewriterTests
{
    const string Check = "global::CRE132.Engine.RunBudget.Step();";

    static string Instrument(string source) =>
        LoopBudgetRewriter.Instrument(CSharpSyntaxTree.ParseText(source)).ToString();

    [Fact]
    public void A_while_loop_body_gets_a_budget_check()
    {
        string result = Instrument("while (true) { x++; }");
        Assert.Contains(Check, result);
        Assert.True(result.IndexOf(Check) < result.IndexOf("x++"),
            "the check must run before the body");
    }

    [Fact]
    public void For_do_and_foreach_loops_are_all_instrumented()
    {
        Assert.Contains(Check, Instrument("for (int i = 0; ; i++) { }"));
        Assert.Contains(Check, Instrument("do { } while (true);"));
        Assert.Contains(Check, Instrument("foreach (var x in items) { }"));
    }

    [Fact]
    public void A_braceless_loop_body_is_wrapped_in_a_block()
    {
        string result = Instrument("while (true) x++;");
        Assert.Contains(Check, result);
        // The rewritten body must still be a single statement to the parser - a block.
        Assert.Contains("{", result);
    }

    [Fact]
    public void Nested_loops_get_one_check_each()
    {
        string result = Instrument("while (true) { while (true) { } }");
        Assert.Equal(2, CountOf(result, Check));
    }

    [Fact]
    public void A_goto_gets_a_check_so_label_loops_cannot_dodge_the_budget()
    {
        string result = Instrument("start: x++; goto start;");
        Assert.Contains(Check, result);
        Assert.True(result.IndexOf(Check) < result.IndexOf("goto start"),
            "the check must run before the jump");
    }

    [Fact]
    public void Code_without_loops_is_untouched()
    {
        string source = "Console.WriteLine(\"hi\");";
        Assert.Equal(source, Instrument(source));
    }

    static int CountOf(string text, string needle)
    {
        int count = 0, at = 0;
        while ((at = text.IndexOf(needle, at)) >= 0) { count++; at += needle.Length; }
        return count;
    }
}
