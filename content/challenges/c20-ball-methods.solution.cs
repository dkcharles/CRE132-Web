Ball ball = new Ball(320, 180, 4, 3);

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    ball.Move();
    ball.Bounce();
    ball.Draw();
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
    }

    public void Bounce()
    {
        // A ball is drawn from its centre, so it turns round one radius in from each edge.
        if (x < radius || x > Screen.Width - radius) speedX = -speedX;
        if (y < radius || y > Screen.Height - radius) speedY = -speedY;
    }

    public void Draw()
    {
        Screen.Circle(x, y, radius, Colour.Yellow);
    }
}
