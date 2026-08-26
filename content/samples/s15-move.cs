float x = 300;
float y = 160;
float speed = 5;
float size = 40;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    // Four separate ifs, not else ifs, so holding two arrows at once moves the square diagonally.
    if (Keys.IsDown(Key.Left)) x = x - speed;
    if (Keys.IsDown(Key.Right)) x = x + speed;
    if (Keys.IsDown(Key.Up)) y = y - speed;
    if (Keys.IsDown(Key.Down)) y = y + speed;
    Screen.Rect(x, y, size, size, Colour.Cyan);
}

Game.Run(Setup, Draw);
