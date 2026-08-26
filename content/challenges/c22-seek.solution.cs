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
    Vec2 toTarget = new Vec2(target.x - seeker.x, target.y - seeker.y);
    if (toTarget.Length() > speed)
    {
        seeker = seeker.Add(toTarget.Normalised().Scale(speed));
    }
    Screen.Circle(target.x, target.y, 12, Colour.Red);
    Screen.Circle(seeker.x, seeker.y, 14, Colour.Yellow);
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
