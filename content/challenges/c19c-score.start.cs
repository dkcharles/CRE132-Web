double leftY = 140;
double rightY = 140;
double bx = 320;
double by = 180;
double speedX = 4;
double speedY = 5;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);

    if (Keys.IsDown(Key.W)) leftY = leftY - 6;
    if (Keys.IsDown(Key.S)) leftY = leftY + 6;
    if (leftY < 0) leftY = 0;
    if (leftY > 280) leftY = 280;

    if (Keys.IsDown(Key.Up)) rightY = rightY - 6;
    if (Keys.IsDown(Key.Down)) rightY = rightY + 6;
    if (rightY < 0) rightY = 0;
    if (rightY > 280) rightY = 280;

    bx = bx + speedX;
    by = by + speedY;
    if (by < 12 || by > 348) speedY = -speedY;
    if (bx > 8 && bx < 48 && by > leftY - 12 && by < leftY + 92) speedX = -speedX;
    if (bx > 592 && bx < 632 && by > rightY - 12 && by < rightY + 92) speedX = -speedX;
    if (bx < 0 || bx > 640)
    {
        bx = 320;
        by = 180;
    }

    Screen.Rect(20, leftY, 16, 80, Colour.White);
    Screen.Rect(604, rightY, 16, 80, Colour.White);
    Screen.Circle(bx, by, 12, Colour.Yellow);
}

Game.Run(Setup, Draw);
