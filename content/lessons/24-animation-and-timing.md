# Animation & timing

Thirty frames go past every second, and most of what a game does should not happen on all of
them. A shot fired on every frame the trigger is held is thirty shots a second. A picture that
changes on every frame is a blur. A snake that slides one pixel at a time is not a snake.

Everything in this lesson is one idea wearing three costumes: **do the thing, then wait.**

## Counting down

The waiting part is a number you take one off every frame, and test:

```csharp
cooldown = cooldown - 1;
if (cooldown <= 0)
{
    // the thing that happens
    cooldown = 30;
}
```

Two lines of bookkeeping around one line of work. Winding the timer back up on that last line is
the part that gets left out, and leaving it out is loud: `cooldown` sits at or below zero from
then on and the thing happens on every single frame.

:::run s24-timer A flash every thirty frames, with the countdown on screen.

`<= 0`, not `== 0`. They behave the same here, but the moment anything else touches the timer — a
power-up that takes five off it, a pause that skips a frame — a test for exactly zero can be
stepped straight over, and a timer that never fires again is a horrible bug to go looking for.
`<= 0` cannot be missed.

:::key
A timer is a number you count down and test. Take one off it every frame; when it reaches `0`, do
the thing **and set it back up again**. Test with `<= 0`, never `== 0`.
:::

## Every n frames

When something should happen on a steady beat and nothing else is going to interfere with it,
there is a shorter way to say the same thing. `Frame.Count` counts frames from the start of the
program, and `%` — the remainder operator from the maths lesson — comes out `0` exactly once
every `n`:

```csharp
if (Frame.Count % 6 == 0)
```

True on frames 0, 6, 12, 18 and so on: five times a second. That is fast enough to read as
movement and slow enough to see, which is roughly the rate hand-drawn animation has always used.

An animation is a list of pictures and an index saying which one you are on:

:::run s24a-frames Four pictures, five changes a second, round and round.

`radii` and `tints` are two lists filled in `Setup`, and `step` is the index into both of them.
Every sixth frame the index moves on by one, and when it runs off the end of the list it goes
back to `0` — which is what turns a sequence into a loop instead of a crash.

:::key
`Frame.Count % n == 0` is true once every `n` frames: a steady beat with nothing to wind back up.
A frame animation is a **list of pictures** plus an **index** that steps along it and wraps round
to `0` at the end.
:::

## Moving a whole cell at a time

Everything that has moved so far has moved a few pixels a frame, and where it ends up is wherever
the arithmetic happened to put it. Some games do not work that way. In Snake, in Pac-Man, in
almost any puzzle game, things live on a grid: a piece is *in* a square or it is not, and it moves
one whole square at a time, on a beat.

That is the two ideas above put together. The arrows choose a direction and change nothing else,
the timer decides when the square actually moves, and when it does move it moves a whole `cell`:

:::edit s24b-grid-step One twenty-pixel cell every six frames, in the last direction you pressed.

`stepX` and `stepY` are not a speed. They are `-1`, `0` or `1` — a direction, nothing more — and
the distance comes from `cell`. Multiplying the two is what makes every move exactly one square,
whichever way it is going.

Hold an arrow down and the square does not speed up; it travels the same distance per beat in a
new direction. Let go and it keeps going — straight off the edge of the board and out of sight,
because nothing ever sets `stepX` and `stepY` back to zero, and nothing here says the board has
edges. Both of those are how a snake behaves, and both came free.

:::try
Change `framesPerStep` to `3` and run it: the same distance per step, twice as often. Then try
`12` and watch it turn ponderous. The speed of a grid game is not the size of the step, it is how
often the step happens.

Then give the board edges that wrap round. Inside the same `if`, under the two lines that change
`x` and `y`, add four more:

```csharp
if (x < 0) x = Screen.Width - cell;
if (x > Screen.Width - cell) x = 0;
if (y < 0) y = Screen.Height - cell;
if (y > Screen.Height - cell) y = 0;
```

Drive off the right-hand edge and the square comes back on the left. Then delete those four lines
and try the other rule instead: end the round when the square leaves the board, using the enum
and the `switch` from the game-state lesson.
:::

## Challenge

:::challenge c24-cooldown
The starter is a ship — a `60` by `20` rectangle at `(290, 340)` — and a `List<Shot>`. `Shot` is
written for you: `x`, `y`, a `speed` of `6`, a `Move()` that takes `speed` off `y`, and a
`Draw()` that draws a circle of radius `12`. The trigger works too, and that is the problem.
`Keys.IsDown(Key.Space)` is true on every frame the key is held, so holding the space bar for two
seconds fires sixty shots, each one sitting on top of the last. Give it a cooldown. Five numbered
comments mark the five places to write.

Where comment 1 is, at the top of the file:

```csharp
int cooldown = 0;
```

Where comment 2 is, as the first thing `Draw` does after clearing the screen:

```csharp
cooldown = cooldown - 1;
```

Where comment 3 is, give the `if` below it a second condition, so it reads
`if (Keys.IsDown(Key.Space) && cooldown <= 0)`, and add `cooldown = 15;` inside it, on the line
after `shots.Add(...)`.

Where comment 4 is, inside the loop: when `shots[i].y` is less than `-12` — one radius past the
top, so the shot is out of sight — remove it with `shots.RemoveAt(i);` and then step the index
back with `i--;`.

Where comment 5 is, after the loop:

```csharp
Screen.Text(10, 10, "Shots: " + shots.Count, Colour.White);
```

That exact text, at that exact position. Change nothing else: not the ship, not the shot's speed
or radius, not where a shot starts.

Two scripts check it. The first holds the space bar down for all fifty of its frames. A cooldown
of `15` fires on frames 1, 16, 31 and 46 — four shots, and no more — so frame 50 must read
`Shots: 4`, with the four shots at `y = 40`, `130`, `220` and `310`, ninety pixels apart, and
frame 25 must read `Shots: 2`. Fire on every frame instead and there are fifty of them in one
solid column. The second script holds the space bar for thirty frames and then runs on to frame
90, by which time both shots have long since left the top of the screen: it must read `Shots: 0`,
which only happens if the removal is there. Press **Check** when you are ready.
:::
