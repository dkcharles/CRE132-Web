namespace CRE132.Game;

// What the host knows about the keyboard and mouse for ONE frame. Edge detection (WasPressed,
// WasClicked) is computed by comparing two of these, so the state itself is level-only.
public sealed record InputState(IReadOnlySet<Key> Down, int MouseX, int MouseY, bool MouseDown)
{
    public static readonly InputState None = new(new HashSet<Key>(), 0, 0, false);
}
