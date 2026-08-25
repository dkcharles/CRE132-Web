double x = 100;
double y = 100;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Screen.Circle(x, y, 30, Colour.Cyan);
    Screen.Text(10, 10, $"x={x} y={y}", Colour.White);
}

Game.Run(Setup, Draw);
