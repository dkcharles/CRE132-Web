namespace CRE132.Web;

// The deploy race. Every push republishes the lazy assemblies under new fingerprinted names,
// so a student who has had a page open across a deploy asks for files that no longer exist -
// and no retry can fix it, because the page itself is the stale part. Thrown instead of the
// raw HttpRequestException/JSException so the owners can tell this apart from "the wifi
// dropped", which the same catch filters already cover with a different sentence.
public sealed class CompilerUnavailableException : Exception
{
    public const string Notice = "The site was updated a moment ago — reload this page and press Run again.";

    public CompilerUnavailableException() : base(Notice) { }

    public CompilerUnavailableException(Exception inner) : base(Notice, inner) { }
}
