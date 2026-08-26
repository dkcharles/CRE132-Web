# Mini-game: Snake

Five lessons of parts: a class, a list of objects, vectors, states, timers and grids. This
lesson adds one list method and nothing else. It puts the parts together into the game that
shipped on every Nokia phone in the world, and it is a much smaller program than you would guess.

:::run s25-snake The finished game. Click the canvas first, then press the space bar and steer with the arrow keys.

Play a round before you read any further. Eat the red squares, do not hit a wall, and do not run
into yourself. When it is over, **Enter** takes you back to the title.

Everything on that board is something you have already written. The snake is a `List<Segment>` —
the same list of objects as the objects-together lesson's, with a tiny class holding two `int`
fields instead of a `Ball` holding five `float` ones. It moves one whole cell on a beat, which is
the grid step from the animation lesson. And the three screens are one `enum`, one variable and
one `switch`, exactly as the game-state lesson left them.

What is new is the arrangement. So the rest of this lesson builds that same program from nothing,
in three steps. Each step is a challenge, and each challenge's starter is the step before it,
finished — get one working and you carry it forward.

Two numbers set everything else up. The board is **32 columns by 18 rows**, and a cell is **20
pixels** square, so the canvas is `32 * 20` by `18 * 20` — the usual 640 by 360. From then on the
game thinks in columns and rows, never in pixels, and the only place pixels appear at all is the
drawing.

## Step 1: a snake that moves

A `Segment` is the smallest class in this course:

```csharp
class Segment
{
    public int col, row;

    public Segment(int atCol, int atRow)
    {
        col = atCol;
        row = atRow;
    }
}
```

Two `int` fields, a constructor, no methods at all. A snake is a `List<Segment>` of them, and the
rule that makes the whole thing work is that **`body[0]` is the head** and the rest of the list
trails away behind it. The snake starts as three segments across row `9` — the head at column
`2`, then columns `1` and `0` behind it — heading right.

Moving it looks like it should be hard and is not. You do not move every segment. You put a new
head on the front and take the last segment off the back:

```csharp
body.Insert(0, new Segment(nextCol, nextRow));
body.RemoveAt(body.Count - 1);
```

`Insert` is `Add` with a place to put it: `Add` always goes on the end, `Insert(0, ...)` goes on
the front. Two lines, and the whole snake appears to slide forward by one cell.

The direction is a pair of `int`s — `dirCol` and `dirRow`, each `-1`, `0` or `1`, never anything
else. That is the animation lesson's `stepX` and `stepY` with names that say what they index. The
arrows set them, and each arrow refuses the one turn that would fold the snake back into its own
neck:

```csharp
if (Keys.IsDown(Key.Left) && dirCol != 1) { dirCol = -1; dirRow = 0; }
```

Read the second half as "and we are not already going right". Without it, one careless press
turns the head straight into the segment behind it, and the game is over for a reason that feels
like a bug rather than a mistake.

:::key
A snake is a `List<Segment>` with the head at `body[0]`. Moving it is `Insert(0, newHead)` plus
`RemoveAt(body.Count - 1)` — a segment on the front, a segment off the back, and nothing in the
middle moves at all. The arrows change a direction, not a position; the timer from the animation
lesson decides when the direction is actually used.
:::

:::challenge c25a-move
The starter draws a snake that never moves. The board is already set up: `cols` is `32`, `rows`
is `18`, `cell` is `20`, `framesPerStep` is `6`, and `Setup` fills `body` with three segments at
row `9`, columns `2`, `1` and `0`, with `dirCol` at `1` and `dirRow` at `0` — the head at
`body[0]`, heading right. `DrawBoard()` already draws every segment as an `18` by `18` rectangle
at `part.col * cell, part.row * cell`. Three numbered comments mark the three places to write.

Where comment 1 is, inside `Step()`, write five things in this order:

1. `int nextCol = body[0].col + dirCol;` and `int nextRow = body[0].row + dirRow;`
2. wrap the column: if `nextCol` is less than `0`, set it to `cols - 1`, which is `31`; if
   `nextCol` is more than `cols - 1`, set it to `0`
3. wrap the row the same way, with `rows - 1`, which is `17`, and `0`
4. `body.Insert(0, new Segment(nextCol, nextRow));`
5. `body.RemoveAt(body.Count - 1);`

Where comment 2 is, in `Draw`, add the four arrow lines exactly as printed here:

```csharp
if (Keys.IsDown(Key.Left) && dirCol != 1) { dirCol = -1; dirRow = 0; }
if (Keys.IsDown(Key.Right) && dirCol != -1) { dirCol = 1; dirRow = 0; }
if (Keys.IsDown(Key.Up) && dirRow != 1) { dirCol = 0; dirRow = -1; }
if (Keys.IsDown(Key.Down) && dirRow != -1) { dirCol = 0; dirRow = 1; }
```

Where comment 3 is, still in `Draw`, add `if (Frame.Count % framesPerStep == 0) Step();` — one
step every `6` frames, five a second.

Change nothing else — not `DrawBoard`, not `Setup`, not the `Segment` class.

Two scripts check it. The first presses nothing for 60 frames. A step happens on frames 1, 7, 13
and so on, which is ten steps in 60 frames, so the head must be at column `12` of row `9`, with
the rest of the snake at columns `11` and `10`. The second script holds **Down** for frames 1 to
20, **Left** for 21 to 40 and **Right** for 41 to 60. Down for four steps puts the head at row
`13`; Left for three steps takes it from column `2` to column `1`, then `0`, then off the left
edge and back on at column `31`. Then **Right** is pressed — and it is the exact reverse of Left,
so it must be **ignored**: the last three steps carry on leftwards, and at frame 60 the snake is
at columns `30`, `29` and `28` of row `13`, head at `28`. Obey that press and it is at columns
`0`, `1` and `2` instead, at the other end of the board. Press **Check** when you are ready.
:::

## Step 2: food, and growing

Food is one cell, picked at random, and `Rand.Range(0, 32)` picks a column while
`Rand.Range(0, 18)` picks a row:

```csharp
void PlaceFood()
{
    foodCol = Rand.Range(0, cols);
    foodRow = Rand.Range(0, rows);
}
```

Column first, then row, every time — and that order matters more than it looks. The checker gives
every program the same sequence of random numbers, so two programs only see the same board if
they ask for those numbers in the same order. Call it once in `Setup` and once more each time the
food is eaten, and nowhere else.

The food is drawn as a **square**, not a circle: `Screen.Rect(foodCol * cell + 4, foodRow * cell
+ 4, 12, 12, Colour.Red)`. On a board made of squares a circle is genuinely harder to place by
eye — a `12` by `12` square inset `4` pixels sits neatly inside its cell and leaves no doubt
about which cell that is.

And growing is the smallest change in the whole lesson. `Step` already adds a head and drops a
tail. Eating means **not** dropping the tail:

```csharp
body.Insert(0, new Segment(nextCol, nextRow));
if (nextCol == foodCol && nextRow == foodRow)
{
    score = score + 1;
    PlaceFood();
}
else
{
    body.RemoveAt(body.Count - 1);
}
```

One `if`/`else` around a line you already had. The snake gets longer because a segment that
should have gone did not go, and that is the entire growth mechanic.

:::key
Growing is not adding a segment. It is **skipping the removal** — the head goes on either way, and
the tail only comes off when nothing was eaten. The thing doing the growing is the
objects-together lesson's list of objects, unchanged — `body` gains and loses `Segment`s exactly
as that lesson's list gained and lost balls. `Rand` is only fair if every program calls it in the
same order, so a game that uses it has to say exactly when and with what.
:::

:::challenge c25b-grow
The starter is your moving snake from step 1. Give it something to eat. Six numbered comments
mark the six places to write.

Where comment 1 is, at the top of the file: `int foodCol = 0;`, `int foodRow = 0;` and
`int score = 0;`.

Where comment 2 is, as the last line of `Setup`: `PlaceFood();`.

Where comment 3 is, above `DrawBoard`, the method that call needs:

```csharp
void PlaceFood()
{
    foodCol = Rand.Range(0, cols);
    foodRow = Rand.Range(0, rows);
}
```

The column first and the row second, `Rand.Range(0, cols)` and `Rand.Range(0, rows)` — that is
`0` to `31` and `0` to `17` — and called in exactly two places: comment 2 and comment 6, and
nowhere else.

Where comment 4 is, at the top of `DrawBoard`, before the loop, exactly:

`Screen.Rect(foodCol * cell + 4, foodRow * cell + 4, 12, 12, Colour.Red);`

Where comment 5 is, at the end of `DrawBoard`, after the loop, exactly:

`Screen.Text(10, 10, "Score: " + score, Colour.White);`

Where comment 6 is, inside `Step`: the `body.RemoveAt(body.Count - 1);` line below it must now
only run when nothing was eaten. Replace that one line with

```csharp
if (nextCol == foodCol && nextRow == foodRow)
{
    score = score + 1;
    PlaceFood();
}
else
{
    body.RemoveAt(body.Count - 1);
}
```

Change nothing else — not the wrapping, not the arrows, not the step timer.

Two scripts check it. Under the checker the first food always lands on column `2`, row `1` —
straight above the snake's head — so the first script simply holds **Up** for all 48 of its
frames and looks twice. At frame 40 the snake has taken seven steps up and is three segments at
rows `2`, `3` and `4` of column `2`, with the food still on the board above it and `Score: 0` in
the corner. At frame 48 it has taken the eighth step, onto the food: four segments now, at rows
`1`, `2`, `3` and `4`, `Score: 1`, and the food gone from column `2` and re-dealt to column `24`,
row `9`. A snake that eats without growing is three segments there; one that grows without moving
the food leaves the new square missing. The second script presses nothing for 60 frames, so the
snake runs right along row `9` and never meets the food at all: three segments at columns `12`,
`11` and `10`, the food still at column `2` row `1`, and `Score: 0`. Press **Check** when you are
ready.
:::

## Step 3: walls, teeth and three screens

The wrapping was always a placeholder. A real snake game has two ways to lose, and they are two
lines of the same test — the head's next cell is off the board, or the head's next cell is one
the snake is already on:

```csharp
bool dead = nextCol < 0 || nextCol > cols - 1 || nextRow < 0 || nextRow > rows - 1;
foreach (Segment part in body)
{
    if (part.col == nextCol && part.row == nextRow) dead = true;
}
```

The `foreach` walks every segment, the very last one included. Purists will tell you the tail is
about to move out of the way and should not count; they are right, and the difference shows up
about once a year. Counting it is one line shorter and easier to explain, so that is the rule
this game uses.

The three screens are the game-state lesson's, unchanged. An `enum` at the bottom of the file, a
`state` variable at the top, and a `switch` in `Draw` with a case each. The space bar starts a
round from the title, **Enter** goes back to the title from game over — a different key on
purpose, because the space bar is the one you were leaning on when you died.

The one thing worth planning is *where* the restart happens. Going back to `Title` is a state
change, and a state change is not a restart, so the title screen would show the corpse of the
last round unless something puts the snake back. So `Reset()` — a fresh three-segment `body`, the
direction back to right, the score back to `0` — is called on the way **out** of game over,
before the title is drawn, rather than on the way into a round:

```csharp
if (Keys.WasPressed(Key.Enter))
{
    Reset();
    state = State.Title;
}
```

And `Setup` calls the same `Reset()` to build the snake in the first place, so the very first
round starts exactly the way every later one does.

:::key
Two ways to die, one `bool`: off the board, or onto yourself. Three screens, one `enum` and one
`switch` — the game-state lesson's, unchanged, down to the space bar starting a round and
**Enter** leaving one — and a `Reset()` called when you leave the game-over screen, so the title
always shows a new snake rather than the one that just died.
:::

:::challenge c25c-game-over
The starter is your finished step 2, wrapping and all. Take the wrapping out and give the game a
beginning and an end. There are no comment markers this time — every edit below names the code it
replaces.

At the top of the file, under `int score = 0;`, add `State state = State.Title;`.

At the very bottom, under the `Segment` class, add `enum State { Title, Playing, GameOver }`.

Add a `Reset()` method above `PlaceFood()` that builds a fresh snake and clears the round:

```csharp
void Reset()
{
    body = new List<Segment>();
    for (int i = 0; i < 3; i++) body.Add(new Segment(startCol - i, startRow));
    dirCol = 1;
    dirRow = 0;
    score = 0;
}
```

Then in `Setup`, replace the `for` loop that fills `body` with a single call to `Reset();`, so
`Setup` reads `Screen.Size(...)`, `Reset();`, `PlaceFood();` in that order.

In `Step`, delete the four wrapping lines and put the two ways of dying in their place, above the
`Insert`:

```csharp
bool dead = nextCol < 0 || nextCol > cols - 1 || nextRow < 0 || nextRow > rows - 1;
foreach (Segment part in body)
{
    if (part.col == nextCol && part.row == nextRow) dead = true;
}
```

`cols - 1` is `31` and `rows - 1` is `17`. Every segment counts, the last one included. Then wrap
the rest of `Step` — the `Insert` and the `if`/`else` you wrote in step 2 — in
`if (dead) { state = State.GameOver; } else { ... }`.

Finally, in `Draw`, put a `switch (state)` around what is there now, with three cases, each
ending in `break;`:

- `case State.Title:` calls `DrawBoard();`, then
  `Screen.Text(120, 160, "SNAKE - SPACE TO PLAY", Colour.White);`, then sets `state` to
  `State.Playing` when `Keys.WasPressed(Key.Space)`.
- `case State.Playing:` is exactly what `Draw` did before — the four arrow lines, the
  `if (Frame.Count % framesPerStep == 0) Step();` line, and `DrawBoard();`.
- `case State.GameOver:` calls `DrawBoard();`, then
  `Screen.Text(190, 140, "GAME OVER", Colour.White);` and
  `Screen.Text(130, 180, "ENTER FOR TITLE", Colour.White);`, then, when
  `Keys.WasPressed(Key.Enter)`, calls `Reset();` and sets `state` to `State.Title`.

Those three texts at exactly those positions. `Screen.Clear(Colour.Black);` stays where it is, at
the top of `Draw` and outside the switch.

Three scripts check it, and all three press the space bar on frame 5. The first then presses
nothing for 200 frames: the snake runs right along row `9` from column `2`, a step every `6`
frames, and on the thirtieth step it walks into the wall past column `31`, so frame 200 must show
`GAME OVER`, `ENTER FOR TITLE` and a dead snake still sitting at columns `31`, `30` and `29` —
a program that still wraps is back at the left-hand side with no message at all. The second runs
the same round and presses **Enter** on frame 200: frame 240 must be the title screen with
`SNAKE - SPACE TO PLAY` and a brand-new three-segment snake at columns `2`, `1` and `0` of row
`9`, so leaving `Reset();` out shows the old snake at the far wall instead. The third makes it
bite itself: **Up** from frame 6 to 49 walks it up column `2` onto the food at row `1`, so it is
four segments long; **Right** for 50 to 55, **Down** for 56 to 61 and **Left** for 62 to 67 then
walk it round a two-by-two square and straight back into its own tail. Frame 100 must show
`GAME OVER` with `Score: 1` and the four segments where they stopped. Press **Check** when you
are ready.
:::

## Make it yours

That is Snake: three steps, one small class, one list, one enum. The whole game is about a
hundred and thirty lines, and the only genuinely clever line in it is `Insert(0, ...)` followed by
`RemoveAt(body.Count - 1)`.

This lesson has no editable sample of its own, because you already have three editors full of the
game. Everything below goes in the **step 3 editor** — the `c25c-game-over` challenge, the last
one on this page — where the finished game lives. Nothing you do there can break anything, and
**Reset** always brings the starter back.

:::try
Three changes, all in the step 3 editor.

Make it speed up. `framesPerStep` is a plain variable, so nothing stops you changing it while the
game runs. Inside the `if` where you score, add `if (score % 5 == 0 && framesPerStep > 2)
framesPerStep = framesPerStep - 1;` — a cell faster every five points, down to a floor of two
frames a step. Take the floor away and find out whether you can survive long enough to reach
zero, and what a `%` by zero does about it.

Add a second food. Two more variables, `foodCol2` and `foodRow2`, a second `PlaceFood`-style
method for them, a second `Screen.Rect` in `DrawBoard`, and a second test in `Step`. It works,
and it is four edits in four places to say one thing twice — which is exactly the itch that
`List<Segment>` scratched for the snake and that a `class Food` would scratch here.

Then add walls. A `List<Segment>` called `obstacles`, filled in `Setup` with a few cells, drawn
in `DrawBoard` in `Colour.Grey`, and one more `foreach` in `Step` setting `dead = true` when the
head lands on one. The class you wrote for the snake describes a wall perfectly well: a segment
is a cell, and a cell is a cell whatever is standing on it.
:::

That is the last game in the course. What is left is Part 4, Going further — one page with three
programs on it to read, run and pull apart: a flock of agents that seek and flee, Conway's Game of
Life on a grid of ints, and a firework of short-lived particles. Nothing there asks you for
anything, and it ends by pointing at where all of this actually goes.

Everything in Part 3 was chosen because it is what Unity is made of: a `class` with public fields
you can see and set, an `Update` that runs once a frame, a `List` of objects the game owns, a
`Vector2` that does its own arithmetic, and a state machine deciding which screen you are looking
at. Unity gives you the window, the sprites and the physics. The thinking is the part you have
just spent six lessons doing.
