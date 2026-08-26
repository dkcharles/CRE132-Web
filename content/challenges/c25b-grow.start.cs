int cols = 32;
int rows = 18;
int cell = 20;
int framesPerStep = 6;
int startCol = 2;
int startRow = 9;
int dirCol = 1;
int dirRow = 0;
List<Segment> body = new List<Segment>();

// 1. foodCol, foodRow and score go here.

void Setup()
{
    Screen.Size(cols * cell, rows * cell);
    // body[0] is the head; the two behind it trail away to the left.
    for (int i = 0; i < 3; i++) body.Add(new Segment(startCol - i, startRow));

    // 3. Put the first food on the board here.
}

// 2. PlaceFood() goes here.

// 18 rather than a full 20 leaves a two-pixel gap, so the segments can be counted.
void DrawBoard()
{
    // 4. Draw the food square here.

    foreach (Segment part in body)
    {
        Screen.Rect(part.col * cell, part.row * cell, 18, 18, Colour.Green);
    }

    // 5. Draw the score here.
}

// One whole cell, in whichever direction the arrows last chose.
void Step()
{
    int nextCol = body[0].col + dirCol;
    int nextRow = body[0].row + dirRow;
    // Off one edge and back on the other, for now.
    if (nextCol < 0) nextCol = cols - 1;
    if (nextCol > cols - 1) nextCol = 0;
    if (nextRow < 0) nextRow = rows - 1;
    if (nextRow > rows - 1) nextRow = 0;
    // Insert(0, ...) is Add with a place to put it: the new head goes on the front of the list.
    body.Insert(0, new Segment(nextCol, nextRow));

    // 6. When the new head has landed on the food, score and grow instead of dropping the tail.
    body.RemoveAt(body.Count - 1);
}

void Draw()
{
    Screen.Clear(Colour.Black);

    // An arrow only chooses a direction, and a turn back on yourself is not one: half the snake
    // is already in the way, so the game ignores it.
    if (Keys.IsDown(Key.Left) && dirCol != 1) { dirCol = -1; dirRow = 0; }
    if (Keys.IsDown(Key.Right) && dirCol != -1) { dirCol = 1; dirRow = 0; }
    if (Keys.IsDown(Key.Up) && dirRow != 1) { dirCol = 0; dirRow = -1; }
    if (Keys.IsDown(Key.Down) && dirRow != -1) { dirCol = 0; dirRow = 1; }

    if (Frame.Count % framesPerStep == 0) Step();

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
