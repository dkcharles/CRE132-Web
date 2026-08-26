List<Particle> sparks = new List<Particle>();
int burstSize = 40;
int betweenBursts = 90;

void Setup()
{
    Screen.Size(640, 360);
}

void Burst(float x, float y)
{
    for (int i = 0; i < burstSize; i++)
    {
        sparks.Add(new Particle(x, y));
    }
}

void Draw()
{
    Screen.Clear(Colour.Black);
    if (Mouse.WasClicked) Burst(Mouse.X, Mouse.Y);
    // One of its own every three seconds, so there is always something to watch.
    if (Frame.Count % betweenBursts == 0) Burst(320, 180);
    // An index loop, not foreach: a list may not be changed while a foreach is walking it.
    for (int i = 0; i < sparks.Count; i++)
    {
        sparks[i].Update();
        sparks[i].Draw();
        if (sparks[i].life <= 0)
        {
            sparks.RemoveAt(i);
            // Step the index back, or the spark that slid into the gap is skipped this frame.
            i--;
        }
    }
    Screen.Text(10, 10, "Sparks: " + sparks.Count, Colour.White);
}

Game.Run(Setup, Draw);

class Particle
{
    public float x, y;
    public float speedX, speedY;
    public float spread = 3.5f;
    public float gravity = 0.2f;
    public int fullLife = 75;
    public int life;
    public float bigRadius = 16;
    public float smallRadius = 12;

    public Particle(float startX, float startY)
    {
        x = startX;
        y = startY;
        life = fullLife;
        // Two calls to Rand per spark, in this order: sideways first, then up or down.
        speedX = Rand.Range(-spread, spread);
        speedY = Rand.Range(-spread, spread);
    }

    public void Update()
    {
        x = x + speedX;
        y = y + speedY;
        speedY = speedY + gravity;
        life = life - 1;
    }

    public void Draw()
    {
        // Full life draws 16 and no life draws 12: a spark fades by shrinking, but never below
        // 12, because a circle much smaller than that is hard to pick out at all.
        float radius = smallRadius + (bigRadius - smallRadius) * life / fullLife;
        Screen.Circle(x, y, radius, Colour.Orange);
    }
}
