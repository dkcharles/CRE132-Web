int screenWidth = 640;
int screenHeight = 360;
int groundY = 300;

void Setup()
{
    Screen.Size(screenWidth, screenHeight);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    // Later calls paint over earlier ones, which is why the horizon line shows on the ground.
    Screen.Rect(0, groundY, screenWidth, 60, Colour.Green);
    Screen.Circle(560, 70, 30, Colour.Yellow);
    Screen.Line(0, groundY, screenWidth, groundY, Colour.White);
    Screen.Text(20, 20, "My first scene", Colour.White);
}

Game.Run(Setup, Draw);
