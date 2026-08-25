int screenWidth = 640;
int screenHeight = 360;
int paddleWidth = 16;
int paddleHeight = 80;
int paddleSpeed = 6;
int leftX = 20;
int rightX = screenWidth - paddleWidth - leftX;
int paddleMaxY = screenHeight - paddleHeight;
double leftY = 140;
double rightY = 140;

int centreX = screenWidth / 2;
int centreY = screenHeight / 2;
int ballRadius = 12;
double bx = centreX;
double by = centreY;
double ballSpeedX = 4;
double ballSpeedY = 5;

void Setup()
{
    Screen.Size(screenWidth, screenHeight);
}

void Draw()
{
    Screen.Clear(Colour.Black);

    if (Keys.IsDown(Key.W)) leftY = leftY - paddleSpeed;
    if (Keys.IsDown(Key.S)) leftY = leftY + paddleSpeed;
    if (leftY < 0) leftY = 0;
    if (leftY > paddleMaxY) leftY = paddleMaxY;

    if (Keys.IsDown(Key.Up)) rightY = rightY - paddleSpeed;
    if (Keys.IsDown(Key.Down)) rightY = rightY + paddleSpeed;
    if (rightY < 0) rightY = 0;
    if (rightY > paddleMaxY) rightY = paddleMaxY;

    // Move the ball, bounce it off the top and bottom, reflect it off the
    // paddles, and put it back in the middle when it leaves the screen.

    Screen.Rect(leftX, leftY, paddleWidth, paddleHeight, Colour.White);
    Screen.Rect(rightX, rightY, paddleWidth, paddleHeight, Colour.White);
    Screen.Circle(bx, by, ballRadius, Colour.Yellow);
}

Game.Run(Setup, Draw);
