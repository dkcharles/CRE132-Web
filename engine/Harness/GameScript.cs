namespace CRE132.Engine;

// A challenge case's scripted input for a game, straight from cases.json. Frame numbers are
// 1-based: frame 1 is the first Draw. Keys: name -> "a-b" or "a", held on every frame of the
// range. Mouse: one position for the whole case, optionally pressed over a range.
public sealed record MouseScript(int X = 0, int Y = 0, string? Down = null);

public sealed record GameScript(
    int Frames,
    IReadOnlyList<int>? Snapshot = null,
    IReadOnlyDictionary<string, string>? Keys = null,
    MouseScript? Mouse = null);

// The text-renderer grid after the n-th Draw, rows joined by '\n'.
public sealed record FrameSnapshot(int Frame, string Text);

public readonly record struct FrameRange(int First, int Last)
{
    public bool Contains(int frame) => frame >= First && frame <= Last;

    public static FrameRange Parse(string text)
    {
        string t = (text ?? "").Trim();
        string[] parts = t.Split('-');
        if (parts.Length is < 1 or > 2 || !int.TryParse(parts[0], out int first)
            || (parts.Length == 2 && !int.TryParse(parts[1], out _)))
            throw new FormatException($"'{text}' is not a frame range — use \"10-30\" or \"5\".");
        int last = parts.Length == 2 ? int.Parse(parts[1]) : first;
        if (first < 1 || last < first)
            throw new FormatException($"'{text}' is not a frame range — frames start at 1 and the first number must not exceed the second.");
        return new FrameRange(first, last);
    }
}
