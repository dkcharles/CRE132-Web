Ball ball = new Ball();
ball.x = 320;
ball.y = 180;
ball.speedX = 4;
ball.speedY = 3;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    ball.Move();
    ball.Draw();
}

Game.Run(Setup, Draw);

class Ball
{
    public float x, y, speedX, speedY;
    public float radius = 20;

    public void Move()
    {
        x = x + speedX;
        y = y + speedY;
        // A ball is drawn from its centre, so it turns round one radius in from each edge.
        if (x < radius || x > Screen.Width - radius) speedX = -speedX;
        if (y < radius || y > Screen.Height - radius) speedY = -speedY;
    }

    public void Draw()
    {
        Screen.Circle(x, y, radius, Colour.Yellow);
    }
}
