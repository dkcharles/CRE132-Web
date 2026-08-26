int screenWidth = 640;
int screenHeight = 360;
int paddleWidth = 16;
int paddleHeight = 80;
int paddleSpeed = 6;
int leftX = 20;
int rightX = screenWidth - paddleWidth - leftX;
int paddleMaxY = screenHeight - paddleHeight;
float leftY = 140;
float rightY = 140;

void Setup()
{
    Screen.Size(screenWidth, screenHeight);
}

void Draw()
{
    Screen.Clear(Colour.Black);

    // Move the left paddle with W and S here, then clamp leftY.

    // Move the right paddle with Up and Down here, then clamp rightY.

    Screen.Rect(leftX, leftY, paddleWidth, paddleHeight, Colour.White);
    Screen.Rect(rightX, rightY, paddleWidth, paddleHeight, Colour.White);
}

Game.Run(Setup, Draw);
