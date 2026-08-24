namespace CRE132.Engine;

// The one place the checking tolerance lives: the browser's Check button and the test suite's
// golden/solution gates must agree on what "matches" means, or a challenge could pass in CI
// and fail a student. Rule: CRLF normalised, every line's trailing whitespace trimmed, leading
// and trailing blank lines dropped - then exact per-line equality. `Hello` != `hello` is a
// lesson; an invisible trailing space is not.
public static class OutputComparer
{
    // null = match. Otherwise the 1-based index of the first differing line, counted in the
    // trimmed form (which is also how a verdict panel should display both sides).
    public static int? FirstDifference(string expected, string actual)
    {
        IReadOnlyList<string> e = Lines(expected);
        IReadOnlyList<string> a = Lines(actual);

        for (int i = 0; i < Math.Max(e.Count, a.Count); i++)
        {
            string left = i < e.Count ? e[i] : null!;
            string right = i < a.Count ? a[i] : null!;
            if (left != right) return i + 1;
        }
        return null;
    }

    public static IReadOnlyList<string> Lines(string text)
    {
        var lines = text.Replace("\r\n", "\n").Replace("\r", "\n")
                        .Split('\n')
                        .Select(l => l.TrimEnd())
                        .ToList();
        while (lines.Count > 0 && lines[0].Length == 0) lines.RemoveAt(0);
        while (lines.Count > 0 && lines[^1].Length == 0) lines.RemoveAt(lines.Count - 1);
        return lines;
    }
}
