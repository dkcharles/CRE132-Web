double tx = 320;
double ty = 180;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    // When the mouse is clicked, move the target to where the mouse is.
    Screen.Circle(tx, ty, 20, Colour.Yellow);
}

Game.Run(Setup, Draw);
