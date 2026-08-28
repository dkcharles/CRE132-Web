using CRE132.Engine;

namespace CRE132.Web;

// Mirrors engine/LessonDoc/Block.cs. Deliberately a copy rather than a shared reference:
// LessonDoc pulls in Markdig, and referencing it from the web project would put a markdown
// parser in the payload - the one thing the build-time design exists to prevent.
// NarrationShapeTests pins the two together. Code's meaning depends on Kind: "run" carries
// build-time-highlighted HTML; "edit"/"challenge" carry RAW source for the editor buffer.
public sealed record Block(
    string Kind,
    string? Html = null,
    string? Id = null,
    string? Name = null,
    string? Caption = null,
    string? Variant = null,
    string? Svg = null,
    string? Code = null,
    IReadOnlyList<ChallengeCase>? Cases = null,
    string? Input = null,     // run, edit: prefill for the input panel; what Console.ReadLine reads unless the student edits it
    string? Solution = null,  // challenge: the reference solution, RAW (it can be copied into the editor)
    string? SolutionHtml = null, // challenge: the same solution highlighted, for showing after three failed Checks
    string? Hint = null);     // challenge: rendered <id>.hint.md, offered after the first failed Check; null when the kit has none
