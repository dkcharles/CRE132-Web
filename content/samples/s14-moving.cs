double x = 0;
double speed = 3;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Screen.Circle(x, 180, 20, Colour.Yellow);
    x = x + speed;
    if (x > Screen.Width) x = 0;
}

Game.Run(Setup, Draw);
