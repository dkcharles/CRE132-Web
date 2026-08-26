State state = State.Title;
float startY = 180;
float ballY = startY;
float speedY = 0;
float radius = 20;
int startFrame = 0;
int seconds = 0;

void Setup()
{
    Screen.Size(640, 360);
}

// Everything a fresh round needs, in one place: the ball back in the middle, the clock re-zeroed.
void Reset()
{
    ballY = startY;
    speedY = -3;
    startFrame = Frame.Count;
}

void Draw()
{
    Screen.Clear(Colour.Black);
    switch (state)
    {
        case State.Title:
            Screen.Text(130, 170, "SPACE TO PLAY", Colour.White);
            if (Keys.WasPressed(Key.Space))
            {
                Reset();
                state = State.Playing;
            }
            break;
        case State.Playing:
            if (Keys.WasPressed(Key.Space)) speedY = -6;
            speedY = speedY + 0.2f;
            ballY = ballY + speedY;
            // Frames since the round began, turned into whole seconds.
            seconds = (Frame.Count - startFrame) / 30;
            Screen.Circle(320, ballY, radius, Colour.Yellow);
            Screen.Text(10, 10, "Seconds: " + seconds, Colour.White);
            if (ballY > Screen.Height + radius) state = State.GameOver;
            break;
        case State.GameOver:
            Screen.Text(190, 130, "GAME OVER", Colour.White);
            Screen.Text(160, 170, "SECONDS: " + seconds, Colour.White);
            Screen.Text(130, 210, "ENTER FOR TITLE", Colour.White);
            if (Keys.WasPressed(Key.Enter)) state = State.Title;
            break;
    }
}

Game.Run(Setup, Draw);

enum State { Title, Playing, GameOver }
