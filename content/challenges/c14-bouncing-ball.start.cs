float x = 320;
float y = 180;
float speedX = 4;
float speedY = 3;
float radius = 20;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Screen.Circle(x, y, radius, Colour.Yellow);
    x = x + speedX;
    y = y + speedY;
    // Add the two bounce tests here.
}

Game.Run(Setup, Draw);
