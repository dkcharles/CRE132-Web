using CRE132.Engine;
using Xunit;

namespace CRE132.Tests;

public class SourceTextTests
{
    [Fact]
    public void Normalise_makes_CRLF_and_LF_versions_identical()
    {
        Assert.Equal(SourceText.Normalise("a\r\nb\rc"), SourceText.Normalise("a\nb\nc"));
    }

    [Fact]
    public void Fingerprint_ignores_line_endings_but_not_content()
    {
        Assert.Equal(SourceText.Fingerprint("x\r\ny"), SourceText.Fingerprint("x\ny"));
        Assert.NotEqual(SourceText.Fingerprint("x"), SourceText.Fingerprint("y"));
    }

    [Fact]
    public void Fingerprint_is_stable_across_processes()
    {
        // FNV-1a of "hello" - a fixed value; if this changes, every student's saved work is
        // silently discarded on their next visit.
        Assert.Equal("a430d84680aabd0b", SourceText.Fingerprint("hello"));
    }
}
