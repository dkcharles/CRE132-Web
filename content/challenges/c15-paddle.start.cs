double x = 270;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    // Move the paddle with Left and Right here, then keep it on the screen.
    Screen.Rect(x, 330, 100, 16, Colour.White);
}

Game.Run(Setup, Draw);
