namespace CRE132.Web;

// How much of a program's output a console pane shows. A `for` loop with one digit wrong
// prints tens of thousands of lines, and rendering all of them freezes the page long after
// the program itself has finished - so a pane shows the LAST 200 (the end is where the
// interesting part of a runaway loop is) with a way to expand.
//
// Deliberately NOT OutputComparer.Lines: that one exists for MARKING and trims trailing
// whitespace and drops leading/trailing blank lines, which is the right tolerance for
// comparing and the wrong thing for displaying. This one shows what the program printed.
public static class ConsoleLines
{
    public const int Cap = 200;

    // Blank lines in the MIDDLE are kept - the program printed them. The one dropped is the
    // empty piece after a final newline: Console.WriteLine ends every line, so 300 WriteLines
    // split into 301 pieces, and "Show all 301 lines" for 300 lines printed is just wrong.
    public static IReadOnlyList<string> Lines(string text)
    {
        var lines = text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n').ToList();
        if (lines.Count > 1 && lines[^1].Length == 0) lines.RemoveAt(lines.Count - 1);
        return lines;
    }

    public static int Count(string text) => Lines(text).Count;

    public static bool IsCapped(string text) => Count(text) > Cap;

    // Joined back together, so a pane keeps its single <pre> and the browser keeps doing the
    // wrapping. An uncapped output round-trips to exactly what it printed.
    public static string Shown(string text, bool showAll)
    {
        IReadOnlyList<string> all = Lines(text);
        if (showAll || all.Count <= Cap) return string.Join("\n", all);
        return string.Join("\n", all.Skip(all.Count - Cap));
    }
}
