List<Spark> sparks = new List<Spark>();

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    if (Frame.Count % 10 == 0)
    {
        sparks.Add(new Spark(Rand.Range(100, 540), 360, -8));
    }
    for (int i = 0; i < sparks.Count; i++)
    {
        sparks[i].Move();
        sparks[i].Draw();
        if (sparks[i].y > 380)
        {
            sparks.RemoveAt(i);
            // Step the index back, or the spark that slid into the gap is skipped this frame.
            i--;
        }
    }
    Screen.Text(10, 10, "Sparks: " + sparks.Count, Colour.White);
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
        speedY = speedY + 0.3f;
        y = y + speedY;
    }

    public void Draw()
    {
        Screen.Circle(x, y, 12, Colour.Orange);
    }
}
