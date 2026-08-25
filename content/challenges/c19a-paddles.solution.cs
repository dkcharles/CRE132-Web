double leftY = 140;
double rightY = 140;

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

    Screen.Rect(20, leftY, 16, 80, Colour.White);
    Screen.Rect(604, rightY, 16, 80, Colour.White);
}

Game.Run(Setup, Draw);
