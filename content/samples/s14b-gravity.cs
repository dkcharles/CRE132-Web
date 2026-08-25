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
    speedY = speedY + gravity;
    y = y + speedY;
    // The floor: flip speedY when the ball reaches it and the fall becomes a rise.
    if (y > Screen.Height - radius) speedY = -speedY;
}

Game.Run(Setup, Draw);
