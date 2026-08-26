List<Spark> sparks = new List<Spark>();

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    // 1. Launch one new spark here.
    for (int i = 0; i < sparks.Count; i++)
    {
        sparks[i].Move();
        sparks[i].Draw();
        // 2. Remove this spark here once it has fallen off the bottom.
    }
    // 3. Draw the spark count here.
}

Game.Run(Setup, Draw);

class Spark
{
    public float x, y, speedY;

    public Spark(float startX, float startY, float startSpeedY)
    {
        x = startX;
        y = startY;
        speedY = startSpeedY;
    }

    public void Move()
    {
        // 4. Pull this spark down a little, then move it.
    }

    public void Draw()
    {
        // 5. Draw this spark here.
    }
}
