float y = 60;
float speedY = 0;
float gravity = 0.5f;
float radius = 20;

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
    // The floor: put the ball back on it, then flip speedY so the fall becomes a rise. Setting y
    // matters as much as flipping speedY - a ball that bounces from wherever it sank to leaves
    // the floor lower and faster every time, and eventually falls straight through.
    if (y > Screen.Height - radius)
    {
        y = Screen.Height - radius;
        speedY = -speedY;
    }
}

Game.Run(Setup, Draw);
