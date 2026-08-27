namespace CRE132.Web;

// Every failure to load the compiler, whatever caused it, arrives as this one type: the
// owners' network/JS filters no longer see loader failures at all, because CompilerLoader
// retypes them here.
//
// The message names the deploy race on purpose, because that is the common cause. Every push
// republishes the lazy assemblies under new fingerprinted names, so a student who has had a
// page open across a deploy asks for files that no longer exist - and no retry can fix it,
// because the page itself is the stale part. A genuine dropped connection mid-download lands
// on the same sentence, and reads fine there too: reloading the page is what that student
// should do next as well.
public sealed class CompilerUnavailableException : Exception
{
    public const string Notice = "The site was updated a moment ago — reload this page and press Run again.";

    public CompilerUnavailableException() : base(Notice) { }

    public CompilerUnavailableException(Exception inner) : base(Notice, inner) { }
}
