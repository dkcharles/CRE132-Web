double x = 20;
double y = 20;
double speedX = 4;
double speedY = 3;
double radius = 12;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    // No Screen.Clear on purpose: every circle ever drawn stays, so the path shows as a trail.
    Screen.Circle(x, y, radius, Colour.Pink);
    x = x + speedX;
    y = y + speedY;
    if (x < radius || x > Screen.Width - radius) speedX = -speedX;
    if (y < radius || y > Screen.Height - radius) speedY = -speedY;
}

Game.Run(Setup, Draw);
