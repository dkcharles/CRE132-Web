namespace CRE132.Game;

// A colour is three bytes. Named colours are the beginner path; Rgb is the escape hatch.
public readonly record struct Colour(byte R, byte G, byte B)
{
    public static Colour Rgb(int r, int g, int b) =>
        new((byte)Math.Clamp(r, 0, 255), (byte)Math.Clamp(g, 0, 255), (byte)Math.Clamp(b, 0, 255));

    public static readonly Colour Black  = new(0, 0, 0);
    public static readonly Colour White  = new(255, 255, 255);
    public static readonly Colour Grey   = new(128, 128, 128);
    public static readonly Colour Red    = new(224, 73, 47);
    public static readonly Colour Orange = new(247, 179, 43);
    public static readonly Colour Yellow = new(240, 196, 25);
    public static readonly Colour Green  = new(76, 175, 80);
    public static readonly Colour Cyan   = new(58, 208, 224);
    public static readonly Colour Blue   = new(66, 133, 244);
    public static readonly Colour Purple = new(156, 39, 176);
    public static readonly Colour Pink   = new(233, 30, 99);
}
