double px = 100;

bool Overlaps(double ax, double ay, double aw, double ah,
              double bx, double by, double bw, double bh)
{
    return ax < bx + bw && ax + aw > bx && ay < by + bh && ay + ah > by;
}

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    if (Keys.IsDown(Key.Left)) px = px - 5;
    if (Keys.IsDown(Key.Right)) px = px + 5;
    Screen.Rect(400, 100, 40, 160, Colour.Grey);
    Screen.Rect(px, 160, 40, 40, Colour.Cyan);
    if (Overlaps(px, 160, 40, 40, 400, 100, 40, 160))
        Screen.Text(240, 40, "touching", Colour.White);
}

Game.Run(Setup, Draw);
