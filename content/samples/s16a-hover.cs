double bx = 240;
double by = 140;
double bw = 160;
double bh = 80;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    bool inside = Mouse.X > bx && Mouse.X < bx + bw && Mouse.Y > by && Mouse.Y < by + bh;
    if (inside) Screen.Rect(bx, by, bw, bh, Colour.Green);
    else Screen.Rect(bx, by, bw, bh, Colour.Grey);
    if (inside) Screen.Text(272, 172, "hover!", Colour.Black);
}

Game.Run(Setup, Draw);
