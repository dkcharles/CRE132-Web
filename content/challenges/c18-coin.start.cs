double px = 460;
double py = 100;
double cx = 520;
double cy = 100;
int score = 0;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    if (Keys.IsDown(Key.Left)) px = px - 5;
    if (Keys.IsDown(Key.Right)) px = px + 5;
    if (Keys.IsDown(Key.Up)) py = py - 5;
    if (Keys.IsDown(Key.Down)) py = py + 5;
    // When the player touches the coin, move the coin and score a point.
    Screen.Circle(cx, cy, 10, Colour.Yellow);
    Screen.Rect(px, py, 30, 30, Colour.Cyan);
    Screen.Text(10, 10, "Score: " + score, Colour.White);
}

Game.Run(Setup, Draw);
