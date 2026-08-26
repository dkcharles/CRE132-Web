List<Star> stars = new List<Star>();

void Setup()
{
    Screen.Size(640, 360);
    for (int i = 0; i < 6; i++)
    {
        stars.Add(new Star(60 + i * 100, i * 60, 2 + i, 12 + i * 4));
    }
}

void Draw()
{
    Screen.Clear(Colour.Black);
    foreach (Star star in stars)
    {
        star.Move();
        star.Draw();
    }
}

Game.Run(Setup, Draw);

class Star
{
    public float x, y, speed, size;

    public Star(float startX, float startY, float startSpeed, float startSize)
    {
        x = startX;
        y = startY;
        speed = startSpeed;
        size = startSize;
    }

    public void Move()
    {
        y = y + speed;
        // Back to the top once it is completely past the bottom, so the sky never empties.
        if (y > Screen.Height + size) y = -size;
    }

    public void Draw()
    {
        Screen.Circle(x, y, size, Colour.Cyan);
    }
}
