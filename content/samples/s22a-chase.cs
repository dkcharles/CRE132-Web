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
    Vec2 toTarget = new Vec2(target.x - position.x, target.y - position.y);
    if (toTarget.Length() > speed)
    {
        position = position.Add(toTarget.Normalised().Scale(speed));
    }
    Screen.Circle(target.x, target.y, 12, Colour.Grey);
    Screen.Circle(position.x, position.y, 16, Colour.Yellow);
}

Game.Run(Setup, Draw);

class Vec2
{
    public float x, y;

    public Vec2(float startX, float startY)
    {
        x = startX;
        y = startY;
    }

    public float Length()
    {
        return MathF.Sqrt(x * x + y * y);
    }

    public Vec2 Normalised()
    {
        float length = Length();
        return new Vec2(x / length, y / length);
    }

    public Vec2 Add(Vec2 other)
    {
        return new Vec2(x + other.x, y + other.y);
    }

    public Vec2 Scale(float amount)
    {
        return new Vec2(x * amount, y * amount);
    }
}
