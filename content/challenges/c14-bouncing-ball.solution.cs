double x = 320;
double y = 180;
double speedX = 4;
double speedY = 3;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Screen.Circle(x, y, 10, Colour.Yellow);
    x = x + speedX;
    y = y + speedY;
    if (x < 10 || x > 630) speedX = -speedX;
    if (y < 10 || y > 350) speedY = -speedY;
}

Game.Run(Setup, Draw);
