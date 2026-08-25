double y = 60;
double speedY = 0;
double gravity = 0.5;
double radius = 20;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Screen.Circle(320, y, radius, Colour.Orange);
    // Gravity is a speed that grows: adding to speedY every frame makes the fall get faster.
    speedY = speedY + gravity;
    y = y + speedY;
    if (y > 360 - radius) speedY = -speedY;
}

Game.Run(Setup, Draw);
