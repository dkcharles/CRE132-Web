double leftY = 140;
double rightY = 140;
double bx = 320;
double by = 180;
double speedX = 4;
double speedY = 5;
int left = 0;
int right = 0;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);

    // A rectangle is drawn from its top edge, so the lowest a paddle may sit is 360 - 80 = 280.
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
    // A paddle hit is the collision lesson's box grown by the ball's radius: a paddle at x = 20,
    // 16 wide and 80 tall, grown by 12, is 20 - 12 = 8 to 20 + 16 + 12 = 48 and 80 + 12 = 92 tall.
    if (bx > 8 && bx < 48 && by > leftY - 12 && by < leftY + 92) speedX = -speedX;
    if (bx > 592 && bx < 632 && by > rightY - 12 && by < rightY + 92) speedX = -speedX;
    // A miss: the ball is put back in the middle and the player at the other end scores.
    if (bx < 0)
    {
        bx = 320;
        by = 180;
        right = right + 1;
    }
    if (bx > 640)
    {
        bx = 320;
        by = 180;
        left = left + 1;
    }

    Screen.Rect(20, leftY, 16, 80, Colour.White);
    Screen.Rect(604, rightY, 16, 80, Colour.White);
    Screen.Circle(bx, by, 12, Colour.Yellow);
    // Drawn last, so the score sits on top of everything else rather than under the ball.
    Screen.Text(300, 10, left + " : " + right, Colour.White);
}

Game.Run(Setup, Draw);
