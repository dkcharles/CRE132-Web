void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Screen.Rect(0, 300, 640, 60, Colour.Green);
    Screen.Rect(220, 180, 200, 120, Colour.Orange);
    Screen.Rect(300, 240, 40, 60, Colour.Black);
    Screen.Line(220, 180, 320, 100, Colour.Red);
    Screen.Line(320, 100, 420, 180, Colour.Red);
}

Game.Run(Setup, Draw);
