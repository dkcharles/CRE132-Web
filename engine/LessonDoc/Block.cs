using CRE132.Engine;

namespace CRE132.LessonDoc;

// One flat record with a Kind discriminator rather than a type hierarchy. The consumer is a
// Blazor component walking a list and switching on Kind; polymorphic JSON would add
// converters and buy nothing. Nulls are omitted from the JSON, so each block carries only
// the fields its kind uses.
//
// Code's meaning depends on Kind, deliberately: for "run" it is build-time-highlighted HTML
// (the browser ships no highlighter); for "edit" and "challenge" it is RAW normalised source,
// because it seeds an editor buffer and must be byte-comparable to what shipped.
public sealed record Block(
    string Kind,              // prose | callout | figure | run | edit | challenge
    string? Html = null,      // prose, callout, challenge (the task statement)
    string? Id = null,        // run, edit (sample id), challenge (challenge id)
    string? Name = null,      // figure
    string? Caption = null,   // run, edit, figure
    string? Variant = null,   // callout: "try" | "key"
    string? Svg = null,       // figure, inlined by the validator
    string? Code = null,      // see above
    IReadOnlyList<ChallengeCase>? Cases = null,   // challenge
    string? Input = null);    // run, edit: prefill for the input panel; what Console.ReadLine reads unless the student edits it

// A malformed directive, reported with the line it is on so the build message is actionable.
public sealed class NarrationException(int line, string message)
    : Exception($"line {line}: {message}")
{
    public int Line { get; } = line;
}
