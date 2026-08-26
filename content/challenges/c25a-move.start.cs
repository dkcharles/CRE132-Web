int cols = 32;
int rows = 18;
int cell = 20;
int framesPerStep = 6;
int startCol = 2;
int startRow = 9;
int dirCol = 1;
int dirRow = 0;
List<Segment> body = new List<Segment>();

void Setup()
{
    Screen.Size(cols * cell, rows * cell);
    // body[0] is the head; the two behind it trail away to the left.
    for (int i = 0; i < 3; i++) body.Add(new Segment(startCol - i, startRow));
}

// 18 rather than a full 20 leaves a two-pixel gap, so the segments can be counted.
void DrawBoard()
{
    foreach (Segment part in body)
    {
        Screen.Rect(part.col * cell, part.row * cell, 18, 18, Colour.Green);
    }
}

// One whole cell, in whichever direction the arrows last chose.
void Step()
{
    // 1. Work out the next head cell, wrap it round the edges, and move the snake here.
}

void Draw()
{
    Screen.Clear(Colour.Black);

    // 2. Let the four arrow keys change dirCol and dirRow here.

    // 3. Call Step() once every framesPerStep frames here.

    DrawBoard();
}

Game.Run(Setup, Draw);

class Segment
{
    public int col, row;

    public Segment(int atCol, int atRow)
    {
        col = atCol;
        row = atRow;
    }
}
