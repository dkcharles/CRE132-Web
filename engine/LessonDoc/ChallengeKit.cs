using System.Text.Json;
using CRE132.Engine;

namespace CRE132.LessonDoc;

// Loads content/challenges/<id>.* and turns it into ChallengeFiles, reporting every problem
// rather than the first. Game cases get their expected snapshots attached from <id>.frames.txt
// here, so the narration JSON carries everything the browser's checker needs.
public static class ChallengeKit
{
    static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };

    public static (ChallengeFiles? Kit, IReadOnlyList<string> Errors) Load(string challengesDir, string id)
    {
        var errors = new List<string>();
        string starter = Path.Combine(challengesDir, id + ".start.cs");
        string solution = Path.Combine(challengesDir, id + ".solution.cs");
        string casesFile = Path.Combine(challengesDir, id + ".cases.json");
        string framesFile = Path.Combine(challengesDir, id + ".frames.txt");

        if (!File.Exists(starter) || !File.Exists(solution) || !File.Exists(casesFile))
        {
            errors.Add($"challenge '{id}': needs {id}.start.cs, {id}.solution.cs and {id}.cases.json.");
            return (null, errors);
        }

        IReadOnlyList<ChallengeCase> cases;
        try { cases = LoadCases(casesFile, File.Exists(framesFile) ? framesFile : null, errors); }
        catch (JsonException e) { errors.Add($"challenge '{id}': cases.json is not valid JSON: {e.Message}"); return (null, errors); }
        catch (FormatException e) { errors.Add($"challenge '{id}': {e.Message}"); return (null, errors); }

        bool anyGame = cases.Any(c => c.Game is not null);
        if (anyGame && !File.Exists(framesFile))
            errors.Add($"challenge '{id}': has a game case but no {id}.frames.txt — run the content tests with CRE132_UPDATE_GOLDENS=1 to generate it.");
        if (!anyGame && File.Exists(framesFile))
            errors.Add($"challenge '{id}': {id}.frames.txt exists but no case has a game script.");

        if (errors.Count > 0) return (null, errors);
        return (new ChallengeFiles(SourceText.Normalise(File.ReadAllText(starter)).TrimEnd() + "\n", cases), errors);
    }

    // Reads cases.json, validates every game script, and attaches snapshots from frames.txt
    // (when given). Throws JsonException/FormatException for unreadable files; script problems
    // are appended to `errors` so one run reports them all.
    public static IReadOnlyList<ChallengeCase> LoadCases(string casesFile, string? framesFile, List<string> errors)
    {
        var cases = JsonSerializer.Deserialize<List<ChallengeCase>>(File.ReadAllText(casesFile), Json) ?? new();
        var frames = framesFile is null
            ? new Dictionary<int, IReadOnlyList<FrameSnapshot>>()
            : FramesFile.Parse(File.ReadAllText(framesFile));

        for (int i = 0; i < cases.Count; i++)
        {
            GameScript? g = cases[i].Game;
            if (g is null) continue;
            string where = $"case {i + 1}";

            if (g.Frames < 1) errors.Add($"{where}: game.frames must be at least 1 (was {g.Frames}).");
            if (g.Snapshot is null || g.Snapshot.Count == 0) errors.Add($"{where}: game.snapshot must name at least one frame.");
            else foreach (int n in g.Snapshot)
                if (n < 1 || n > g.Frames) errors.Add($"{where}: snapshot frame {n} is outside 1..{g.Frames} (game.frames).");
            foreach ((string name, string range) in g.Keys ?? new Dictionary<string, string>())
            {
                try { ScriptRunner.ParseKey(name); } catch (FormatException e) { errors.Add($"{where}: {e.Message}"); }
                try { FrameRange.Parse(range); } catch (FormatException e) { errors.Add($"{where}: key {name}: {e.Message}"); }
            }
            if (g.Mouse?.Down is not null)
                try { FrameRange.Parse(g.Mouse.Down); } catch (FormatException e) { errors.Add($"{where}: mouse.down: {e.Message}"); }

            if (framesFile is not null && g.Snapshot is not null)
            {
                if (!frames.TryGetValue(i + 1, out IReadOnlyList<FrameSnapshot>? snaps))
                    errors.Add($"{where}: no '=== case {i + 1} ===' section in {Path.GetFileName(framesFile)}.");
                else
                {
                    foreach (int n in g.Snapshot)
                        if (!snaps.Any(s => s.Frame == n))
                            errors.Add($"{where}: {Path.GetFileName(framesFile)} has no '--- frame {n} ---' block — regenerate it.");
                    cases[i] = cases[i] with { Frames = snaps };
                }
            }
        }
        return cases;
    }
}
