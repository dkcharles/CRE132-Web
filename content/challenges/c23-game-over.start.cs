// 1. Add the state variable here.
Ball ball = new Ball(320, 180, 4, 3);

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    // 2. Replace these two lines with the switch on state.
    ball.Move();
    ball.Draw();
}

Game.Run(Setup, Draw);

// 3. Add the enum here.

class Ball
{
    public float x, y, speedX, speedY;
    public float startX, startY;
    public float radius = 20;

    public Ball(float firstX, float firstY, float startSpeedX, float startSpeedY)
    {
        startX = firstX;
        startY = firstY;
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
        // 4. Change the line below so the ball bounces off the top only.
        if (y < radius || y > Screen.Height - radius) speedY = -speedY;
    }

    public void Draw()
    {
        Screen.Circle(x, y, radius, Colour.Yellow);
    }

    // Written for you: puts the ball back where the constructor started it.
    public void Reset()
    {
        x = startX;
        y = startY;
    }
}
