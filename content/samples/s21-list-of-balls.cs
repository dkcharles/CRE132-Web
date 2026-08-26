List<Ball> balls = new List<Ball>();

void Setup()
{
    Screen.Size(640, 360);
    // Setup runs once, so the cast is built here rather than thirty times a second in Draw.
    for (int i = 0; i < 5; i++)
    {
        balls.Add(new Ball(80 + i * 120, 60 + i * 50, 3 + i, 2 + i));
    }
}

void Draw()
{
    Screen.Clear(Colour.Black);
    foreach (Ball ball in balls)
    {
        ball.Move();
        ball.Draw();
    }
}

Game.Run(Setup, Draw);

class Ball
{
    public float x, y, speedX, speedY;
    public float radius = 20;

    public Ball(float startX, float startY, float startSpeedX, float startSpeedY)
    {
        x = startX;
        y = startY;
        speedX = startSpeedX;
        speedY = startSpeedY;
    }

    public void Move()
    {
        x = x + speedX;
        y = y + speedY;
        if (x < radius || x > Screen.Width - radius) speedX = -speedX;
        if (y < radius || y > Screen.Height - radius) speedY = -speedY;
    }

    public void Draw()
    {
        Screen.Circle(x, y, radius, Colour.Yellow);
    }
}
