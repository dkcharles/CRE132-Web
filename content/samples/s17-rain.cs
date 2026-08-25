List<double> xs = new List<double> { 40, 100, 160, 220, 280, 340, 400, 460, 520, 580 };
List<double> ys = new List<double> { 0, 36, 72, 108, 144, 180, 216, 252, 288, 324 };

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    // One index reaches into both lists: xs[i] and ys[i] are the same drop's two coordinates.
    for (int i = 0; i < xs.Count; i++)
    {
        Screen.Circle(xs[i], ys[i], 12, Colour.Cyan);
        ys[i] = ys[i] + 4;
        // Send a drop that has fallen past the bottom back to the top, so it rains forever.
        if (ys[i] > Screen.Height) ys[i] = 0;
    }
}

Game.Run(Setup, Draw);
