List<Shot> shots = new List<Shot>();
float shipTop = 340;
// 1. Add the cooldown counter here.

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    // 2. Count the cooldown down by one here, every frame.
    // 3. Add the cooldown test to the if below, and wind the cooldown back up after firing.
    if (Keys.IsDown(Key.Space))
    {
        shots.Add(new Shot(320, shipTop));
    }
    for (int i = 0; i < shots.Count; i++)
    {
        shots[i].Move();
        shots[i].Draw();
        // 4. Remove this shot once it is above the top of the screen.
    }
    Screen.Rect(290, shipTop, 60, 20, Colour.Cyan);
    // 5. Draw the shot count here.
}

Game.Run(Setup, Draw);

class Shot
{
    public float x, y;
    public float speed = 6;

    public Shot(float startX, float startY)
    {
        x = startX;
        y = startY;
    }

    public void Move()
    {
        y = y - speed;
    }

    public void Draw()
    {
        Screen.Circle(x, y, 12, Colour.Orange);
    }
}
