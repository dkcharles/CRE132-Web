namespace CRE132.Engine;

// One test case of a challenge: what goes into stdin, what stdout must show. Lives in Harness
// because the checker (eager, browser) and the generator (build tool) both consume it.
public sealed record ChallengeCase(string Input, string Expected);
