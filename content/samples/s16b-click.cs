double tx = 320;
double ty = 180;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    // WasClicked is true for one frame only, so the target sits still between clicks.
    if (Mouse.WasClicked)
    {
        tx = Mouse.X;
        ty = Mouse.Y;
    }
    Screen.Circle(tx, ty, 30, Colour.Orange);
    Screen.Text(10, 10, "Click to move the target", Colour.White);
}

Game.Run(Setup, Draw);
