int betweenFlashes = 30;
int cooldown = betweenFlashes;
int flashes = 0;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    cooldown = cooldown - 1;
    if (cooldown <= 0)
    {
        flashes = flashes + 1;
        // Wind the timer straight back up, or it would fire again on every frame after this one.
        cooldown = betweenFlashes;
    }
    Screen.Text(10, 10, "Next flash in: " + cooldown, Colour.White);
    Screen.Text(10, 40, "Flashes so far: " + flashes, Colour.White);
    // On screen only for the five frames after the timer fired.
    if (cooldown > betweenFlashes - 5) Screen.Circle(320, 200, 40, Colour.Yellow);
}

Game.Run(Setup, Draw);
