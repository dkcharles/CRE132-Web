Ball first = new Ball(160, 100, 4, 3);
Ball second = new Ball(480, 260, -3, 4);

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    first.Move();
    first.Draw();
    second.Move();
    second.Draw();
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
