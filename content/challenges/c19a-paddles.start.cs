double leftY = 140;
double rightY = 140;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);

    // Move the left paddle with W and S here, then clamp leftY.

    // Move the right paddle with Up and Down here, then clamp rightY.

    Screen.Rect(20, leftY, 16, 80, Colour.White);
    Screen.Rect(604, rightY, 16, 80, Colour.White);
}

Game.Run(Setup, Draw);
