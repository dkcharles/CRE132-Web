double x = 320;
double y = 180;
double speedX = 4;
double speedY = 3;
double radius = 20;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Screen.Circle(x, y, radius, Colour.Yellow);
    x = x + speedX;
    y = y + speedY;
    // The ball is drawn from its centre, so turn it round one radius in from each edge.
    if (x < radius || x > Screen.Width - radius) speedX = -speedX;
    if (y < radius || y > Screen.Height - radius) speedY = -speedY;
}

Game.Run(Setup, Draw);
