double y = 60;
double speedY = 0;
double gravity = 0.5;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Screen.Circle(320, y, 20, Colour.Orange);
    speedY = speedY + gravity;
    y = y + speedY;
    if (y > 340) speedY = -speedY;
}

Game.Run(Setup, Draw);
