List<double> xs = new List<double>();
List<double> ys = new List<double>();

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    // 1. Spawn one new star here.
    for (int i = 0; i < xs.Count; i++)
    {
        Screen.Circle(xs[i], ys[i], 12, Colour.White);
        ys[i] = ys[i] + 4;
        // 2. Remove this star here once it has fallen off the bottom.
    }
    // 3. Draw the star count here.
}

Game.Run(Setup, Draw);
