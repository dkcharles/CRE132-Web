namespace CRE132.Game;

// Fixed step: there is no clock. Count is 0 on the first Draw; Time is seconds at 30 fps.
public static class Frame
{
    public const int Rate = 30;
    public static int Count => GameHost.State.FrameCount;
    public static double Time => Count / (double)Rate;
}
