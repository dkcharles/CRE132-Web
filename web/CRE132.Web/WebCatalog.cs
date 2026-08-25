namespace CRE132.Web;

public sealed record Entry(string Id, string Title, string NarrationFile);

// One band of the course. FirstId..LastId is the band's slice of the lesson numbering, so a
// part is defined by the numbers it owns rather than by a list that must be kept in step with
// Entries. PlannedTitles names the lessons of a part that is not written yet, one per id in the
// span; the home page shows a planned title only where no Entry exists for that id, so adding
// the row for lesson 20 turns its greyed title into a working link with no other change.
public sealed record Part(
    string Title,
    string Subtitle,
    int FirstId,
    int LastId,
    IReadOnlyList<string> PlannedTitles);

// What the site offers, in course order. One row per lesson; the contents list, the hash
// router and the pager all read this. Adding a lesson = one row + its markdown/samples.
public static class WebCatalog
{
    public static readonly IReadOnlyList<Entry> Entries = new[]
    {
        new Entry("0", "Welcome", "lessons/00-welcome.json"),
        new Entry("1", "Your first program", "lessons/01-first-program.json"),
        new Entry("2", "Variables and types", "lessons/02-variables.json"),
        new Entry("3", "Maths and operators", "lessons/03-maths-and-operators.json"),
        new Entry("4", "Reading input", "lessons/04-reading-input.json"),
        new Entry("5", "Making decisions", "lessons/05-making-decisions.json"),
        new Entry("6", "More decisions", "lessons/06-more-decisions.json"),
        new Entry("7", "Repetition", "lessons/07-repetition.json"),
        new Entry("8", "Loop patterns", "lessons/08-loop-patterns.json"),
        new Entry("9", "Methods", "lessons/09-methods.json"),
        new Entry("10", "Scope", "lessons/10-scope.json"),
        new Entry("11", "Collections", "lessons/11-collections.json"),
        new Entry("12", "Console project: The Snack Machine", "lessons/12-console-project.json"),
        new Entry("13", "First graphics", "lessons/13-first-graphics.json"),
        new Entry("14", "Motion", "lessons/14-motion.json"),
        new Entry("15", "The keyboard", "lessons/15-the-keyboard.json"),
        new Entry("16", "The mouse", "lessons/16-the-mouse.json"),
        new Entry("17", "Many things", "lessons/17-many-things.json"),
        new Entry("18", "Collision", "lessons/18-collision.json"),
        new Entry("19", "Mini-game: Pong", "lessons/19-pong.json"),
    };

    // Contiguous and non-overlapping: between them the parts cover 0..26 exactly once. Parts 1
    // and 2 are written, so they name no planned titles; part 3 is all planned titles and no rows.
    public static readonly IReadOnlyList<Part> Parts = new[]
    {
        new Part("Part 1 · Foundations", "Lessons 0–12", 0, 12, Array.Empty<string>()),
        new Part("Part 2 · Graphics & motion", "Lessons 13–19", 13, 19, Array.Empty<string>()),
        new Part("Part 3 · Objects & real games", "Coming soon", 20, 26, new[]
        {
            "Your first class",
            "Objects together",
            "Vectors",
            "Game state",
            "Animation & timing",
            "Mini-game: Snake",
            "Going further",
        }),
    };

    public static Entry? Find(string id) => Entries.FirstOrDefault(e => e.Id == id);

    // Catalog order, not numeric order: Entries is the single source of sequence, and the
    // contents list and pager already read it that way.
    public static IReadOnlyList<Entry> EntriesIn(Part p) => Entries
        .Where(e => int.TryParse(e.Id, out int n) && n >= p.FirstId && n <= p.LastId)
        .ToList();
}
