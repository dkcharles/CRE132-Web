using CRE132.Engine;
using Xunit;

namespace CRE132.Tests;

public class FramesFileTests
{
    [Fact]
    public void Format_then_parse_round_trips_two_cases_with_blank_rows_intact()
    {
        var cases = new Dictionary<int, IReadOnlyList<FrameSnapshot>>
        {
            [1] = new[] { new FrameSnapshot(30, "   \n  o\n   "), new FrameSnapshot(60, "o  \n   \n   ") },
            [3] = new[] { new FrameSnapshot(1, "###\n   \n   ") }
        };
        string text = FramesFile.Format(cases);
        Assert.StartsWith("=== case 1 ===\n--- frame 30 ---\n", text);
        var back = FramesFile.Parse(text);
        Assert.Equal(new[] { 1, 3 }, back.Keys.OrderBy(k => k));
        Assert.Equal(30, back[1][0].Frame);
        Assert.Equal(3, back[1][0].Text.Split('\n').Length);      // the leading blank row survived
        Assert.Equal("  o", back[1][0].Text.Split('\n')[1]);
        Assert.Equal(1, back[3][0].Frame);
    }

    [Fact]
    public void Parse_rejects_text_that_does_not_start_with_a_case_header()
    {
        Assert.Throws<FormatException>(() => FramesFile.Parse("--- frame 1 ---\n"));
    }
}
