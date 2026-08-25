double circleX = 320;
double circleY = 180;
double radius = 40;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    double dx = Mouse.X - circleX;
    double dy = Mouse.Y - circleY;
    // Pythagoras: dx and dy are the short sides of a triangle, dist is the long one.
    double dist = Math.Sqrt(dx * dx + dy * dy);
    Screen.Line(circleX, circleY, Mouse.X, Mouse.Y, Colour.Grey);
    // Two circles touch once the gap between their centres drops below 40 + 30, their two radii.
    if (dist < 70) Screen.Circle(circleX, circleY, radius, Colour.Red);
    else Screen.Circle(circleX, circleY, radius, Colour.Green);
    Screen.Circle(Mouse.X, Mouse.Y, 30, Colour.Cyan);
    Screen.Text(10, 10, "distance: " + dist, Colour.White);
}

Game.Run(Setup, Draw);
