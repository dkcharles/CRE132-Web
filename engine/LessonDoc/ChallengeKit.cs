using System.Text.Json;
using CRE132.Engine;

namespace CRE132.LessonDoc;

// Loads content/challenges/<id>.* and turns it into ChallengeFiles, reporting every problem
// rather than the first. Game cases get their expected snapshots attached from <id>.frames.txt
// here, so the narration JSON carries everything the browser's checker needs.
public static class ChallengeKit
{
    static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };

    // `bootstrapping` is CRE132_UPDATE_GOLDENS=1: a game challenge with no frames.txt yet, or
    // with a frames.txt that no longer covers every case/frame the script now needs (widened
    // after the golden was last generated), is not fatal, because that variable is what
    // (re)generates the file in the first place (the content tests compile against the build
    // this runs in — see AUTHORING.md). Each becomes a "warning: "-prefixed entry in the
    // returned list instead, and the kit still loads, with Frames left null on any case whose
    // coverage was incomplete. Every other check (including a stray frames.txt on a non-game
    // kit) is still an error either way.
    public static (ChallengeFiles? Kit, IReadOnlyList<string> Errors) Load(
        string challengesDir, string id, bool bootstrapping = false)
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
        try { cases = LoadCases(casesFile, File.Exists(framesFile) ? framesFile : null, errors, bootstrapping); }
        catch (JsonException e) { errors.Add($"challenge '{id}': cases.json is not valid JSON: {e.Message}"); return (null, errors); }
        catch (FormatException e) { errors.Add($"challenge '{id}': {e.Message}"); return (null, errors); }

        bool anyGame = cases.Any(c => c.Game is not null);
        if (anyGame && !File.Exists(framesFile))
        {
            if (bootstrapping)
                errors.Add($"warning: challenge '{id}': no {id}.frames.txt yet — proceeding because " +
                    "CRE132_UPDATE_GOLDENS=1; run the content tests with the same variable set to generate it.");
            else
                errors.Add($"challenge '{id}': has a game case but no {id}.frames.txt — run the content tests with CRE132_UPDATE_GOLDENS=1 to generate it.");
        }
        if (!anyGame && File.Exists(framesFile))
            errors.Add($"challenge '{id}': {id}.frames.txt exists but no case has a game script.");

        // <id>.hint.md is optional in the kit (ContentTests insists on one for every shipped
        // challenge): a nudge in markdown, shown after the first failed Check. It goes through
        // the lesson pipeline, so a directive inside it would parse as one - refused here.
        string hintFile = Path.Combine(challengesDir, id + ".hint.md");
        string? hint = null;
        if (File.Exists(hintFile))
        {
            string markdown = SourceText.Normalise(File.ReadAllText(hintFile)).Trim();
            if (markdown.Length == 0)
                errors.Add($"challenge '{id}': {id}.hint.md is empty — write one nudge, or delete the file.");
            else if (markdown.Split('\n').Any(l => l.TrimStart().StartsWith(":::", StringComparison.Ordinal)))
                errors.Add($"challenge '{id}': {id}.hint.md contains a ::: directive — a hint is plain markdown.");
            else
                hint = NarrationParser.RenderMarkdown(markdown);
        }

        if (errors.Any(e => !e.StartsWith("warning: "))) return (null, errors);

        string solutionSource = SourceText.Normalise(File.ReadAllText(solution)).TrimEnd() + "\n";
        return (new ChallengeFiles(
                    SourceText.Normalise(File.ReadAllText(starter)).TrimEnd() + "\n", cases,
                    Solution: solutionSource,
                    SolutionHtml: CSharpHighlighter.Highlight(solutionSource),
                    Hint: hint),
                errors);
    }

    // Reads cases.json, validates every game script, and attaches snapshots from frames.txt
    // (when given). Throws JsonException/FormatException for unreadable files; script problems
    // are appended to `errors` so one run reports them all. When `bootstrapping` is true (see
    // Load above), a frames.txt that exists but doesn't yet cover a case — because the case or
    // one of its snapshot frames is new — reports a "warning: " instead of an error and leaves
    // that case's Frames null rather than failing the whole kit; the content tests regenerate
    // the file from the reference solution.
    public static IReadOnlyList<ChallengeCase> LoadCases(
        string casesFile, string? framesFile, List<string> errors, bool bootstrapping = false)
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
                string fileName = Path.GetFileName(framesFile);
                if (!frames.TryGetValue(i + 1, out IReadOnlyList<FrameSnapshot>? snaps))
                {
                    string message = $"{where}: no '=== case {i + 1} ===' section in {fileName}.";
                    errors.Add(bootstrapping
                        ? $"warning: {message} Proceeding because CRE132_UPDATE_GOLDENS=1 — run the " +
                          $"content tests with the same variable set to regenerate {fileName}, or delete it if in doubt."
                        : message);
                }
                else
                {
                    bool covered = true;
                    foreach (int n in g.Snapshot)
                        if (!snaps.Any(s => s.Frame == n))
                        {
                            covered = false;
                            string message = $"{where}: {fileName} has no '--- frame {n} ---' block — regenerate it.";
                            errors.Add(bootstrapping
                                ? $"warning: {message} Proceeding because CRE132_UPDATE_GOLDENS=1 — run the " +
                                  $"content tests with the same variable set to regenerate {fileName}, or delete it if in doubt."
                                : message);
                        }
                    // Fully covered: attach regardless of mode. Not covered: in normal mode the
                    // kit is dropped below anyway so attaching is harmless, but attach it too for
                    // parity with that fully-covered case; in bootstrapping mode leave Frames
                    // null so the caller can tell this case still needs (re)generating.
                    if (covered) cases[i] = cases[i] with { Frames = snaps };
                    else if (!bootstrapping) cases[i] = cases[i] with { Frames = snaps };
                }
            }
        }
        return cases;
    }
}
