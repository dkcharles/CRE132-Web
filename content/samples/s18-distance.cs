double bx = 320;
double by = 180;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    double dx = Mouse.X - bx;
    double dy = Mouse.Y - by;
    double dist = Math.Sqrt(dx * dx + dy * dy);
    Screen.Line(bx, by, Mouse.X, Mouse.Y, Colour.Grey);
    if (dist < 70) Screen.Circle(bx, by, 40, Colour.Red);
    else Screen.Circle(bx, by, 40, Colour.Green);
    Screen.Circle(Mouse.X, Mouse.Y, 30, Colour.Cyan);
    Screen.Text(10, 10, "distance: " + dist, Colour.White);
}

Game.Run(Setup, Draw);
