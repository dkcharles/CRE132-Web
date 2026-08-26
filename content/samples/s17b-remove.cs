List<float> xs = new List<float>();
List<float> ys = new List<float>();

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    if (Frame.Count % 10 == 0)
    {
        xs.Add(Rand.Range(20, 620));
        ys.Add(0);
    }
    for (int i = 0; i < xs.Count; i++)
    {
        Screen.Circle(xs[i], ys[i], 12, Colour.Blue);
        ys[i] = ys[i] + 6;
        if (ys[i] > Screen.Height)
        {
            xs.RemoveAt(i);
            ys.RemoveAt(i);
            // Step the index back, or the drop that slid into the gap is skipped this frame.
            i--;
        }
    }
    Screen.Text(10, 10, "drops: " + xs.Count, Colour.White);
}

Game.Run(Setup, Draw);
