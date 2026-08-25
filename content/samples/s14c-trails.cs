double x = 20;
double y = 20;
double speedX = 4;
double speedY = 3;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Circle(x, y, 12, Colour.Pink);
    x = x + speedX;
    y = y + speedY;
    if (x < 12 || x > 628) speedX = -speedX;
    if (y < 12 || y > 348) speedY = -speedY;
}

Game.Run(Setup, Draw);
