void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    // Later calls paint over earlier ones, which is why the horizon line shows on the ground.
    Screen.Rect(0, 300, 640, 60, Colour.Green);
    Screen.Circle(560, 70, 30, Colour.Yellow);
    Screen.Line(0, 300, 640, 300, Colour.White);
    Screen.Text(20, 20, "My first scene", Colour.White);
}

Game.Run(Setup, Draw);
