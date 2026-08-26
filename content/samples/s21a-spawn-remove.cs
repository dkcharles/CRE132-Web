List<Drop> drops = new List<Drop>();

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    if (Frame.Count % 15 == 0)
    {
        drops.Add(new Drop(Rand.Range(20, 620), 0));
    }
    // An index loop, not foreach: a list may not be changed while a foreach is walking it.
    for (int i = 0; i < drops.Count; i++)
    {
        drops[i].Move();
        drops[i].Draw();
        if (drops[i].y > 370)
        {
            drops.RemoveAt(i);
            // Step the index back, or the drop that slid into the gap is skipped this frame.
            i--;
        }
    }
    Screen.Text(10, 10, "drops: " + drops.Count, Colour.White);
}

Game.Run(Setup, Draw);

class Drop
{
    public float x, y;
    public float speed = 6;

    public Drop(float startX, float startY)
    {
        x = startX;
        y = startY;
    }

    public void Move()
    {
        y = y + speed;
    }

    public void Draw()
    {
        Screen.Circle(x, y, 12, Colour.Cyan);
    }
}
