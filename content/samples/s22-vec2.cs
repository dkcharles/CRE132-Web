Vec2 corner = new Vec2(3, 4);
Console.WriteLine("length: " + corner.Length());

Vec2 direction = corner.Normalised();
Console.WriteLine("direction: " + direction.x + ", " + direction.y);
Console.WriteLine("its length: " + direction.Length());

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
        // Dividing both parts by the length keeps the direction and throws the size away.
        return new Vec2(x / length, y / length);
    }
}
