int screenWidth = 640;
int screenHeight = 360;
int paddleWidth = 100;
int paddleHeight = 16;
int paddleY = 330;
int paddleMaxX = screenWidth - paddleWidth;
int paddleSpeed = 6;
float x = 270;

void Setup()
{
    Screen.Size(screenWidth, screenHeight);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    // Move the paddle with Left and Right here, then keep it on the screen.
    Screen.Rect(x, paddleY, paddleWidth, paddleHeight, Colour.White);
}

Game.Run(Setup, Draw);
