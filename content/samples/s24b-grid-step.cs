float cell = 20;
float x = 320;
float y = 180;
float stepX = 0;
float stepY = 0;
int framesPerStep = 6;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    // The arrows only choose a direction. The timer below decides when the square moves.
    if (Keys.IsDown(Key.Left))
    {
        stepX = -1;
        stepY = 0;
    }
    if (Keys.IsDown(Key.Right))
    {
        stepX = 1;
        stepY = 0;
    }
    if (Keys.IsDown(Key.Up))
    {
        stepX = 0;
        stepY = -1;
    }
    if (Keys.IsDown(Key.Down))
    {
        stepX = 0;
        stepY = 1;
    }
    // One whole cell every six frames, never a fraction of one.
    if (Frame.Count % framesPerStep == 0)
    {
        x = x + stepX * cell;
        y = y + stepY * cell;
    }
    Screen.Rect(x, y, cell, cell, Colour.Green);
}

Game.Run(Setup, Draw);
