# Collision

A game is mostly two questions asked over and over: where is everything, and *is it touching?*
You can already answer the first. This lesson answers the second.

## How far apart are two points?

Two points, `(x1, y1)` and `(x2, y2)`. Subtract to get the gap along each axis, then turn the two
gaps into one straight-line distance:

```csharp
float dx = x2 - x1;
float dy = y2 - y1;
float dist = MathF.Sqrt(dx * dx + dy * dy);
```

`MathF.Sqrt(n)` is the square root of `n` — the number that, multiplied by itself, gives `n`.
`MathF.Sqrt(9)` is `3`. `MathF` is the maths toolbox for floats — Unity's `Mathf` works the same
way. That third line is Pythagoras: the two gaps are the short sides of a right-angled triangle
and the distance is the long one. You do not have to remember the proof; you do have to remember
the shape of the line, because you will write it in every game you make.

:::run s18-distance A fixed circle, a circle on the pointer, and the distance between them.

The grey line is the straight-line distance made visible — the hypotenuse of the triangle whose
short sides are `dx` and `dy`. Park the pointer 48 pixels across and 36 down from the middle and
the distance reads exactly `60`; move it anywhere else and you get a long tail of decimals,
because `MathF.Sqrt` hands back every digit it worked out rather than a tidy answer.

The fixed circle has radius `40` and the pointer's has radius `30`. They are touching the moment
the gap between their **centres** drops below `40 + 30` — when the distance is less than the two
radii added together, each rim has reached the other:

```csharp
if (dist < 70) { ... }
```

That is the whole of circle-to-circle collision. Nothing about the shapes on screen, just two
centres and two radii.

When you only care about one axis there is a shorter tool: `MathF.Abs(n)` throws away a number's
minus sign, so `MathF.Abs(-12)` and `MathF.Abs(12)` are both `12`. `MathF.Abs(px - qx) < 30` asks
"are these two within 30 pixels of each other across?" without caring which is on the left.

:::key
Distance is `MathF.Sqrt(dx * dx + dy * dy)`. Two circles are touching when that distance is less
than their two radii **added together**.
:::

## Rectangles

Rectangles do not need a square root. Two rectangles overlap unless one of them is completely
past the other on some side, which is four comparisons — and four comparisons that always look
the same are exactly what a method is for:

:::run s18a-rect-overlap A square driven into a wall with the arrow keys. Click the canvas first, then hold Right.

```csharp
bool Overlaps(float ax, float ay, float aw, float ah,
              float bx, float by, float bw, float bh)
{
    return ax < bx + bw && ax + aw > bx && ay < by + bh && ay + ah > by;
}
```

Eight parameters is a lot to type once and a joy to never type again. Written this way the game
loop reads like English:

```csharp
if (Overlaps(playerX, playerY, playerSize, playerSize, wallX, wallY, wallWidth, wallHeight))
```

If the test turns out to be wrong there is exactly one place to fix it. Every one of those eight
arguments is a variable declared at the top of the file, and that is what makes the line
readable: the same call written out as `Overlaps(100, 160, 40, 40, 400, 100, 40, 160)` compiles
just as well and tells you nothing at all about which square is which.

The point-in-rectangle test from the mouse lesson is this same idea with one of the rectangles
shrunk to nothing: a point is a rectangle with no width and no height.

A **circle** against a rectangle is the same trick the other way round. Grow the rectangle by the
circle's radius on all four sides, then ask the point-in-rectangle question about the circle's
centre. A coin of radius `coinRadius` meeting a `playerSize` by `playerSize` square whose corner
is at `px`, `py`:

```csharp
if (cx > px - coinRadius && cx < px + playerSize + coinRadius &&
    cy > py - coinRadius && cy < py + playerSize + coinRadius)
```

`px - coinRadius` is the left edge pushed out by one radius; `px + playerSize + coinRadius` is
the right edge pushed out by the same — with a coin of `10` and a square of `30` that works out
as `px + 40`; the two `py` comparisons do the top and the bottom. The condition is split over two
lines with the `&&` left at the end of the first: C# does not care where you break a line, it
reads on until the `)` closes the condition. It treats the circle
as a square, so it is a shade generous at the corners — and it is four comparisons with no square
root, which is why most 2D games use it anyway. You will write it once more in the next lesson,
for a ball meeting a paddle.

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
Make the coin harder to catch as you go. Add `float size = 15;` at the top of the file, draw the
coin with `Screen.Circle(cx, cy, size, Colour.Yellow);`, and add `size = size - 1;` next to
`score = score + 1;`. Then fix the collision test, which still says `40`: it should now read
`if (MathF.Sqrt(dx * dx + dy * dy) < size + 25)`, because the coin's radius is no longer a fixed
number. Catch enough coins and it shrinks to nothing — which is a bug, and a reason to clamp it.
:::

## Challenge

:::challenge c18-coin
The starter is a small game already: a `playerSize` by `playerSize` player square — that is `30`
by `30` — at `px = 460`, `py = 100`, which the arrow keys move `playerSpeed` (`5`) pixels a
frame; a yellow coin of radius `coinRadius` (`10`) at `cx = 520`, `cy = 100`; an `int score`
starting at `0`; and `Screen.Text(10, 10, "Score: " + score, Colour.White);` drawing the score.
The coin does nothing when you reach it.

Where the comment is — after the four arrow-key lines, before anything is drawn — add an `if`
with exactly this test, split over two lines the same way:

```csharp
if (cx > px - coinRadius && cx < px + playerSize + coinRadius &&
    cy > py - coinRadius && cy < py + playerSize + coinRadius)
```

That is the player's square grown by the coin's radius on every side — `px - 10` to `px + 40`
across, the same down — so it is true when the coin's centre has come within a coin's-width of
the square. Inside the `if`, do three things: set `cx` to `100`, set `cy` to `300`, and add one
to `score`.

Change nothing else: not the starting positions, not `coinRadius`, not `playerSize`, not
the text. The checker runs two 20-frame scripts and looks at the last frame of each. In the
first it holds **Right**, so the square reaches the coin: it expects the coin at `(100, 300)` and
the text `Score: 1`. In the second it presses nothing at all: it expects the coin still at
`(520, 100)` and the text `Score: 0`, so a program that moves the coin whether or not it was
touched fails. Press **Check** when you are ready.
:::
