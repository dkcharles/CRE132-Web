double cx = 400;
double cy = 120;
int score = 0;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    double dx = Mouse.X - cx;
    double dy = Mouse.Y - cy;
    if (Math.Sqrt(dx * dx + dy * dy) < 40)
    {
        // Caught: the coin jumps to a new random spot and the score goes up.
        cx = Rand.Range(40, 600);
        cy = Rand.Range(40, 320);
        score = score + 1;
    }
    Screen.Circle(cx, cy, 15, Colour.Yellow);
    Screen.Circle(Mouse.X, Mouse.Y, 25, Colour.Cyan);
    Screen.Text(10, 10, "Caught: " + score, Colour.White);
}

Game.Run(Setup, Draw);
