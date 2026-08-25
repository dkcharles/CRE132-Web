namespace CRE132.Game;

public static class Mouse
{
    public static int X => GameHost.State.Current.MouseX;
    public static int Y => GameHost.State.Current.MouseY;
    public static bool IsDown => GameHost.State.Current.MouseDown;
    public static bool WasClicked => GameHost.State.Current.MouseDown && !GameHost.State.Previous.MouseDown;
}
