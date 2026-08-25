using CRE132.Engine;
using Xunit;

namespace CRE132.Tests;

public class OutputComparerTests
{
    [Fact]
    public void Identical_output_matches() =>
        Assert.Null(OutputComparer.FirstDifference("a\nb\n", "a\nb\n"));

    [Fact]
    public void Trailing_spaces_and_crlf_and_outer_blank_lines_are_forgiven()
    {
        Assert.Null(OutputComparer.FirstDifference("a\nb", "a  \r\nb\r\n\r\n"));
        Assert.Null(OutputComparer.FirstDifference("\na\n", "a"));
    }

    [Fact]
    public void Case_differences_are_real_differences() =>
        Assert.Equal(1, OutputComparer.FirstDifference("Hello", "hello"));

    [Fact]
    public void The_first_differing_line_is_reported_one_based()
    {
        Assert.Equal(2, OutputComparer.FirstDifference("a\nb\nc", "a\nX\nc"));
        Assert.Equal(3, OutputComparer.FirstDifference("a\nb\nc", "a\nb"));      // missing line
        Assert.Equal(3, OutputComparer.FirstDifference("a\nb", "a\nb\nextra")); // extra line
    }

    [Fact]
    public void Interior_blank_lines_are_significant()
    {
        Assert.Equal(2, OutputComparer.FirstDifference("a\n\nb", "a\nb"));
    }

    [Fact]
    public void FirstDifferentRow_keeps_blank_rows_and_pads_a_short_side()
    {
        Assert.Equal(2, OutputComparer.FirstDifferentRow("   \n o \n   ", "   \n   \n o "));
        Assert.Null(OutputComparer.FirstDifferentRow(" o \n   \n   ", " o   \n\n"));
        Assert.Null(OutputComparer.FirstDifferentRow("a\r\nb", "a\nb"));
    }
}
