void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Blue);
    Screen.Circle(320, 180, 40, Colour.Yellow);
}

Game.Run(Setup, Draw);
