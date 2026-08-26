Vec2 corner = new Vec2(3, 4);
Console.WriteLine("length: " + corner.Length());

Vec2 direction = corner.Normalised();
Console.WriteLine("direction: " + direction.X + ", " + direction.Y);
Console.WriteLine("its length: " + direction.Length());

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
        // Dividing both parts by the length keeps the direction and throws the size away.
        return new Vec2(X / length, Y / length);
    }
}
