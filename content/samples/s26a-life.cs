int cols = 32;
int rows = 18;
int cell = 20;
int framesPerStep = 6;
int generation = 0;
int[,] cells = new int[cols, rows];

void Setup()
{
    Screen.Size(cols * cell, rows * cell);
    // A glider: five cells that crawl diagonally, with no wall to die against on a board
    // whose edges wrap round.
    cells[1, 0] = 1;
    cells[2, 1] = 1;
    cells[0, 2] = 1;
    cells[1, 2] = 1;
    cells[2, 2] = 1;
    // An r-pentomino: five cells that fill the board with wreckage and never quite settle.
    cells[16, 12] = 1;
    cells[17, 12] = 1;
    cells[15, 13] = 1;
    cells[16, 13] = 1;
    cells[16, 14] = 1;
}

int Neighbours(int col, int row)
{
    int total = 0;
    for (int dx = -1; dx <= 1; dx++)
    {
        for (int dy = -1; dy <= 1; dy++)
        {
            // Adding cols before the % keeps the answer positive off the left and top edges.
            int nearCol = (col + dx + cols) % cols;
            int nearRow = (row + dy + rows) % rows;
            if (dx != 0 || dy != 0) total = total + cells[nearCol, nearRow];
        }
    }
    return total;
}

// One generation: three rules, applied to every cell at once.
void Step()
{
    // A second grid, because every cell must be judged on the old one, not on half-new neighbours.
    int[,] next = new int[cols, rows];
    for (int col = 0; col < cols; col++)
    {
        for (int row = 0; row < rows; row++)
        {
            int neighbours = Neighbours(col, row);
            next[col, row] = 0;
            if (cells[col, row] == 1 && (neighbours == 2 || neighbours == 3)) next[col, row] = 1;
            if (cells[col, row] == 0 && neighbours == 3) next[col, row] = 1;
        }
    }
    cells = next;
}

void Draw()
{
    Screen.Clear(Colour.Black);
    // Frame.Count is 0 on the very first Draw, so generation 1 arrives straight away.
    if (Frame.Count % framesPerStep == 0)
    {
        Step();
        generation = generation + 1;
    }
    for (int col = 0; col < cols; col++)
    {
        for (int row = 0; row < rows; row++)
        {
            if (cells[col, row] == 1) Screen.Rect(col * cell, row * cell, 18, 18, Colour.Green);
        }
    }
    Screen.Text(10, 10, "Generation " + generation, Colour.White);
}

Game.Run(Setup, Draw);
