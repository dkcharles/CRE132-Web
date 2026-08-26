float playerX = 100;
float playerY = 160;
float playerSize = 40;
float wallX = 400;
float wallY = 100;
float wallWidth = 40;
float wallHeight = 160;
float playerSpeed = 5;

bool Overlaps(float ax, float ay, float aw, float ah,
              float bx, float by, float bw, float bh)
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
    if (Keys.IsDown(Key.Left)) playerX = playerX - playerSpeed;
    if (Keys.IsDown(Key.Right)) playerX = playerX + playerSpeed;
    Screen.Rect(wallX, wallY, wallWidth, wallHeight, Colour.Grey);
    Screen.Rect(playerX, playerY, playerSize, playerSize, Colour.Cyan);
    if (Overlaps(playerX, playerY, playerSize, playerSize, wallX, wallY, wallWidth, wallHeight))
        Screen.Text(240, 40, "touching", Colour.White);
}

Game.Run(Setup, Draw);
