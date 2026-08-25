using CRE132.Engine;
using CRE132.Game;
using Xunit;

namespace CRE132.Tests;

public class TextCanvasTests
{
    static DrawCommand Rect(double x, double y, double w, double h) => new(DrawKind.Rect, x, y, w, h, Colour.White);
    static DrawCommand Circle(double x, double y, double r) => new(DrawKind.Circle, x, y, r, 0, Colour.White);

    [Fact]
    public void Default_screen_is_40_columns_by_23_rows_of_spaces()
    {
        var c = new TextCanvas(640, 360);
        Assert.Equal((40, 23), (c.Columns, c.Rows));
        string[] rows = c.Snapshot();
        Assert.Equal(23, rows.Length);
        Assert.All(rows, r => Assert.Equal(new string(' ', 40), r));
    }

    [Fact]
    public void A_rect_fills_every_cell_it_touches_with_hash()
    {
        // x 16..48 covers columns 1 and 2 exactly; y 16..32 is row 1.
        string[] rows = TextCanvas.Render(new[] { Rect(16, 16, 32, 16) }, 640, 360);
        Assert.Equal(" ##" + new string(' ', 37), rows[1]);
        Assert.Equal(new string(' ', 40), rows[0]);
        Assert.Equal(new string(' ', 40), rows[2]);
        // A rect that straddles a boundary by one pixel touches the next cell too.
        rows = TextCanvas.Render(new[] { Rect(16, 16, 33, 16) }, 640, 360);
        Assert.Equal(" ###" + new string(' ', 36), rows[1]);
    }

    [Fact]
    public void A_circle_marks_cells_whose_centre_is_within_the_radius()
    {
        // Centre (40, 40) = middle of cell (2,2); radius 8 reaches no neighbouring centre (16 px apart).
        string[] rows = TextCanvas.Render(new[] { Circle(40, 40, 8) }, 640, 360);
        Assert.Equal("  o" + new string(' ', 37), rows[2]);
        Assert.Equal(new string(' ', 40), rows[1]);
        // Radius 16 reaches the four orthogonal neighbours, not the diagonals (22.6 px away).
        rows = TextCanvas.Render(new[] { Circle(40, 40, 16) }, 640, 360);
        Assert.Equal("  o" + new string(' ', 37), rows[1]);
        Assert.Equal(" ooo" + new string(' ', 36), rows[2]);
        Assert.Equal("  o" + new string(' ', 37), rows[3]);
    }

    [Fact]
    public void A_line_plots_plus_along_its_cells_and_text_writes_one_char_per_cell()
    {
        string[] rows = TextCanvas.Render(new[]
        {
            new DrawCommand(DrawKind.Line, 0, 0, 64, 0, Colour.White),        // cells 0..4 of row 0
            new DrawCommand(DrawKind.Text, 32, 32, 0, 0, Colour.White, "Hi!")      // row 2, from column 2
        }, 640, 360);
        Assert.Equal("+++++" + new string(' ', 35), rows[0]);
        Assert.Equal("  Hi!" + new string(' ', 35), rows[2]);
    }

    [Fact]
    public void Later_commands_overwrite_and_clear_wipes_but_the_canvas_persists_between_applies()
    {
        var c = new TextCanvas(640, 360);
        c.Apply(new[] { Rect(0, 0, 640, 360) });
        c.Apply(new[] { Circle(8, 8, 4) });                     // second frame, no Clear: rect survives
        Assert.Equal('o', c.Snapshot()[0][0]);
        Assert.Equal('#', c.Snapshot()[0][1]);
        c.Apply(new[] { new DrawCommand(DrawKind.Clear, 0, 0, 0, 0, Colour.Black) });
        Assert.All(c.Snapshot(), r => Assert.Equal(new string(' ', 40), r));
    }

    [Fact]
    public void Off_screen_geometry_is_clipped_not_thrown()
    {
        string[] rows = TextCanvas.Render(new[]
        {
            Rect(-100, -100, 150, 150), Circle(700, 400, 30),
            new DrawCommand(DrawKind.Line, -50, 200, 900, 200, Colour.White),   // row 12, clipped both ends
            new DrawCommand(DrawKind.Text, 624, 0, 0, 0, Colour.White, "overflowing")
        }, 640, 360);
        Assert.Equal(23, rows.Length);
        Assert.Equal('#', rows[0][0]);
        Assert.Equal('o', rows[0][39]);   // text 'o' at column 39; the rest fell off the grid
        Assert.Equal(new string('+', 40), rows[12]);
    }
}
