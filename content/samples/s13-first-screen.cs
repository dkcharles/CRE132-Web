void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Blue);
    // A circle is drawn from its centre, and (320, 180) is the middle of a 640 x 360 screen.
    Screen.Circle(320, 180, 40, Colour.Yellow);
}

Game.Run(Setup, Draw);
