int screenWidth = 640;
int screenHeight = 360;
int paddleWidth = 100;
int paddleHeight = 16;
int paddleY = 330;
int paddleMaxX = screenWidth - paddleWidth;
double paddleSpeed = 6;
double x = 270;

void Setup()
{
    Screen.Size(screenWidth, screenHeight);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    if (Keys.IsDown(Key.Left)) x = x - paddleSpeed;
    if (Keys.IsDown(Key.Right)) x = x + paddleSpeed;
    // Clamp: a rectangle is drawn from its left edge, so the far end is the screen less its width.
    if (x < 0) x = 0;
    if (x > paddleMaxX) x = paddleMaxX;
    Screen.Rect(x, paddleY, paddleWidth, paddleHeight, Colour.White);
}

Game.Run(Setup, Draw);
