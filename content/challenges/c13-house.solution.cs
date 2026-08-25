void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Screen.Rect(0, 300, 640, 60, Colour.Green);
    Screen.Rect(220, 180, 200, 120, Colour.Orange);
    Screen.Circle(320, 240, 16, Colour.Cyan);
    Screen.Line(220, 180, 320, 100, Colour.Red);
    Screen.Line(320, 100, 420, 180, Colour.Red);
}

Game.Run(Setup, Draw);
