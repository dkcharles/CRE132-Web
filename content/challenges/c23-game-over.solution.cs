State state = State.Title;
Ball ball = new Ball(320, 180, 4, 3);

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    switch (state)
    {
        case State.Title:
            ball.Draw();
            Screen.Text(220, 240, "Press SPACE", Colour.White);
            if (Keys.WasPressed(Key.Space)) state = State.Playing;
            break;
        case State.Playing:
            ball.Move();
            ball.Draw();
            // One radius past the bottom of the screen the ball is gone for good.
            if (ball.y > Screen.Height + ball.radius) state = State.GameOver;
            break;
        case State.GameOver:
            Screen.Text(240, 170, "GAME OVER", Colour.White);
            Screen.Text(130, 210, "ENTER FOR TITLE", Colour.White);
            if (Keys.WasPressed(Key.Enter))
            {
                ball.Reset();
                state = State.Title;
            }
            break;
    }
}

Game.Run(Setup, Draw);

enum State { Title, Playing, GameOver }

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
        if (y < radius) speedY = -speedY;
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
