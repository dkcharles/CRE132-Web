double x = 300;
double y = 160;
double speed = 5;
double size = 40;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    if (Keys.IsDown(Key.Left)) x = x - speed;
    if (Keys.IsDown(Key.Right)) x = x + speed;
    if (Keys.IsDown(Key.Up)) y = y - speed;
    if (Keys.IsDown(Key.Down)) y = y + speed;
    // A square is drawn from its top-left corner, so the far edge is the screen less its size.
    if (x < 0) x = 0;
    if (x > Screen.Width - size) x = Screen.Width - size;
    if (y < 0) y = 0;
    if (y > Screen.Height - size) y = Screen.Height - size;
    Screen.Rect(x, y, size, size, Colour.Green);
    Screen.Text(10, 10, $"x={x} y={y}", Colour.White);
}

Game.Run(Setup, Draw);
