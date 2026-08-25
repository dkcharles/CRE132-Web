int count = 0;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    if (Keys.WasPressed(Key.Space)) count = count + 1;
    Screen.Text(180, 160, $"Presses: {count}", Colour.White);
}

Game.Run(Setup, Draw);
