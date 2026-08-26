List<Shot> shots = new List<Shot>();
float shipTop = 340;
int cooldown = 0;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    cooldown = cooldown - 1;
    if (Keys.IsDown(Key.Space) && cooldown <= 0)
    {
        shots.Add(new Shot(320, shipTop));
        // Wind the timer back up, or the next frame fires again while the key is still down.
        cooldown = 15;
    }
    for (int i = 0; i < shots.Count; i++)
    {
        shots[i].Move();
        shots[i].Draw();
        if (shots[i].y < -shots[i].radius)
        {
            shots.RemoveAt(i);
            // Step the index back, or the shot that slid into the gap is skipped this frame.
            i--;
        }
    }
    Screen.Rect(290, shipTop, 60, 20, Colour.Cyan);
    Screen.Text(10, 10, "Shots: " + shots.Count, Colour.White);
}

Game.Run(Setup, Draw);

class Shot
{
    public float x, y;
    public float speed = 6;
    public float radius = 12;

    public Shot(float startX, float startY)
    {
        x = startX;
        y = startY;
    }

    public void Move()
    {
        y = y - speed;
    }

    public void Draw()
    {
        Screen.Circle(x, y, radius, Colour.Orange);
    }
}
