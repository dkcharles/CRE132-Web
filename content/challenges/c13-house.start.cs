void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Screen.Rect(0, 300, 640, 60, Colour.Green);
    // Draw your house here: walls, then a window, then two roof lines.
}

Game.Run(Setup, Draw);
