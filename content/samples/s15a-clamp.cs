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
    if (x < 0) x = 0;
    if (x > Screen.Width - 40) x = Screen.Width - 40;
    if (y < 0) y = 0;
    if (y > Screen.Height - 40) y = Screen.Height - 40;
    Screen.Rect(x, y, 40, 40, Colour.Green);
    Screen.Text(10, 10, $"x={x} y={y}", Colour.White);
}

Game.Run(Setup, Draw);
