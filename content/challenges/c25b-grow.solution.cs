int cols = 32;
int rows = 18;
int cell = 20;
int framesPerStep = 6;
int startCol = 2;
int startRow = 9;
int dirCol = 1;
int dirRow = 0;
List<Segment> body = new List<Segment>();
int foodCol = 0;
int foodRow = 0;
int score = 0;

void Setup()
{
    Screen.Size(cols * cell, rows * cell);
    // body[0] is the head; the two behind it trail away to the left.
    for (int i = 0; i < 3; i++) body.Add(new Segment(startCol - i, startRow));

    PlaceFood();
}

// Two Rand calls, the column first and then the row, every time — so the board deals out the
// same cells in the same order on every run.
void PlaceFood()
{
    foodCol = Rand.Range(0, cols);
    foodRow = Rand.Range(0, rows);
}

// 18 rather than a full 20 leaves a two-pixel gap, so the segments can be counted.
void DrawBoard()
{
    // A square rather than a circle: on a board made of squares it is much easier to see which
    // cell the food is actually in.
    Screen.Rect(foodCol * cell + 4, foodRow * cell + 4, 12, 12, Colour.Red);

    foreach (Segment part in body)
    {
        Screen.Rect(part.col * cell, part.row * cell, 18, 18, Colour.Green);
    }

    Screen.Text(10, 10, "Score: " + score, Colour.White);
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

    if (nextCol == foodCol && nextRow == foodRow)
    {
        score = score + 1;
        PlaceFood();
    }
    else
    {
        // Nothing eaten, so the tail comes up behind the head and the snake stays its length.
        body.RemoveAt(body.Count - 1);
    }
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
