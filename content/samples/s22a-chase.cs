Vec2 position = new Vec2(320, 180);
float speed = 4;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Vec2 target = new Vec2(Mouse.X, Mouse.Y);
    // Target minus position is the arrow from here to there: direction and distance in one.
    Vec2 toTarget = new Vec2(target.X - position.X, target.Y - position.Y);
    if (toTarget.Length() > speed)
    {
        position = position.Add(toTarget.Normalised().Scale(speed));
    }
    Screen.Circle(target.X, target.Y, 12, Colour.Grey);
    Screen.Circle(position.X, position.Y, 16, Colour.Yellow);
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
