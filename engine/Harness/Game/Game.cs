namespace CRE132.Game;

// The one call that turns a program into a game. It registers and RETURNS: Main finishes, then
// the host runs Setup once and Draw every frame. Nothing here blocks, which is what keeps a
// single-threaded WebAssembly page alive.
public static class Game
{
    public static void Run(Action setup, Action draw)
    {
        if (setup is null || draw is null)
            throw new ArgumentException("Game.Run needs two methods: Game.Run(Setup, Draw).");
        if (GameHost.State.Draw is not null)
            throw new InvalidOperationException("Game.Run was called twice — call it once, at the end of your program.");
        GameHost.State.Setup = setup;
        GameHost.State.Draw = draw;
    }
}
