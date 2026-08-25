# Motion

Nothing on the screen really moves. A circle is drawn at `x = 100`; the frame ends; a
thirtieth of a second later a circle is drawn at `x = 103`. Do that thirty times a second and
your eye calls it motion. So the whole idea of this lesson is one sentence: **a variable changed
every frame is movement.**

:::run s14-moving A circle crossing the screen at 3 pixels a frame.

`x` and `speed` are declared at the **top of the file**, outside both methods, so they survive
from one frame to the next. A variable declared *inside* `Draw` would be created fresh every
frame and never get anywhere — that is the scope rule from the scope lesson, doing real work.

Each `Draw` does three things: clear the screen, draw the circle at `x`, then `x = x + speed;`.

`speed` is how far the shape moves **each frame**. At 30 frames a second, `speed = 3` is 90
pixels a second. Double it and the circle crosses twice as fast; make it negative and it travels
left, because adding a negative number takes it away.

:::key
Motion is arithmetic: draw at `x`, then `x = x + speed;`. A bigger `speed` is faster, a negative
`speed` goes the other way, and the variable has to live **outside** `Draw` to survive the frame.
:::

## Edges

Left alone, `x` keeps growing and the circle sails off the right-hand side forever. There are
two things you can do about that, and each is one `if`.

**Wrap** — when it goes past the right edge, put it back on the left:

```csharp
if (x > Screen.Width) x = 0;
```

That is the last line of the sample above. `Screen.Width` is `640` here — the width you set in
`Setup` — and asking for it beats typing `640` everywhere, because then changing the screen size
changes only one line.

**Bounce** — when it reaches an edge, reverse the speed:

```csharp
if (x < 20 || x > 620) speedX = -speedX;
```

`speedX = -speedX;` flips the sign: `4` becomes `-4`, and `-4` becomes `4` again. That single
line is a bounce. Give `y` its own `speedY` and do the same for the top and bottom, and the
shape rattles around all four walls:

:::run s14a-bounce Two speeds, four edges.

The `20` and the `620` are the circle's radius in from each edge. The circle is drawn from its
*centre*, so testing the centre against `20` and `640 - 20` turns it round when its rim touches
the wall rather than when half of it is already outside.

## Gravity is a speed that changes

If a speed can be reversed, it can also be nudged a little every frame. Add a small amount to
`speedY` on every `Draw` and the shape falls faster and faster; bounce it off the floor and you
have a ball:

:::edit s14b-gravity `speedY` grows by `0.5` every frame, and the floor flips it.

:::try
Change `gravity` from `0.5` to `0.1` and run — a slow, floaty, moon-sized bounce. Then try `2`
for something heavy. Then make the ball lose energy each time it lands: change the last line of
`Draw` to `if (y > 340) speedY = -speedY * 0.8;` so it comes back with 80% of the speed it
arrived with, and watch the bounces get smaller until it settles on the floor.
:::

## A free clock

`Frame.Count` is how many frames have gone by: `0` on the very first `Draw`, then `1`, `2`, `3`,
and so on. Because `Draw` runs 30 times a second, `Frame.Count` counts thirtieths of a second,
and the remainder operator from the maths lesson turns it into a metronome:

```csharp
if (Frame.Count % 30 == 0) Console.WriteLine("one second");
```

`Frame.Count % 30` is `0` once every thirty frames — once a second. And yes, `Console.WriteLine`
still works: its output appears in the console under the canvas, which is how you find out what
a moving program thinks it is doing without stopping it.

## What happens without Clear

Here is a bouncing circle with exactly one line missing — the `Screen.Clear` at the top of
`Draw`:

:::run s14c-trails A bouncing circle with nothing wiped between frames.

Every circle it has ever drawn is still there, because that is what a canvas does: it holds
paint until something covers it. Once in a while that is the effect you want. The rest of the
time it is the reason `Clear` comes first.

:::key
Bounce by reversing the speed (`speedY = -speedY;`); make gravity by adding to the speed every
frame. The canvas keeps everything you have ever drawn until you clear it.
:::

## Challenge

:::challenge c14-bouncing-ball
The starter draws a ball of radius `10` starting at `(320, 180)` and moves it by `speedX = 4`
and `speedY = 3` every frame — so after a couple of seconds it leaves the screen and never comes
back. Make it bounce off all four edges instead, so its centre never leaves the screen.

Where the comment is, add exactly two lines:

- reverse `speedX` when `x` is less than `10` **or** greater than `630`
- reverse `speedY` when `y` is less than `10` **or** greater than `350`

Those four numbers are the ball's radius, `10`, in from each of the four edges. Do not change
the starting position, the radius, or the two speeds — the checker compares where the ball is on
frames 60, 120 and 200, so its whole path has to match. Press **Check** when you are ready.
:::
