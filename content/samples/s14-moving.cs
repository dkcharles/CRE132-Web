float x = 0;
float speed = 3;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Screen.Circle(x, 180, 20, Colour.Yellow);
    x = x + speed;
    // Wrap: once the circle has gone past the right edge, start it again at the left.
    if (x > Screen.Width) x = 0;
}

Game.Run(Setup, Draw);
