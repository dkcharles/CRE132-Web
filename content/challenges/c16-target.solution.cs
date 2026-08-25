double tx = 320;
double ty = 180;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    if (Mouse.WasClicked)
    {
        tx = Mouse.X;
        ty = Mouse.Y;
    }
    Screen.Circle(tx, ty, 20, Colour.Yellow);
}

Game.Run(Setup, Draw);
