int screenWidth = 640;
int screenHeight = 360;
int groundY = 300;
int wallX = 220;
int wallY = 180;
int wallWidth = 200;
int wallHeight = 120;
int middleX = wallX + wallWidth / 2;
int roofTopY = 100;

void Setup()
{
    Screen.Size(screenWidth, screenHeight);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Screen.Rect(0, groundY, screenWidth, 60, Colour.Green);
    Screen.Rect(wallX, wallY, wallWidth, wallHeight, Colour.Orange);
    Screen.Circle(middleX, 240, 16, Colour.Cyan);
    // Two lines meeting at the apex make the roof: up from one wall corner, down to the other.
    Screen.Line(wallX, wallY, middleX, roofTopY, Colour.Red);
    Screen.Line(middleX, roofTopY, wallX + wallWidth, wallY, Colour.Red);
}

Game.Run(Setup, Draw);
