using CRE132.LessonDoc;
using Xunit;

namespace CRE132.Tests;

public class CSharpHighlighterTests
{
    [Fact]
    public void A_double_slash_inside_a_string_literal_is_coloured_as_string_not_comment()
    {
        string html = CSharpHighlighter.Highlight("var url = \"http://example.com\";");

        Assert.Contains("<span class=\"hl-s\">&quot;http://example.com&quot;</span>", html);
        Assert.DoesNotContain("hl-c", html);
    }

    [Fact]
    public void Escaped_braces_in_an_interpolated_string_stay_in_the_string()
    {
        string html = CSharpHighlighter.Highlight("var s = $\"a{{b}}c\";");

        Assert.Contains("<span class=\"hl-s\">$&quot;a{{b}}c&quot;</span>", html);
    }

    [Fact]
    public void Verbatim_string_handles_the_doubled_embedded_quote()
    {
        string html = CSharpHighlighter.Highlight("var s = @\"a\"\"b\";");

        Assert.Contains("<span class=\"hl-s\">@&quot;a&quot;&quot;b&quot;</span>", html);
    }

    [Fact]
    public void An_unterminated_string_literal_stops_at_end_of_line_instead_of_eating_the_file()
    {
        string html = CSharpHighlighter.Highlight("var s = \"abc\nint x = 5;");

        // The literal is closed at the newline (no closing quote was found)...
        Assert.Contains("<span class=\"hl-s\">&quot;abc</span>", html);
        // ...and the following line is highlighted normally rather than swallowed as string.
        Assert.Contains("<span class=\"hl-k\">int</span>", html);
        Assert.Contains("<span class=\"hl-n\">5</span>", html);
    }

    [Fact]
    public void Angle_brackets_and_ampersands_in_code_are_html_encoded()
    {
        string html = CSharpHighlighter.Highlight("bool ok = a < b && c;");

        Assert.Contains("&lt;", html);
        Assert.Contains("&amp;&amp;", html);
        Assert.DoesNotContain(" < ", html);
        Assert.DoesNotContain(" && ", html);
    }

    [Fact]
    public void Quotes_in_code_are_html_encoded()
    {
        string html = CSharpHighlighter.Highlight("var s = \"hi\";");

        Assert.Contains("&quot;hi&quot;", html);
        Assert.DoesNotContain("\"hi\"", html);
    }

    // NOTE - not tested: "the hole's contents of an interpolated string are not string-coloured,
    // only the literal parts are". CSharpHighlighter deliberately does NOT re-highlight holes
    // (see its own comment: "the holes of a $"..." are not re-highlighted, because finding their
    // boundaries needs a real parser") - it emits the ENTIRE interpolated literal, hole included,
    // as a single <span class="hl-s"> run. For $"x{a + 1}y" the hole text "a + 1" is wrapped in
    // the same hl-s span as the literal parts "x" and "y", i.e. it IS string-coloured. This is a
    // real mismatch against the fix-wave spec for case (b); flagging it here rather than baking
    // the (wrong, per spec) current behaviour into an assertion. See fixwave-report.md.
}
