# Collision

A game is mostly two questions asked over and over: where is everything, and *is it touching?*
You can already answer the first. This lesson answers the second.

## How far apart are two points?

Two points, `(x1, y1)` and `(x2, y2)`. Subtract to get the gap along each axis, then turn the two
gaps into one straight-line distance:

```csharp
double dx = x2 - x1;
double dy = y2 - y1;
double dist = Math.Sqrt(dx * dx + dy * dy);
```

`Math.Sqrt(n)` is the square root of `n` — the number that, multiplied by itself, gives `n`.
`Math.Sqrt(9)` is `3`. That third line is Pythagoras: the two gaps are the short sides of a
right-angled triangle and the distance is the long one. You do not have to remember the proof;
you do have to remember the shape of the line, because you will write it in every game you make.

:::run s18-distance A fixed circle, a circle on the pointer, and the distance between them.

The grey line is the straight-line distance made visible — the hypotenuse of the triangle whose
short sides are `dx` and `dy`. Park the pointer 48 pixels across and 36 down from the middle and
the distance reads exactly `60`; move it anywhere else and you get a long tail of decimals,
because `Math.Sqrt` hands back every digit it worked out rather than a tidy answer.

The fixed circle has radius `40` and the pointer's has radius `30`. They are touching the moment
the gap between their **centres** drops below `40 + 30` — when the distance is less than the two
radii added together, each rim has reached the other:

```csharp
if (dist < 70) { ... }
```

That is the whole of circle-to-circle collision. Nothing about the shapes on screen, just two
centres and two radii.

When you only care about one axis there is a shorter tool: `Math.Abs(n)` throws away a number's
minus sign, so `Math.Abs(-12)` and `Math.Abs(12)` are both `12`. `Math.Abs(px - qx) < 30` asks
"are these two within 30 pixels of each other across?" without caring which is on the left.

:::key
Distance is `Math.Sqrt(dx * dx + dy * dy)`. Two circles are touching when that distance is less
than their two radii **added together**.
:::

## Rectangles

Rectangles do not need a square root. Two rectangles overlap unless one of them is completely
past the other on some side, which is four comparisons — and four comparisons that always look
the same are exactly what a method is for:

:::run s18a-rect-overlap A square driven into a wall with the arrow keys. Click the canvas first, then hold Right.

```csharp
bool Overlaps(double ax, double ay, double aw, double ah,
              double bx, double by, double bw, double bh)
{
    return ax < bx + bw && ax + aw > bx && ay < by + bh && ay + ah > by;
}
```

Eight parameters is a lot to type once and a joy to never type again. Written this way the game
loop reads like English — `if (Overlaps(px, 160, 40, 40, 400, 100, 40, 160))` — and if the test
turns out to be wrong there is exactly one place to fix it.

The point-in-rectangle test from the mouse lesson is this same idea with one of the rectangles
shrunk to nothing: a point is a rectangle with no width and no height.

:::key
Write a collision test as a method that returns `bool`. The four comparisons live in one place,
and every `if` that calls it says what it means.
:::

## Something to collect

Put the two ideas together and you have the oldest mechanic in games: touch a thing, the thing
goes somewhere else, a number goes up.

:::edit s18b-catch Chase the coin with the pointer. Catch it and it jumps somewhere new.

`Rand.Range` from the previous lesson picks the coin's new home, and `score = score + 1` counts
the catches. The test is a circle-to-circle one — the coin's radius is `15` and the pointer's is
`25`, so `< 40` is "the two rims have met".

:::try
Make the coin harder to catch as you go. Add `double size = 15;` at the top of the file, draw the
coin with `Screen.Circle(cx, cy, size, Colour.Yellow);`, and add `size = size - 1;` next to
`score = score + 1;`. Then fix the collision test, which still says `40`: it should now read
`if (Math.Sqrt(dx * dx + dy * dy) < size + 25)`, because the coin's radius is no longer a fixed
number. Catch enough coins and it shrinks to nothing — which is a bug, and a reason to clamp it.
:::

## Challenge

:::challenge c18-coin
The starter is a small game already: a `30` by `30` player square at `px = 460`, `py = 100` that
the arrow keys move `5` pixels a frame, a yellow coin of radius `10` at `cx = 520`, `cy = 100`,
an `int score` starting at `0`, and `Screen.Text(10, 10, "Score: " + score, Colour.White);`
drawing the score. The coin does nothing when you reach it.

Where the comment is — after the four arrow-key lines, before anything is drawn — add an `if`
with exactly this test:

`if (cx > px - 10 && cx < px + 40 && cy > py - 10 && cy < py + 40)`

That is the player's square grown by the coin's radius of `10` on every side, so it is true when
the coin's centre has come within a coin's-width of the square. Inside the `if`, do three things:
set `cx` to `100`, set `cy` to `300`, and add one to `score`.

Change nothing else: not the starting positions, not the radius `10`, not the `30` by `30`, not
the text. The checker runs two 20-frame scripts and looks at the last frame of each. In the
first it holds **Right**, so the square reaches the coin: it expects the coin at `(100, 300)` and
the text `Score: 1`. In the second it presses nothing at all: it expects the coin still at
`(520, 100)` and the text `Score: 0`, so a program that moves the coin whether or not it was
touched fails. Press **Check** when you are ready.
:::
