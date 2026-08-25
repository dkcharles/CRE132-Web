void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Screen.Circle(Mouse.X, Mouse.Y, 20, Colour.Cyan);
    Screen.Text(10, 10, $"x={Mouse.X} y={Mouse.Y}", Colour.White);
}

Game.Run(Setup, Draw);
