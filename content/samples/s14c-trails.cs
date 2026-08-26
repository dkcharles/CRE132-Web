float x = 20;
float y = 20;
float speedX = 4;
float speedY = 3;
float radius = 12;

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
