namespace CRE132.Web;

public sealed record Entry(string Id, string Title, string NarrationFile);

// One band of the course. FirstId..LastId is the band's slice of the lesson numbering, so a
// part is defined by the numbers it owns rather than by a list that must be kept in step with
// Entries.
//
// PlannedTitles is indexed BY POSITION IN THE SPAN: PlannedTitles[id - FirstId] announces that
// id. An Entry for the same id simply takes precedence, so writing lesson 20 means adding its
// row and LEAVING its planned title where it is - deleting the title instead would slide every
// later one up a slot and relabel 21 as "Vectors". WebCatalogTests holds the invariant that
// makes either mistake fail the build: every id in the span resolves to an Entry, or to a
// non-blank planned title at its own position.
//
// Subtitle overrides the "Lessons a-b" line the home page computes from the span. It is set
// only where a part needs to say something else: part 3 says "Coming soon" until lesson 20
// lands and then drops it, part 4 says what its showcase lessons are for and keeps saying it.
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
    // and 2 are fully written, so their spans need no planned titles and their subtitles are the
    // computed "Lessons a-b"; parts 3 and 4 are all planned titles and no rows. Part 3 ends on
    // Snake and part 4 holds the showcase lessons - 26 today, later ones after it - so a reader
    // sees where the taught course stops and the "read, run, tinker" material starts.
    public static readonly IReadOnlyList<Part> Parts = new[]
    {
        new Part("Part 1 · Foundations", "", 0, 12, Array.Empty<string>()),
        new Part("Part 2 · Graphics & motion", "", 13, 19, Array.Empty<string>()),
        new Part("Part 3 · Objects & real games", "Coming soon", 20, 25, new[]
        {
            "Your first class",
            "Objects together",
            "Vectors",
            "Game state",
            "Animation & timing",
            "Mini-game: Snake",
        }),
        new Part("Part 4 · Going further", "Study, run and try out", 26, 26, new[]
        {
            "Going further",
        }),
    };

    public static Entry? Find(string id) => Entries.FirstOrDefault(e => e.Id == id);

    // Catalog order, not numeric order: Entries is the single source of sequence, and the
    // contents list and pager already read it that way.
    public static IReadOnlyList<Entry> EntriesIn(Part p) => Entries
        .Where(e => int.TryParse(e.Id, out int n) && n >= p.FirstId && n <= p.LastId)
        .ToList();

    // The one way to read PlannedTitles: by the id's own position in the span, never by walking
    // the list. Empty means "nothing announced for this id" - fine when an Entry covers it.
    public static string PlannedTitle(Part p, int id)
    {
        int slot = id - p.FirstId;
        return slot >= 0 && slot < p.PlannedTitles.Count ? p.PlannedTitles[slot] : "";
    }
}
