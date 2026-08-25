namespace CRE132.Engine;

// One test case of a challenge. Console cases: stdin in, stdout expected. Game cases add a
// script and the expected frame snapshots (attached by the generator from <id>.frames.txt);
// Expected is then optional and compared only when non-empty. Defaults keep every existing
// cases.json deserialising unchanged.
public sealed record ChallengeCase(
    string Input = "",
    string Expected = "",
    GameScript? Game = null,
    IReadOnlyList<FrameSnapshot>? Frames = null);
