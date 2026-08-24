using CRE132.LessonDoc;
using Xunit;

namespace CRE132.Tests;

public class NarrationParserTests
{
    [Fact]
    public void Prose_between_directives_becomes_html_blocks()
    {
        var blocks = NarrationParser.Parse("Hello **world**\n\n:::run s01-hello\n\nAfter");
        Assert.Equal(3, blocks.Count);
        Assert.Equal("prose", blocks[0].Kind);
        Assert.Contains("<strong>world</strong>", blocks[0].Html);
        Assert.Equal("run", blocks[1].Kind);
        Assert.Equal("s01-hello", blocks[1].Id);
        Assert.Equal("prose", blocks[2].Kind);
    }

    [Fact]
    public void Run_and_edit_require_an_id()
    {
        Assert.Throws<NarrationException>(() => NarrationParser.Parse(":::run"));
        Assert.Throws<NarrationException>(() => NarrationParser.Parse(":::edit"));
    }

    [Fact]
    public void A_challenge_captures_id_and_task_statement()
    {
        var blocks = NarrationParser.Parse(":::challenge c01-three-lines\nPrint *exactly* three lines.\n:::");
        Block b = Assert.Single(blocks);
        Assert.Equal("challenge", b.Kind);
        Assert.Equal("c01-three-lines", b.Id);
        Assert.Contains("<em>exactly</em>", b.Html);
    }

    [Fact]
    public void An_unclosed_body_directive_is_an_error_with_its_line()
    {
        var ex = Assert.Throws<NarrationException>(() => NarrationParser.Parse("one\n:::key\nnever closed"));
        Assert.Equal(2, ex.Line);
    }

    [Fact]
    public void Key_and_try_become_callouts_and_unity_is_gone()
    {
        var blocks = NarrationParser.Parse(":::key\nBig idea.\n:::");
        Assert.Equal("callout", blocks[0].Kind);
        Assert.Equal("key", blocks[0].Variant);
        Assert.Throws<NarrationException>(() => NarrationParser.Parse(":::unity\nx\n:::"));
    }

    [Fact]
    public void A_leading_h1_is_dropped_but_only_as_the_first_line()
    {
        var blocks = NarrationParser.Parse("# Title\n\nBody text");
        Block b = Assert.Single(blocks);
        Assert.DoesNotContain("Title", b.Html);
        Assert.Contains("Body text", b.Html);
    }

    [Fact]
    public void Csharp_fences_in_prose_are_highlighted_at_build_time()
    {
        var blocks = NarrationParser.Parse("```csharp\nint x = 1;\n```");
        Assert.Contains("language-csharp", blocks[0].Html);
        Assert.Contains("<span", blocks[0].Html);   // the highlighter left its mark
    }
}
