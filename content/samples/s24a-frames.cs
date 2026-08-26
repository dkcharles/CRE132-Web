List<float> radii = new List<float>();
List<Colour> tints = new List<Colour>();
int step = 0;
int framesPerStep = 6;

void Setup()
{
    Screen.Size(640, 360);
    radii.Add(16);
    radii.Add(28);
    radii.Add(40);
    radii.Add(28);
    tints.Add(Colour.Cyan);
    tints.Add(Colour.Green);
    tints.Add(Colour.Yellow);
    tints.Add(Colour.Orange);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    // Frame.Count is 0 on the very first Draw, so the first picture changes straight away.
    if (Frame.Count % framesPerStep == 0)
    {
        step = step + 1;
        // Past the end of the list, start the sequence again.
        if (step >= radii.Count) step = 0;
    }
    Screen.Circle(320, 180, radii[step], tints[step]);
    Screen.Text(10, 10, "Picture " + step + " of " + radii.Count, Colour.White);
}

Game.Run(Setup, Draw);
