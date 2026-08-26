using CRE132.Game;
using Xunit;

namespace CRE132.Tests;

public class GameApiTests
{
    // Installs a fresh state for one test and removes it afterwards.
    sealed class Installed : IDisposable
    {
        public GameState State { get; } = new(1);
        public Installed() => GameHost.Active = State;
        public void Dispose() => GameHost.Active = null;
    }

    [Fact]
    public void Screen_calls_are_recorded_into_the_current_frame_and_discarded_outside_one()
    {
        using var s = new Installed();
        Screen.Circle(1, 2, 3, Colour.Red);                 // no frame open: dropped
        s.State.Frame = new List<DrawCommand>();
        Screen.Rect(10, 20, 30, 40, Colour.Blue);
        Screen.Text(5, 6, "hi", Colour.White);
        Assert.Equal(2, s.State.Frame.Count);
        Assert.Equal(new DrawCommand(DrawKind.Rect, 10, 20, 30, 40, Colour.Blue), s.State.Frame[0]);
        Assert.Equal("hi", s.State.Frame[1].Text);
    }

    [Fact]
    public void Size_defaults_to_640_by_360_and_rejects_nonsense()
    {
        using var s = new Installed();
        Assert.Equal((640, 360), (Screen.Width, Screen.Height));
        Screen.Size(800, 480);
        Assert.Equal((800, 480), (Screen.Width, Screen.Height));
        Assert.Throws<ArgumentException>(() => Screen.Size(0, 10));
    }

    [Fact]
    public void WasPressed_and_WasClicked_are_edges_between_two_input_states()
    {
        using var s = new Installed();
        s.State.Previous = InputState.None;
        s.State.Current = new InputState(new HashSet<Key> { Key.Space }, 3, 4, true);
        Assert.True(Keys.IsDown(Key.Space));
        Assert.True(Keys.WasPressed(Key.Space));
        Assert.True(Mouse.WasClicked);
        Assert.Equal((3, 4), (Mouse.X, Mouse.Y));

        s.State.Previous = s.State.Current;                 // still held next frame
        Assert.True(Keys.IsDown(Key.Space));
        Assert.False(Keys.WasPressed(Key.Space));
        Assert.False(Mouse.WasClicked);
    }

    [Fact]
    public void Rand_is_deterministic_for_a_seed_and_validates_its_range()
    {
        int[] a, b;
        using (var s = new Installed()) a = new[] { Rand.Range(0, 100), Rand.Range(0, 100), Rand.Range(0, 100) };
        using (var s = new Installed()) b = new[] { Rand.Range(0, 100), Rand.Range(0, 100), Rand.Range(0, 100) };
        Assert.Equal(a, b);
        using var t = new Installed();
        Assert.Throws<ArgumentException>(() => Rand.Range(5, 5));
        float d = Rand.Range(1.5f, 2.5f);
        Assert.InRange(d, 1.5f, 2.5f);
    }

    [Fact]
    public void Game_Run_registers_once_and_refuses_twice_or_null()
    {
        using var s = new Installed();
        Action setup = () => { }, draw = () => { };
        CRE132.Game.Game.Run(setup, draw);
        Assert.Same(draw, s.State.Draw);
        Assert.Throws<InvalidOperationException>(() => CRE132.Game.Game.Run(setup, draw));
        using var t = new Installed();
        Assert.Throws<ArgumentException>(() => CRE132.Game.Game.Run(null!, draw));
    }

    [Fact]
    public void Colour_Rgb_clamps()
    {
        Assert.Equal(new Colour(255, 0, 7), Colour.Rgb(300, -1, 7));
    }
}
