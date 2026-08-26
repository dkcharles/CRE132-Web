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
        // Both lists must grow on the same frame, or drop i loses the partner it is paired with.
        xs.Add(Rand.Range(20, 620));
        ys.Add(0);
    }
    for (int i = 0; i < xs.Count; i++)
    {
        Screen.Circle(xs[i], ys[i], 12, Colour.Cyan);
        ys[i] = ys[i] + 4;
    }
    Screen.Text(10, 10, "drops: " + xs.Count, Colour.White);
}

Game.Run(Setup, Draw);
