using CRE132.Game;

namespace CRE132.Engine;

// The headless renderer: one character per 16-px cell. Coarse on purpose - a challenge checks
// where things are, not pixels, and a 2-px slip is not a wrong answer. Persists between Apply
// calls exactly as the browser canvas persists between frames.
public sealed class TextCanvas
{
    public const int Cell = 16;

    readonly char[,] cells;
    public int Columns { get; }
    public int Rows { get; }

    public TextCanvas(int width, int height)
    {
        Columns = Math.Max(1, (width + Cell - 1) / Cell);
        Rows = Math.Max(1, (height + Cell - 1) / Cell);
        cells = new char[Rows, Columns];
        Fill(' ');
    }

    public static string[] Render(IReadOnlyList<DrawCommand> commands, int width, int height)
    {
        var canvas = new TextCanvas(width, height);
        canvas.Apply(commands);
        return canvas.Snapshot();
    }

    public void Apply(IReadOnlyList<DrawCommand> commands)
    {
        foreach (DrawCommand c in commands)
        {
            switch (c.Kind)
            {
                case DrawKind.Clear: Fill(' '); break;
                case DrawKind.Rect: PlotRect(c.A, c.B, c.C, c.D); break;
                case DrawKind.Circle: PlotCircle(c.A, c.B, c.C); break;
                case DrawKind.Line: PlotLine(c.A, c.B, c.C, c.D); break;
                case DrawKind.Text: PlotText(c.A, c.B, c.Text); break;
            }
        }
    }

    public string[] Snapshot()
    {
        var rows = new string[Rows];
        for (int r = 0; r < Rows; r++)
        {
            var chars = new char[Columns];
            for (int col = 0; col < Columns; col++) chars[col] = cells[r, col];
            rows[r] = new string(chars);
        }
        return rows;
    }

    public string Text => string.Join("\n", Snapshot());

    void Fill(char ch)
    {
        for (int r = 0; r < Rows; r++)
            for (int col = 0; col < Columns; col++) cells[r, col] = ch;
    }

    void Plot(int row, int col, char ch)
    {
        if (row >= 0 && row < Rows && col >= 0 && col < Columns) cells[row, col] = ch;
    }

    // The ONE place a double coordinate becomes a cell index. Student arithmetic runs away easily
    // (speed = speed * 2 reaches +Infinity in about a thousand frames and NaN one operation later),
    // and a raw (int) cast of NaN or 1e300 yields int.MinValue / int.MaxValue - which makes
    // Math.Abs overflow in PlotLine and leaves Bresenham two billion cells to walk. That runs in
    // the checker, outside ProgramRunner's catch and outside the per-frame budget, so it would
    // take the page down or freeze the tab. Clamping to just off the grid BEFORE the cast leaves
    // every ordinary coordinate exactly where it was (anything from -32 px to 32 px past the edge
    // is already inside the range) and makes an absurd one behave like any other far-away shape.
    enum Round { Down, Up }

    static int ToCell(double v, int limit, Round round = Round.Down)
    {
        double cell = v / Cell;
        if (double.IsNaN(cell)) return -2;              // NaN compares false with everything
        if (cell < -2) return -2;
        if (cell > limit + 2) return limit + 2;
        return (int)(round == Round.Up ? Math.Ceiling(cell) : Math.Floor(cell));
    }

    // A cell is touched when any part of the rect overlaps it: cell c spans [c*16, (c+1)*16).
    void PlotRect(double x, double y, double w, double h)
    {
        if (w <= 0 || h <= 0) return;
        int c0 = ToCell(x, Columns), c1 = ToCell(x + w, Columns, Round.Up) - 1;
        int r0 = ToCell(y, Rows), r1 = ToCell(y + h, Rows, Round.Up) - 1;
        for (int r = Math.Max(r0, 0); r <= Math.Min(r1, Rows - 1); r++)
            for (int c = Math.Max(c0, 0); c <= Math.Min(c1, Columns - 1); c++) cells[r, c] = '#';
    }

    // A cell is inside when its centre is within the radius.
    void PlotCircle(double x, double y, double radius)
    {
        if (radius <= 0) return;
        int c0 = ToCell(x - radius, Columns), c1 = ToCell(x + radius, Columns, Round.Up);
        int r0 = ToCell(y - radius, Rows), r1 = ToCell(y + radius, Rows, Round.Up);
        for (int r = Math.Max(r0, 0); r <= Math.Min(r1, Rows - 1); r++)
            for (int c = Math.Max(c0, 0); c <= Math.Min(c1, Columns - 1); c++)
            {
                double dx = c * Cell + Cell / 2.0 - x, dy = r * Cell + Cell / 2.0 - y;
                if (dx * dx + dy * dy <= radius * radius) cells[r, c] = 'o';
            }
    }

    // Bresenham between the two endpoint cells; Plot clips.
    void PlotLine(double x1, double y1, double x2, double y2)
    {
        int c0 = ToCell(x1, Columns), r0 = ToCell(y1, Rows);
        int c1 = ToCell(x2, Columns), r1 = ToCell(y2, Rows);
        int dc = Math.Abs(c1 - c0), dr = -Math.Abs(r1 - r0);
        int sc = c0 < c1 ? 1 : -1, sr = r0 < r1 ? 1 : -1;
        int err = dc + dr;
        int guard = dc - dr + 2;                  // ToCell bounds this at about (Columns + Rows)
        while (guard-- > 0)
        {
            Plot(r0, c0, '+');
            if (c0 == c1 && r0 == r1) break;
            int e2 = 2 * err;
            if (e2 >= dr) { err += dr; c0 += sc; }
            if (e2 <= dc) { err += dc; r0 += sr; }
        }
    }

    void PlotText(double x, double y, string text)
    {
        int row = ToCell(y, Rows), col = ToCell(x, Columns);
        for (int i = 0; i < text.Length; i++)
            Plot(row, col + i, char.IsControl(text[i]) ? ' ' : text[i]);
    }
}
