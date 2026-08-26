Vec2 position = new Vec2(320, 180);
float speed = 4;
float scaredWithin = 200;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Vec2 pointer = new Vec2(Mouse.X, Mouse.Y);
    // Position minus pointer points the other way: from the pointer towards the dot.
    Vec2 away = new Vec2(position.X - pointer.X, position.Y - pointer.Y);
    float distance = away.Length();
    // Nothing to normalise when the pointer is right on top of it: that arrow has no direction.
    if (distance > speed && distance < scaredWithin)
    {
        position = position.Add(away.Normalised().Scale(speed));
    }
    Screen.Circle(pointer.X, pointer.Y, 12, Colour.Grey);
    Screen.Circle(position.X, position.Y, 16, Colour.Pink);
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
