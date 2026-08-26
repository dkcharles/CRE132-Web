Vec2 position = new Vec2(320, 180);
float speed = 4;
float scaredWithin = 200;
float radius = 16;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Vec2 pointer = new Vec2(Mouse.X, Mouse.Y);
    // Position minus pointer points the other way: from the pointer towards the dot.
    Vec2 away = new Vec2(position.x - pointer.x, position.y - pointer.y);
    float distance = away.Length();
    // Nothing to normalise when the pointer is right on top of it: that arrow has no direction.
    if (distance > speed && distance < scaredWithin)
    {
        position = position.Add(away.Normalised().Scale(speed));
    }
    Screen.Circle(pointer.x, pointer.y, 12, Colour.Grey);
    Screen.Circle(position.x, position.y, radius, Colour.Pink);
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
