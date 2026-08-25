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
    Screen.Circle(x, y, 20, Colour.Yellow);
    x = x + speedX;
    y = y + speedY;
    if (x < 20 || x > 620) speedX = -speedX;
    if (y < 20 || y > 340) speedY = -speedY;
}

Game.Run(Setup, Draw);
