List<double> xs = new List<double>();
List<double> ys = new List<double>();

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
        if (ys[i] > 360)
        {
            xs.RemoveAt(i);
            ys.RemoveAt(i);
            i--;
        }
    }
    Screen.Text(10, 10, "drops: " + xs.Count, Colour.White);
}

Game.Run(Setup, Draw);
