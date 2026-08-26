# Going further

Snake was the last thing this course asks you to build. This page asks you for nothing at all.

There are three programs below, and there is no challenge under any of them. Every one is
longer than anything the lessons made you write, and between the three of them there is exactly
one thing the course has not already taught — a grid of ints, in the middle program, and it gets
three sentences when it turns up. Everything else is a class with public fields, a `List` of
objects, an `enum` and a `switch`, a countdown timer, `Rand` and `MathF.Sqrt`. That is the point
of the page. What separates a lesson sample from a thing that looks alive is not new syntax; it
is more of the same syntax, arranged with a little more nerve.

Run each one, read it, then take it apart. Nothing here can break.

## Steering

Five agents share the canvas with your pointer. Two of them want to reach it, two of them want
to be anywhere else, and one is not interested and drifts about on its own.

:::run s26-steering Five agents, three behaviours. Move the pointer around the canvas and watch who comes and who goes.

Every agent owns a `position` and a `velocity`, both of them `Vec2` objects — the class from the
vectors lesson, copied into the file without a character changed, `Length` and `Normalised` and
`Add` and `Scale` and all. That is why the program runs past a hundred lines: thirty-one of
them are a class you already wrote once.

The agents live in a `List<Agent>`, filled in `Setup` and walked with a `foreach` in `Draw`,
exactly as the objects-together lesson walked its balls. Each one carries a `Behaviour` — an
`enum`, from the game-state lesson, holding `Seek`, `Flee` or `Wander` — and `Update` opens with
a `switch` on it, because the three behaviours differ by one line and no more:

```csharp
case Behaviour.Seek: push = new Vec2(pointer.x - position.x, pointer.y - position.y); break;
case Behaviour.Flee: push = new Vec2(position.x - pointer.x, position.y - pointer.y); break;
```

Seek is target minus position. Flee is the same subtraction the other way round. Wander is two
calls to `Rand.Range(-0.5f, 0.5f)`, a shove in no particular direction. All three arrows are
normalised on the line below, which throws the length away and keeps only which way the arrow
points — so the wanderer's `0.5f` is not a speed. It only decides which way the nudge leans, and
making it bigger or smaller changes nothing you can see.

The three lines after the `switch` are the whole of the movement, and they are the same three
for all five agents. The `push` is normalised and scaled by `steerForce`, which is `0.4f`, and
added to the velocity — a nudge, not a jump, which is why an agent turns in a curve instead of
snapping round. The velocity is then normalised and scaled by `maxSpeed`, which is `4`, so
however hard it was pushed it always ends the frame moving at exactly four pixels a frame.
Finally the velocity is added to the position.

Steady speed is what makes a seeker overshoot: it cannot slow down, so it sails past the pointer,
turns, and comes back — and that circling is the behaviour, not a bug in it. An agent that runs
off an edge appears at the opposite one, four lines at the end of `Update`, so nobody is ever
lost off-screen.

:::try
This one has no editor of its own, so copy the whole program into the **Playground** — the
free-form editor linked from the front page — and change it there.

Start with the numbers. `maxSpeed` at `4` and `steerForce` at `0.4f` give a lazy, wide turn.
Try `steerForce = 2;` and watch the agents snap onto the pointer like magnets; try
`maxSpeed = 12;` and they orbit it in enormous loops.

Then change who is who. Turn one of the `Behaviour.Flee` agents in `Setup` into
`Behaviour.Wander`, or add a sixth line to `Setup` and put another seeker in. Six agents cost you
one line, which is the entire argument for a `List` of objects.

Then make an agent chase an agent. In `Draw`, before the `foreach`, replace the pointer with a
target of your own choosing — `Vec2 pointer = agents[4].position;` hands every other agent the
wanderer's position instead of the mouse's, and the four of them will hunt and flee from a dot
that is itself wandering.
:::

:::key
A class with fields, a `List` of those objects, a `Vec2` doing the arithmetic and an `enum`
choosing what each one wants: that is a simulation. Nothing in this program is new — it is four
things you already know, stacked.
:::

## Conway's Game of Life

A board of `32` by `18` squares. Each square is alive or dead. Once every six frames the whole
board takes a step, and each square looks at the eight around it and follows three rules: a live
square with two or three live neighbours stays alive, a dead square with exactly three live
neighbours comes alive, and everything else is dead next generation.

That is all of it. There is no more to the rules than that paragraph, and yet:

:::run s26a-life A glider crawling out of the top-left corner, and an r-pentomino below the middle that keeps the board boiling for a couple of minutes before it settles into a few blinkers.

The board needs somewhere to live, and this is the one place in the course where a `List` is the
wrong shape. A board is not a queue of things, it is rows and columns, so it gets a **grid of
ints**:

```csharp
int[,] cells = new int[32, 18];
```

which the program writes as `new int[cols, rows]`, `cols` being `32` and `rows` being `18`. Two
numbers in the square brackets instead of one, and two numbers to read a square back out —
`cells[col, row]`, `0` for dead and `1` for alive. Everything else about it is an array as the
collections lesson taught it: fixed size, and every square starts at `0` for free.

`Step` builds a **second** grid rather than editing the one it is reading. That is not tidiness,
it is the rules: every square has to be judged against the board as it was at the start of the
generation, and a square that had already been overwritten would give its neighbours the wrong
answer. When the new grid is finished, `cells = next;` and the old board is gone.

`Neighbours` counts the eight squares around one square with a pair of loops from `-1` to `1`,
skipping `0, 0` because a square is not its own neighbour. The interesting line is what happens
at the edge:

```csharp
int nearCol = (col + dx + cols) % cols;
```

The `% cols` wraps column `32` back round to column `0`, and adding `cols` first keeps the answer
positive when `col + dx` is `-1`. The board has no edges: it is a loop left-to-right and a loop
top-to-bottom, joined into a ring in both directions. That matters for what you are watching.
A glider that reaches the right-hand wall of an ordinary board comes apart against it and stops
being a glider; on this board it sails off the right-hand edge and arrives on the left, and it
keeps going until it runs into whatever the r-pentomino has scattered in its path.

Drawing is two loops and one `if`: an `18` by `18` green square at `col * cell, row * cell` for
every `1` in the grid, the same square the Snake board was drawn with.

:::try
Copy this program into the **Playground** too, and take it apart from the seed outwards.

First, thin it out. Delete the r-pentomino's five lines from `Setup` and leave the glider on its
own. Nothing is left to interfere with it, so it crosses the board forever — off the right-hand
edge, back on at the left, round and round.

Now break that loop, with the glider still on its own. In `Neighbours`, replace the two `%` lines
with `int nearCol = col + dx;` and `int nearRow = row + dy;`, and widen the test on the line that
counts so that a square off the board is never read at all:

```csharp
if ((dx != 0 || dy != 0) && nearCol >= 0 && nearCol < cols && nearRow >= 0 && nearRow < rows)
{
    total = total + cells[nearCol, nearRow];
}
```

The board has walls now, everything outside them counts as dead, and the glider is not immortal
any more: it crashes into the bottom wall and by generation `64` has come apart into a
four-square block at columns `16` and `17`, rows `16` and `17`, which sits there unchanged
forever.

Or fill the board at random rather than seeding it by hand — a loop over every square with
`if (Rand.Range(0, 5) == 0) cells[col, row] = 1;` fills about a fifth of it, and no two runs look
alike.

Last, change the speed. `framesPerStep` is `6`, which is five generations a second. Set it to
`1` and the pattern boils; set it to `30` and you get one generation a second, slow enough to
follow a single glider square by square.
:::

:::key
A grid is an array with two indexes: `int[,] cells = new int[32, 18];`, read as
`cells[col, row]`. Use one when the thing you are storing genuinely has rows and columns — a
board, a map, a maze. Everything else in this course is still a `List`.
:::

## Particles

Click anywhere on the canvas.

:::run s26b-particles Forty sparks a burst. Click the canvas, and one goes off by itself every three seconds anyway.

A `Particle` is a small class, and most of it is numbers you already know: an `x` and a `y`, a
`speedX` and a `speedY`, and an `int life` counting down. `Burst` adds forty of them at the same
point, each with `Rand.Range(-spread, spread)` for its sideways speed and the same again for its
vertical one — `spread` is `3.5f` — so forty sparks leave one place in forty different
directions. `gravity` of `0.2f` is added to `speedY` every frame — the falling from the motion
lesson, one line — which is what bends a ball of sparks into a firework.

`life` starts at `75`, drops by one a frame and reaches zero after two and a half seconds. The
removal loop is the objects-together lesson's, unchanged: an index loop rather than a `foreach`,
`RemoveAt(i)` when the life runs out, and `i--` straight after it so the spark that slides into
the gap is not skipped.

The one number worth stealing is in `Draw`:

```csharp
float radius = smallRadius + (bigRadius - smallRadius) * life / fullLife;
```

`life` and `fullLife` are both `int`, so `life / fullLife` written on its own would be the
whole-number division of the maths lesson: `0` from the second frame of a spark's life onwards,
the remainder thrown away. It is not that, because `*` and `/` run left to right, and
`bigRadius - smallRadius` is a `float`. The multiply goes first, and dividing a `float` by an
`int` keeps the fraction. So `4 * life / fullLife` slides from `4` at birth down to `0` at death,
and the radius with it, from `16` down to `12`. A spark that faded all the way to
nothing would spend its last frames too small to see; one that stops at `12` fades and then
vanishes cleanly.

:::edit s26c-particles-edit The same program, in an editor. Change a number, press Run, click the canvas.

:::try
Everything below goes in the editor just above, and **Reset** always brings the original back.

Make it a fountain. In the `Particle` constructor, change the vertical speed to
`speedY = Rand.Range(-9.5f, -4.5f);` so every spark starts by going up, and gravity brings them
all down again.

Make it a firework that lingers. `fullLife` at `75` is two and a half seconds; try `200`. Then
turn `gravity` down to `0.05f` and up to `1`, and watch the same forty sparks go from smoke to
gravel.

Make it dense. `burstSize` is `40` — try `200`, and then try `2000`, which is where you find out
that the number of things a program can draw in a thirtieth of a second is not infinite.

Then let the sparks land. In `Update`, under the two lines that move the spark, add:

```csharp
if (y > Screen.Height - smallRadius)
{
    y = Screen.Height - smallRadius;
    speedY = -speedY * 0.5f;
}
```

Half the speed back the other way, every bounce, and the sparks pile up along the floor instead
of falling through it.
:::

## What Unity gives you

Everything on this page was drawn by `Screen.Circle` and `Screen.Rect`, moved by numbers you
added up yourself, and put on the canvas by a `Draw` method you wrote. Unity is the same picture
with the tedious parts already done.

A Unity **scene** is the canvas, and the objects in it are listed for you instead of living in a
`List` you fill in `Setup`. A **component** is a class exactly like `Agent` or `Particle`,
attached to one of those objects, and its public fields show up in the **Inspector** as boxes you
can type into while the game is running — which is where `maxSpeed = 4` and `gravity = 0.2f` go
instead of into a constructor. `Vector2` is `Vec2`, already written, with `magnitude` where you
wrote `Length()` and `normalized` where you wrote `Normalised()`, and `+` and `*` as symbols
instead of method calls. And `Update()` is `Draw`: a method Unity calls once a frame, on every
component, forever.

The window, the sprites, the physics and the sound are Unity's. The `class` with the fields, the
`List` of them, the vector arithmetic, the `enum` that decides which screen you are looking at
and the timer that decides when the next thing happens — those are all yours, and you have just
spent twenty-six lessons learning to write them.
