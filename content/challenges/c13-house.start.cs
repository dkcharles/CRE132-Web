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
    // Draw your house here: walls, then a window, then two roof lines.
}

Game.Run(Setup, Draw);
