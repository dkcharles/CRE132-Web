using CRE132.Engine;
using CRE132.LessonDoc;
using Xunit;

namespace CRE132.Tests;

public class NarrationValidatorTests
{
    static readonly Dictionary<string, string> Samples = new() { ["s1"] = "Console.WriteLine(1);" };
    static readonly Dictionary<string, string> Figures = new() { ["fig"] = "<svg/>" };
    static readonly Dictionary<string, ChallengeFiles> Challenges = new()
    {
        ["c1"] = new ChallengeFiles("// start here", new[] { new ChallengeCase("", "hi\n") })
    };

    static (IReadOnlyList<Block> Resolved, IReadOnlyList<string> Errors) Run(params Block[] blocks) =>
        NarrationValidator.Validate(blocks, Samples, Figures, Challenges, new Dictionary<string, string>());

    [Fact]
    public void A_run_naming_a_missing_sample_is_an_error()
    {
        var (_, errors) = Run(new Block("run", Id: "nope"));
        Assert.Contains(errors, e => e.Contains("nope"));
    }

    [Fact]
    public void A_run_gets_highlighted_code_and_an_edit_gets_raw_code()
    {
        var (resolved, errors) = Run(new Block("run", Id: "s1"), new Block("edit", Id: "s1"));
        Assert.Empty(errors);
        Assert.Contains("<span", resolved[0].Code);
        Assert.Equal("Console.WriteLine(1);", resolved[1].Code);
    }

    [Fact]
    public void A_challenge_gets_starter_and_cases()
    {
        var (resolved, errors) = Run(new Block("challenge", Id: "c1", Html: "<p>task</p>"));
        Assert.Empty(errors);
        Assert.Equal("// start here", resolved[0].Code);
        Assert.Single(resolved[0].Cases!);
    }

    [Fact]
    public void A_challenge_with_missing_files_or_no_cases_is_an_error()
    {
        var (_, e1) = Run(new Block("challenge", Id: "ghost"));
        Assert.Contains(e1, e => e.Contains("ghost"));

        var none = new Dictionary<string, ChallengeFiles> { ["c1"] = new("s", Array.Empty<ChallengeCase>()) };
        var (_, e2) = NarrationValidator.Validate(
            new[] { new Block("challenge", Id: "c1") }, Samples, Figures, none, new Dictionary<string, string>());
        Assert.Contains(e2, e => e.Contains("no cases"));
    }

    [Fact]
    public void A_challenge_whose_kit_failed_to_load_is_reported_as_such_not_as_missing_files()
    {
        var failed = new HashSet<string> { "broken" };
        var (_, errors) = NarrationValidator.Validate(
            new[] { new Block("challenge", Id: "broken") },
            Samples, Figures, Challenges, new Dictionary<string, string>(), failed);

        string message = Assert.Single(errors);
        Assert.Contains("see the messages above", message);
        Assert.DoesNotContain("missing one or more of", message);
    }

    [Fact]
    public void A_sample_with_declared_input_carries_it_on_run_and_edit_blocks()
    {
        var inputs = new Dictionary<string, string> { ["s1"] = "16\n" };
        var (resolved, errors) = NarrationValidator.Validate(
            new[] { new Block("run", Id: "s1"), new Block("edit", Id: "s1") },
            Samples, Figures, Challenges, inputs);
        Assert.Empty(errors);
        Assert.Equal("16\n", resolved[0].Input);
        Assert.Equal("16\n", resolved[1].Input);
    }

    [Fact]
    public void A_challenge_carries_its_solution_and_hint_to_the_page()
    {
        var kit = new ChallengeFiles("// s", new[] { new ChallengeCase("", "hi\n") },
                                     Solution: "int a = 1;", SolutionHtml: "<span>int</span> a = 1;", Hint: "<p>h</p>");
        var (resolved, errors) = NarrationValidator.Validate(
            new[] { new Block("challenge", Id: "c1") }, Samples, Figures,
            new Dictionary<string, ChallengeFiles> { ["c1"] = kit }, new Dictionary<string, string>());
        Assert.Empty(errors);
        Assert.Equal("int a = 1;", resolved[0].Solution);
        Assert.Equal("<span>int</span> a = 1;", resolved[0].SolutionHtml);
        Assert.Equal("<p>h</p>", resolved[0].Hint);
    }
}
