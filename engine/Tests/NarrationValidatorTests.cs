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
        NarrationValidator.Validate(blocks, Samples, Figures, Challenges);

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
            new[] { new Block("challenge", Id: "c1") }, Samples, Figures, none);
        Assert.Contains(e2, e => e.Contains("no cases"));
    }
}
