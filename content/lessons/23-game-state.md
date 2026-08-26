# Game state

Every program in this course so far starts doing its one thing the moment you press **Run**, and
keeps doing it until you close the page. A real game does not. It shows a title and waits for
you. It plays a round. The round ends, and it offers you another one.

That is one program behaving in three different ways, and the thing that decides which way it is
behaving is a single variable.

## A name for each screen

You could keep that variable as an `int` — `0` for the title, `1` for playing, `2` for game over
— and spend the rest of the project remembering which number is which. C# has something better.
An **enum** is a type whose values are names you make up:

```csharp
enum State { Title, Playing, GameOver }
```

One line, and there is now a type called `State` with exactly three possible values:
`State.Title`, `State.Playing` and `State.GameOver`. Nothing else fits in a `State` — not `4`,
not `"title"`, not a misspelling — so a wrong screen name is a compiler error rather than a
silent afternoon of wondering why the title never shows.

An enum is a type declaration, the same as a class, so it goes at the **bottom of the file**
under `Game.Run`. The variable that holds one goes at the top with everything else:

```csharp
State state = State.Title;
```

And then the decisions lesson's `switch` — same `case`, same `break`, a value of a new type —
picks which screen to draw:

:::run s23-states Three screens, one variable, one switch.

Press **Run**, click the canvas, and press the space bar.

Then read `Draw` again and notice what is *not* in it. There is no "has the game started" `bool`,
no flag saying the ball is allowed to move, no `if` wrapped round the drawing. The ball moves in
exactly one place — inside `case State.Playing` — so on the title screen it cannot move, because
the line that moves it never runs.

:::key
An `enum` is a type whose values are names you invent, declared at the **bottom of the file**
with the classes. One variable holds which state the game is in, and a `switch` on that variable
picks the screen. Code inside a `case` runs only in that state, and that is the whole trick.
:::

## The key that is not the space bar

The game-over screen goes back to the title on **Enter**, not on the space bar, and that is on
purpose. Space is the key you were leaning on when you lost. A game-over screen that listens for
the same key clears itself the instant it appears, before you have read a word of it. Give each
screen a key of its own and the problem never arises.

## Starting over

Go back to the sample above, let the ball fall off the bottom, press **Enter** for the title, and
press the space bar for another go. The round ends immediately.

Nothing is broken. `state` went back to `Title`, but `ballY` did not: it is still somewhere below
the bottom of the screen, where the last round left it. A state says *what the game is doing*. It
does not put the numbers back.

Putting the numbers back is a job worth a name of its own — `Reset` — written once, in one place,
rather than sprinkled through the case that starts the round:

:::run s23a-restart A round you can play, a score, and a restart that really does start over.

`Reset()` is an ordinary method, declared exactly like `Setup` and `Draw` and called by name. It
puts the ball back at `startY`, gives it a small push upwards, and — the part that is easy to
forget — writes down the frame the round began on. Tap the space bar to keep the ball off the
bottom; the longer you last, the higher the score.

That score is `(Frame.Count - startFrame) / 30`. `Frame.Count` only ever counts up, from the
moment the program started, so it is no use as a score on its own. Subtracting the frame the
round began on turns it into "frames this round", and dividing by the thirty frames in a second
turns that into seconds. Both are `int`, so the division throws the remainder away and leaves a
whole number of seconds — which is all a score needs.

:::key
A state change is not a restart. Going back to `Title` puts the **screen** back; a `Reset()` that
sets every one of the round's variables to its starting value is what puts the **game** back.
Write it once, and call it from the case that starts a round.
:::

## A round with a clock

Not every round ends when something goes wrong. Plenty end when the time runs out, and that is
the same `Frame.Count - startFrame` subtraction read the other way round: instead of counting up
to make a score, count towards a limit.

:::edit s23b-timer-state Ten seconds on the clock, and then the round is over.

`roundFrames` is `300` — ten seconds at thirty frames a second. `Frame.Count - startFrame` is how
many frames this round has lasted, so the round ends the moment that reaches `300`, and
`(roundFrames - (Frame.Count - startFrame)) / 30` is the same subtraction shown to the player as
whole seconds remaining.

:::try
Add a fourth state. Pausing is a state like any other, and it costs three small edits.

Put `Paused` in the enum at the bottom of the file:

```csharp
enum State { Title, Playing, GameOver, Paused }
```

Then, inside `case State.Playing:` and above its `break;`, add the way in:

```csharp
if (Keys.WasPressed(Key.P)) state = State.Paused;
```

And above the closing brace of the switch, a whole new case with the way out:

```csharp
case State.Paused:
    Screen.Text(250, 170, "PAUSED", Colour.White);
    if (Keys.WasPressed(Key.P)) state = State.Playing;
    break;
```

Run it and press **P**. The ball stops dead, because the line that moves it lives in the
`Playing` case and the `Playing` case is not running. Now pause for a few seconds and unpause:
the clock carried on without you, because `Frame.Count` never stops. Fixing that is a couple of
lines — remember the frame you paused on, and when you unpause, push `startFrame` forward by the
number of frames the pause lasted.
:::

## Challenge

:::challenge c23-game-over
The starter is the motion lesson's bouncing ball inside the class from your first class lesson:
`new Ball(320, 180, 4, 3)`, a `radius` of `20`, and a `Move()` that bounces it off all four
edges. It starts playing the moment you press **Run** and it never ends. Give it a title screen
and a game over. Four numbered comments mark the four places to write.

Where comment 1 is, at the top of the file:

```csharp
State state = State.Title;
```

Where comment 3 is, under `Game.Run`:

```csharp
enum State { Title, Playing, GameOver }
```

Where comment 4 is, inside `Move()`, the ball has to be able to fall out of the bottom of the
world, so half of that bounce must go. Change

```csharp
if (y < radius || y > Screen.Height - radius) speedY = -speedY;
```

to

```csharp
if (y < radius) speedY = -speedY;
```

Where comment 2 is, in the top-level `Draw`, replace `ball.Move();` and `ball.Draw();` with a
`switch (state)` holding three cases, each ending in `break;`.

`case State.Title:` calls `ball.Draw();` — drawing it without moving it, so it sits still — then
`Screen.Text(220, 240, "Press SPACE", Colour.White);`, that exact text at that exact position,
and sets `state` to `State.Playing` when `Keys.WasPressed(Key.Space)`.

`case State.Playing:` calls `ball.Move();` and `ball.Draw();`, and then sets `state` to
`State.GameOver` when `ball.y > Screen.Height + ball.radius` — one radius past the bottom, by
which point the ball is out of sight.

`case State.GameOver:` draws `Screen.Text(240, 170, "GAME OVER", Colour.White);` and, when
`Keys.WasPressed(Key.Enter)`, calls `ball.Reset();` and sets `state` back to `State.Title`.
`Reset()` is already written for you: it puts the ball back at `(320, 180)`.

Change nothing else — not the starting position, not the two speeds, not the radius.

Three scripts check it. The first runs ten frames and presses nothing: the ball must still be
sitting at `(320, 180)` with `Press SPACE` below it, so a ball that moves on the title screen is
caught on frame 10. The second presses the space bar on frame 5 and runs for 200 frames; the ball
crosses the bottom edge on frame 72, so frame 60 catches it in mid-fall at `(540, 345)` and frame
200 must show `GAME OVER` and nothing else. The third presses the space bar on frame 5 and
**Enter** on frame 150: frame 140 is still the game-over screen, and by frame 200 the title is
back with the ball returned to `(320, 180)` — leave `ball.Reset();` out and the title screen
comes back empty. Press **Check** when you are ready.
:::
