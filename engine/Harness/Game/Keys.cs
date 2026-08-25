namespace CRE132.Game;

public static class Keys
{
    public static bool IsDown(Key key) => GameHost.State.Current.Down.Contains(key);
    public static bool WasPressed(Key key) =>
        GameHost.State.Current.Down.Contains(key) && !GameHost.State.Previous.Down.Contains(key);
}
