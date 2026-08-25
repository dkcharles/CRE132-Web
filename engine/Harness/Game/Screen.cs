namespace CRE132.Game;

// Pixels, (0,0) top-left, y down. Every call is recorded, not drawn: the browser canvas and the
// text renderer both consume the recording, which is what lets a game be tested without a browser.
public static class Screen
{
    public static int Width => GameHost.State.Width;
    public static int Height => GameHost.State.Height;

    public static void Size(int width, int height)
    {
        if (width < 1 || height < 1)
            throw new ArgumentException("Screen.Size needs a width and a height of at least 1 pixel.");
        GameHost.State.Width = width;
        GameHost.State.Height = height;
    }

    public static void Clear() => Clear(Colour.Black);
    public static void Clear(Colour colour) => Add(new DrawCommand(DrawKind.Clear, 0, 0, 0, 0, colour));
    public static void Rect(double x, double y, double width, double height, Colour colour) =>
        Add(new DrawCommand(DrawKind.Rect, x, y, width, height, colour));
    public static void Circle(double x, double y, double radius, Colour colour) =>
        Add(new DrawCommand(DrawKind.Circle, x, y, radius, 0, colour));
    public static void Line(double x1, double y1, double x2, double y2, Colour colour) =>
        Add(new DrawCommand(DrawKind.Line, x1, y1, x2, y2, colour));
    public static void Text(double x, double y, string text, Colour colour) =>
        Add(new DrawCommand(DrawKind.Text, x, y, 0, 0, colour, text ?? ""));

    static void Add(DrawCommand command) => GameHost.State.Frame?.Add(command);
}
