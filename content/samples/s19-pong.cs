int screenWidth = 640;
int screenHeight = 360;
int paddleWidth = 16;
int paddleHeight = 80;
int paddleSpeed = 6;
int leftX = 20;
// The right paddle sits the same margin in from its edge as the left one: 640 - 16 - 20 = 604.
int rightX = screenWidth - paddleWidth - leftX;
// A rectangle is drawn from its top edge, so 360 - 80 = 280 is as low as a paddle may sit.
int paddleMaxY = screenHeight - paddleHeight;
float leftY = 140;
float rightY = 140;

int centreX = screenWidth / 2;
int centreY = screenHeight / 2;
int ballRadius = 12;
float bx = centreX;
float by = centreY;
float ballSpeedX = 4;
float ballSpeedY = 5;

int left = 0;
int right = 0;

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

    bx = bx + ballSpeedX;
    by = by + ballSpeedY;
    if (by < ballRadius || by > screenHeight - ballRadius) ballSpeedY = -ballSpeedY;
    // A paddle hit is the collision lesson's rectangle grown by the ball's radius: each of the
    // four comparisons is one edge of the paddle pushed out by ballRadius.
    if (bx > leftX - ballRadius && bx < leftX + paddleWidth + ballRadius &&
        by > leftY - ballRadius && by < leftY + paddleHeight + ballRadius)
        ballSpeedX = -ballSpeedX;
    if (bx > rightX - ballRadius && bx < rightX + paddleWidth + ballRadius &&
        by > rightY - ballRadius && by < rightY + paddleHeight + ballRadius)
        ballSpeedX = -ballSpeedX;
    // A miss: the ball goes back to the middle and the player at the other end scores.
    if (bx < 0)
    {
        bx = centreX;
        by = centreY;
        right = right + 1;
    }
    if (bx > screenWidth)
    {
        bx = centreX;
        by = centreY;
        left = left + 1;
    }

    Screen.Rect(leftX, leftY, paddleWidth, paddleHeight, Colour.White);
    Screen.Rect(rightX, rightY, paddleWidth, paddleHeight, Colour.White);
    Screen.Circle(bx, by, ballRadius, Colour.Yellow);
    // Drawn last, so the score sits on top of everything else rather than under the ball.
    Screen.Text(300, 10, left + " : " + right, Colour.White);
}

Game.Run(Setup, Draw);
