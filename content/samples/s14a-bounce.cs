float x = 320;
float y = 180;
float speedX = 4;
float speedY = 3;
float radius = 20;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Screen.Circle(x, y, radius, Colour.Cyan);
    x = x + speedX;
    y = y + speedY;
    // The circle is drawn from its centre, so turn it round one radius in from each edge.
    if (x < radius || x > Screen.Width - radius) speedX = -speedX;
    if (y < radius || y > Screen.Height - radius) speedY = -speedY;
}

Game.Run(Setup, Draw);
