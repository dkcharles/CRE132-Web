namespace CRE132.Game;

// Everything one running game owns. GameSession (CRE132.Engine) creates one per run and installs
// it as GameHost.Active while student code executes, so two sessions on one page (a stage playing
// while the checker runs) never share a screen size, a frame count or a random sequence.
internal sealed class GameState
{
    public Action? Setup;
    public Action? Draw;
    public int Width = 640;
    public int Height = 360;
    public List<DrawCommand>? Frame;          // null outside Draw: Screen calls are then discarded
    public int FrameCount;                    // 0 on the first Draw
    public InputState Current = InputState.None;
    public InputState Previous = InputState.None;
    public Random Random;

    public GameState(int seed) => Random = new Random(seed);
}

internal static class GameHost
{
    public static GameState? Active;

    // Student code that touches the API outside a session (only possible from our own tests)
    // gets a throwaway state rather than a null reference.
    static readonly GameState Idle = new(0);
    public static GameState State => Active ?? Idle;
}
