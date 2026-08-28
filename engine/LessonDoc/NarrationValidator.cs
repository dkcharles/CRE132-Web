using CRE132.Engine;

namespace CRE132.LessonDoc;

// Everything a challenge needs, loaded by Program from content/challenges/<id>.*.
public sealed record ChallengeFiles(
    string Starter, IReadOnlyList<ChallengeCase> Cases,
    string Solution = "", string SolutionHtml = "", string? Hint = null);

// Checks every reference a document makes, and resolves what it can inline. Returning errors
// rather than throwing lets one run report every problem in a file instead of only the first.
public static class NarrationValidator
{
    public static (IReadOnlyList<Block> Resolved, IReadOnlyList<string> Errors) Validate(
        IReadOnlyList<Block> blocks,
        IReadOnlyDictionary<string, string> samples,      // id -> normalised source
        IReadOnlyDictionary<string, string> figures,      // name -> svg
        IReadOnlyDictionary<string, ChallengeFiles> challenges,
        IReadOnlyDictionary<string, string> sampleInputs, // id -> .in.txt contents
        IReadOnlySet<string>? failedChallenges = null)    // ids whose three files exist but whose kit failed to load
    {
        var errors = new List<string>();
        var resolved = new List<Block>(blocks.Count);

        foreach (Block b in blocks)
        {
            switch (b.Kind)
            {
                case "run" or "edit" when !samples.ContainsKey(b.Id!):
                    errors.Add($":::{b.Kind} names '{b.Id}', which has no file in content/samples.");
                    resolved.Add(b);
                    break;

                // Coloured here rather than in the browser: the payload ships no highlighter,
                // the same bargain that keeps Markdig out of it.
                case "run":
                    resolved.Add(b with
                    {
                        Code = CSharpHighlighter.Highlight(samples[b.Id!]),
                        Input = sampleInputs.TryGetValue(b.Id!, out string? runInput) ? runInput : null
                    });
                    break;

                // RAW, not highlighted: this seeds the editor buffer and is the baseline the
                // precompiled-vs-edited comparison and the save fingerprint both hang on.
                case "edit":
                    resolved.Add(b with
                    {
                        Code = samples[b.Id!],
                        Input = sampleInputs.TryGetValue(b.Id!, out string? editInput) ? editInput : null
                    });
                    break;

                // A failed-to-load id is reported by Program from ChallengeKit's own errors
                // already — repeating "missing one or more of ..." here would be false (the
                // files exist) and would bury the real reason under a decoy.
                case "challenge" when !challenges.ContainsKey(b.Id!) && failedChallenges is not null && failedChallenges.Contains(b.Id!):
                    errors.Add($":::challenge names '{b.Id}', which has errors — see the messages above.");
                    resolved.Add(b);
                    break;

                case "challenge" when !challenges.ContainsKey(b.Id!):
                    errors.Add($":::challenge names '{b.Id}', which is missing one or more of " +
                               $"content/challenges/{b.Id}.start.cs, {b.Id}.solution.cs, {b.Id}.cases.json.");
                    resolved.Add(b);
                    break;

                case "challenge" when challenges[b.Id!].Cases.Count == 0:
                    errors.Add($"challenge '{b.Id}' has no cases — nothing would be checked.");
                    resolved.Add(b);
                    break;

                case "challenge":
                    resolved.Add(b with
                    {
                        Code = challenges[b.Id!].Starter,
                        Cases = challenges[b.Id!].Cases,
                        Solution = challenges[b.Id!].Solution,
                        SolutionHtml = challenges[b.Id!].SolutionHtml,
                        Hint = challenges[b.Id!].Hint
                    });
                    break;

                case "figure" when !figures.ContainsKey(b.Name!):
                    errors.Add($":::figure names '{b.Name}', which is not in the figures folder.");
                    resolved.Add(b);
                    break;

                case "figure":
                    resolved.Add(b with { Svg = figures[b.Name!] });
                    break;

                default:
                    resolved.Add(b);
                    break;
            }
        }

        return (resolved, errors);
    }
}
