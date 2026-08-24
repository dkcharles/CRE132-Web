using CRE132.Engine;
using Xunit;

namespace CRE132.Tests;

public class RunBudgetTests
{
    [Fact]
    public void Step_throws_once_the_budget_is_spent()
    {
        RunBudget.Reset(3);
        RunBudget.Step();
        RunBudget.Step();
        RunBudget.Step();
        Assert.Throws<BudgetExceededException>(RunBudget.Step);
    }

    [Fact]
    public void Reset_restores_a_spent_budget()
    {
        RunBudget.Reset(1);
        RunBudget.Step();
        RunBudget.Reset(1);
        RunBudget.Step(); // must not throw
    }

    [Fact]
    public void The_budget_message_tells_a_beginner_what_to_look_for()
    {
        Assert.Equal(
            "Your program ran for too long — it was stopped. Look for a loop that never ends.",
            new BudgetExceededException().Message);
    }

    [Fact]
    public void The_output_cap_message_tells_a_beginner_what_to_look_for()
    {
        Assert.Equal(
            "Your program printed too much text — it was stopped. Look for a loop that keeps printing.",
            new OutputLimitException().Message);
    }
}
