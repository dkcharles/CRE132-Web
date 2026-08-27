using CRE132.Web;
using Xunit;

namespace CRE132.Tests;

// The deploy-race notice. A push replaces the fingerprinted lazy assemblies while a student
// has the old page open; their next Run fetches a file that no longer exists. The only cure
// is a reload, so the sentence must say exactly that - pinned here because the string lives
// in C# and is shown verbatim to beginners.
public class CompilerLoaderMessageTests
{
    [Fact]
    public void The_compiler_unavailable_message_tells_the_student_to_reload()
    {
        Assert.Equal("The site was updated a moment ago — reload this page and press Run again.",
            new CompilerUnavailableException().Message);
    }
}
