double px = 460;
double py = 100;
double cx = 520;
double cy = 100;
double playerSize = 30;
double playerSpeed = 5;
double coinRadius = 10;
int score = 0;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    if (Keys.IsDown(Key.Left)) px = px - playerSpeed;
    if (Keys.IsDown(Key.Right)) px = px + playerSpeed;
    if (Keys.IsDown(Key.Up)) py = py - playerSpeed;
    if (Keys.IsDown(Key.Down)) py = py + playerSpeed;
    // The player's square grown by the coin's radius on all four sides.
    if (cx > px - coinRadius && cx < px + playerSize + coinRadius &&
        cy > py - coinRadius && cy < py + playerSize + coinRadius)
    {
        cx = 100;
        cy = 300;
        score = score + 1;
    }
    Screen.Circle(cx, cy, coinRadius, Colour.Yellow);
    Screen.Rect(px, py, playerSize, playerSize, Colour.Cyan);
    Screen.Text(10, 10, "Score: " + score, Colour.White);
}

Game.Run(Setup, Draw);
