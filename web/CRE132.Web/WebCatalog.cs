namespace CRE132.Web;

public sealed record Entry(string Id, string Title, string NarrationFile);

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
    };

    public static Entry? Find(string id) => Entries.FirstOrDefault(e => e.Id == id);
}
