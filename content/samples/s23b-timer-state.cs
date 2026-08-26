State state = State.Title;
float startX = 60;
float ballX = startX;
float speedX = 6;
float radius = 20;
int startFrame = 0;
int roundFrames = 300;

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
            Screen.Text(130, 170, "SPACE TO START", Colour.White);
            if (Keys.WasPressed(Key.Space))
            {
                ballX = startX;
                startFrame = Frame.Count;
                state = State.Playing;
            }
            break;
        case State.Playing:
            ballX = ballX + speedX;
            if (ballX < radius || ballX > Screen.Width - radius) speedX = -speedX;
            Screen.Circle(ballX, 180, radius, Colour.Cyan);
            // Frames left in the round, turned into whole seconds for the display.
            int left = (roundFrames - (Frame.Count - startFrame)) / 30;
            Screen.Text(10, 10, "Time left: " + left, Colour.White);
            if (Frame.Count - startFrame >= roundFrames) state = State.GameOver;
            break;
        case State.GameOver:
            Screen.Text(210, 150, "TIME UP", Colour.White);
            Screen.Text(130, 190, "ENTER FOR TITLE", Colour.White);
            if (Keys.WasPressed(Key.Enter)) state = State.Title;
            break;
    }
}

Game.Run(Setup, Draw);

enum State { Title, Playing, GameOver }
