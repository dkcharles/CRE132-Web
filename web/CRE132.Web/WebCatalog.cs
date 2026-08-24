namespace CRE132.Web;

public sealed record Entry(string Id, string Title, string NarrationFile);

// What the site offers, in course order. One row per lesson; the contents list, the hash
// router and the pager all read this. Adding a lesson = one row + its markdown/samples.
public static class WebCatalog
{
    public static readonly IReadOnlyList<Entry> Entries = new[]
    {
        new Entry("1", "Your first program", "lessons/01-first-program.json"),
    };

    public static Entry? Find(string id) => Entries.FirstOrDefault(e => e.Id == id);
}
