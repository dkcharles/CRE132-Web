double x = 270;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    if (Keys.IsDown(Key.Left)) x = x - 6;
    if (Keys.IsDown(Key.Right)) x = x + 6;
    if (x < 0) x = 0;
    if (x > 540) x = 540;
    Screen.Rect(x, 330, 100, 16, Colour.White);
}

Game.Run(Setup, Draw);
