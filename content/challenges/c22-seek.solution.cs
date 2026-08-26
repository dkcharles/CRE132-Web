Vec2 seeker = new Vec2(100, 300);
Vec2 target = new Vec2(540, 60);
float speed = 4;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Vec2 toTarget = new Vec2(target.X - seeker.X, target.Y - seeker.Y);
    if (toTarget.Length() > speed)
    {
        seeker = seeker.Add(toTarget.Normalised().Scale(speed));
    }
    Screen.Circle(target.X, target.Y, 12, Colour.Red);
    Screen.Circle(seeker.X, seeker.Y, 14, Colour.Yellow);
}

Game.Run(Setup, Draw);

class Vec2
{
    public float X, Y;

    public Vec2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public float Length()
    {
        return MathF.Sqrt(X * X + Y * Y);
    }

    public Vec2 Normalised()
    {
        float length = Length();
        return new Vec2(X / length, Y / length);
    }

    public Vec2 Add(Vec2 other)
    {
        return new Vec2(X + other.X, Y + other.Y);
    }

    public Vec2 Scale(float amount)
    {
        return new Vec2(X * amount, Y * amount);
    }
}
