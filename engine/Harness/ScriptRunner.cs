using CRE132.Game;

namespace CRE132.Engine;

public sealed record ScriptResult(IReadOnlyList<FrameSnapshot> Snapshots, string Console, string? Error);

// Drives a headless session through a script and renders the requested frames. The checker and
// the content tests both use this, so a golden generated here is exactly what the browser's
// Check button will compare against.
public static class ScriptRunner
{
    public static ScriptResult Run(GameSession session, GameScript script)
    {
        if (!session.IsGame) throw new InvalidOperationException("ScriptRunner needs a game session.");
        var wanted = new HashSet<int>(script.Snapshot ?? Array.Empty<int>());
        var keys = (script.Keys ?? new Dictionary<string, string>())
            .Select(kv => (Key: ParseKey(kv.Key), Range: FrameRange.Parse(kv.Value))).ToList();
        FrameRange? mouseDown = script.Mouse?.Down is null ? null : FrameRange.Parse(script.Mouse.Down);
        int mouseX = script.Mouse?.X ?? 0, mouseY = script.Mouse?.Y ?? 0;

        var canvas = new TextCanvas(session.Width, session.Height);
        var snapshots = new List<FrameSnapshot>();
        for (int n = 1; n <= script.Frames; n++)
        {
            var down = new HashSet<Key>(keys.Where(k => k.Range.Contains(n)).Select(k => k.Key));
            var input = new InputState(down, mouseX, mouseY, mouseDown?.Contains(n) ?? false);
            FrameResult f = session.Step(input);
            if (f.Error is not null) return new ScriptResult(snapshots, f.Console, f.Error);

            int cols = (session.Width + TextCanvas.Cell - 1) / TextCanvas.Cell;
            int rows = (session.Height + TextCanvas.Cell - 1) / TextCanvas.Cell;
            if (canvas.Columns != cols || canvas.Rows != rows)
                canvas = new TextCanvas(session.Width, session.Height);   // Screen.Size changed: fresh grid
            canvas.Apply(f.Commands);
            if (wanted.Contains(n)) snapshots.Add(new FrameSnapshot(n, canvas.Text));
        }
        return new ScriptResult(snapshots, session.ConsoleText, null);
    }

    // Enum.TryParse also accepts the underlying numeric value as a string (e.g. "3"), silently
    // succeeding even when that number happens to land on a real member's ordinal (3 is Down
    // here) — Enum.IsDefined alone doesn't catch that case, so numeric input is rejected outright.
    public static Key ParseKey(string name) =>
        Enum.TryParse(name, ignoreCase: false, out Key key) && Enum.IsDefined(key) && !int.TryParse(name, out _)
            ? key
            : throw new FormatException($"'{name}' is not a key name — use Left, Right, Up, Down, Space, Enter, Escape, A–Z or D0–D9.");
}
