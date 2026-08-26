List<float> xs = new List<float> { 40, 100, 160, 220, 280, 340, 400, 460, 520, 580 };
List<float> ys = new List<float> { 0, 36, 72, 108, 144, 180, 216, 252, 288, 324 };

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
        // Recycled rather than removed, which is what makes it rain forever instead of once.
        if (ys[i] > Screen.Height) ys[i] = 0;
    }
}

Game.Run(Setup, Draw);
