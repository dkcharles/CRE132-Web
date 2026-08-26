Ball ball = new Ball(320, 180, 4, 3);

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    ball.Move();
    // 2. Replace the line below with ball.Bounce(); and ball.Draw();
    Screen.Circle(ball.x, ball.y, ball.radius, Colour.Yellow);
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

    // 1. Add the Bounce() and Draw() methods here.
}
