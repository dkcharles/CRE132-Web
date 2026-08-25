double playerX = 100;
double playerY = 160;
double playerSize = 40;
double wallX = 400;
double wallY = 100;
double wallWidth = 40;
double wallHeight = 160;

bool Overlaps(double ax, double ay, double aw, double ah,
              double bx, double by, double bw, double bh)
{
    return ax < bx + bw && ax + aw > bx && ay < by + bh && ay + ah > by;
}

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    if (Keys.IsDown(Key.Left)) playerX = playerX - 5;
    if (Keys.IsDown(Key.Right)) playerX = playerX + 5;
    Screen.Rect(wallX, wallY, wallWidth, wallHeight, Colour.Grey);
    Screen.Rect(playerX, playerY, playerSize, playerSize, Colour.Cyan);
    if (Overlaps(playerX, playerY, playerSize, playerSize, wallX, wallY, wallWidth, wallHeight))
        Screen.Text(240, 40, "touching", Colour.White);
}

Game.Run(Setup, Draw);
