using System.Text;

namespace CRE132.Engine;

// <id>.frames.txt: expected snapshots per case, generated from the reference solution and
// committed so an author reviews the grids in a diff. Rows are stored verbatim (trailing spaces
// included) so a grid reads as a picture; the comparer trims row ends anyway.
public static class FramesFile
{
    const string CasePrefix = "=== case ", FramePrefix = "--- frame ";

    public static string Format(IReadOnlyDictionary<int, IReadOnlyList<FrameSnapshot>> cases)
    {
        var sb = new StringBuilder();
        foreach (int index in cases.Keys.OrderBy(k => k))
        {
            sb.Append(CasePrefix).Append(index).Append(" ===\n");
            foreach (FrameSnapshot s in cases[index])
                sb.Append(FramePrefix).Append(s.Frame).Append(" ---\n").Append(s.Text).Append('\n');
        }
        return sb.ToString();
    }

    public static IReadOnlyDictionary<int, IReadOnlyList<FrameSnapshot>> Parse(string text)
    {
        var result = new Dictionary<int, IReadOnlyList<FrameSnapshot>>();
        string[] lines = text.Replace("\r\n", "\n").Split('\n');
        int? caseIndex = null, frame = null;
        var frames = new List<FrameSnapshot>();
        var rows = new List<string>();

        void CloseFrame()
        {
            if (frame is null) return;
            while (rows.Count > 0 && rows[^1].Length == 0) rows.RemoveAt(rows.Count - 1);   // file's final newline
            frames.Add(new FrameSnapshot(frame.Value, string.Join("\n", rows)));
            rows = new List<string>(); frame = null;
        }
        void CloseCase()
        {
            CloseFrame();
            if (caseIndex is null) return;
            result[caseIndex.Value] = frames;
            frames = new List<FrameSnapshot>(); caseIndex = null;
        }

        foreach (string line in lines)
        {
            if (line.StartsWith(CasePrefix, StringComparison.Ordinal))
            {
                CloseCase();
                caseIndex = int.Parse(line[CasePrefix.Length..].Replace("===", "").Trim());
            }
            else if (line.StartsWith(FramePrefix, StringComparison.Ordinal))
            {
                if (caseIndex is null) throw new FormatException("frames.txt must start with a '=== case N ===' header.");
                CloseFrame();
                frame = int.Parse(line[FramePrefix.Length..].Replace("---", "").Trim());
            }
            else if (frame is not null) rows.Add(line);
            else if (line.Trim().Length > 0) throw new FormatException($"frames.txt: unexpected text outside a frame: '{line}'.");
        }
        CloseCase();
        return result;
    }
}
