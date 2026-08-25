double x = 300;
double y = 160;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    if (Keys.IsDown(Key.Left)) x = x - 5;
    if (Keys.IsDown(Key.Right)) x = x + 5;
    if (Keys.IsDown(Key.Up)) y = y - 5;
    if (Keys.IsDown(Key.Down)) y = y + 5;
    Screen.Rect(x, y, 40, 40, Colour.Cyan);
}

Game.Run(Setup, Draw);
