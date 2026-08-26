State state = State.Title;
float ballY = 180;
float speed = 3;
float radius = 20;

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
            Screen.Text(180, 170, "PRESS SPACE", Colour.White);
            if (Keys.WasPressed(Key.Space)) state = State.Playing;
            break;
        case State.Playing:
            ballY = ballY + speed;
            Screen.Circle(320, ballY, radius, Colour.Yellow);
            // One radius past the bottom it is out of sight, and nothing brings it back.
            if (ballY > Screen.Height + radius) state = State.GameOver;
            break;
        case State.GameOver:
            Screen.Text(190, 150, "GAME OVER", Colour.White);
            Screen.Text(130, 200, "ENTER FOR TITLE", Colour.White);
            if (Keys.WasPressed(Key.Enter)) state = State.Title;
            break;
    }
}

Game.Run(Setup, Draw);

enum State { Title, Playing, GameOver }
