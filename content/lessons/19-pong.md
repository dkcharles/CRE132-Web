# Mini-game: Pong

Six lessons of parts: shapes, motion, the keyboard, the mouse, lists, collision. This lesson
introduces nothing new at all. It puts the parts together into a game two people can actually
play sitting at the same keyboard — the oldest video game there is, and still one of the best.

:::run s19-pong The finished game. Click the canvas first, then W and S move the left paddle, the up and down arrows the right.

Everything in those fifty-odd lines is something you have already written: two rectangles whose
`y` a key changes, a circle whose `x` and `y` change every frame, a `-` in front of a speed to
turn it round, an overlap test, a number that goes up. What you have not done is put them
together, so the rest of this lesson builds that exact program from nothing, in three steps.

Each step is a challenge, and each challenge's starter is the step before it, finished. Get one
working and you carry it forward — which is how real programs get written, one working version
at a time, never all at once.

## Step 1: two paddles

A Pong paddle is the keyboard lesson's paddle turned on its side: a `Screen.Rect` whose `y` two
keys change, clamped so it cannot walk off the top or the bottom.

There are two of them, and they must not fight over the same keys, so the left paddle takes
`Key.W` and `Key.S` and the right one takes `Key.Up` and `Key.Down`. Both are `16` pixels wide
and `80` tall — the left at `x = 20`, the right at `x = 604`, which is `640 - 16 - 20`, the same
margin in from the other edge.

```csharp
if (Keys.IsDown(Key.W)) leftY = leftY - 6;
if (Keys.IsDown(Key.S)) leftY = leftY + 6;
if (leftY < 0) leftY = 0;
if (leftY > 280) leftY = 280;
```

`280` is `360 - 80`: the screen's height less the paddle's, because a rectangle is drawn from
its **top** edge. It is the same sum as the keyboard lesson's `640 - 100`, rotated a quarter
turn.

:::key
Two paddles are one paddle's code, twice, with different variables and different keys. Each one
needs its own pair of clamps — `0` at the low end, `Screen.Height` less the paddle's height at
the high end.
:::

:::challenge c19a-paddles
The starter draws both paddles and nothing moves them. They are `16` wide and `80` tall, the
left at `x = 20` and the right at `x = 604`, and both start at `140` — the middle of a `360`
tall screen.

Where the first comment is, add four lines for the left paddle:

- when `Key.W` is down, `leftY = leftY - 6`
- when `Key.S` is down, `leftY = leftY + 6`
- if `leftY` is less than `0`, set it to `0`
- if `leftY` is more than `280`, set it to `280`

Where the second comment is, add the same four lines for `rightY`, using `Key.Up` and
`Key.Down` and the same `6`, `0` and `280`.

Leave the two `Screen.Rect` calls exactly as they are. The checker runs two scripts. In the
first it holds **W** and **Up** together for 30 frames, so both paddles run into the top edge
and stop at `0` rather than sliding off it. The second holds **S** and **Down** together for 60
frames and looks twice. At frame 20 each paddle should have taken exactly twenty steps of `6`
down from `140`, which puts both at `260` — a paddle that moves by `5` or by `7` is somewhere
else by then. At frame 60 both should be pinned against the bottom at `280` rather than far
past it. Press **Check** when you are ready.
:::

## Step 2: the ball

The ball is the motion lesson's bouncing circle with one extra rule. It has a position and two
speeds, it moves every frame, and it turns round when it meets the top or the bottom:

```csharp
bx = bx + speedX;
by = by + speedY;
if (by < 12 || by > 348) speedY = -speedY;
```

`12` is the ball's radius, so the test turns it round when its rim touches the wall rather than
when half of it is already outside; `348` is `360 - 12` at the other end.

Left and right are different. The wall is a paddle that moves, and hitting it flips `speedX`
instead of `speedY`. The test is the collision lesson's rectangle-grown-by-a-radius, written for
a paddle at `x = 20`, `16` wide and `80` tall:

```csharp
if (bx > 8 && bx < 48 && by > leftY - 12 && by < leftY + 92) speedX = -speedX;
```

Read the four numbers as the paddle's four edges pushed out by the ball's radius: `20 - 12` is
`8`, `20 + 16 + 12` is `48`, and `80 + 12` is `92`. The right paddle's test is the same shape
around `x = 604`: `604 - 12` is `592` and `604 + 16 + 12` is `632`.

And if the ball gets past a paddle it keeps going, off the screen, forever. That is a miss, and
a miss puts the ball back in the middle:

```csharp
if (bx < 0 || bx > 640)
{
    bx = 320;
    by = 180;
}
```

:::key
A bounce off a wall flips `speedY`; a bounce off a paddle flips `speedX`, but only when the
overlap test says the paddle was actually there. Grow the paddle's rectangle by the ball's
radius and the test is four comparisons you have already written once.
:::

:::challenge c19b-ball
The starter is your two paddles plus a ball that sits in the middle doing nothing: `bx = 320`,
`by = 180`, `speedX = 4`, `speedY = 5`, drawn as a circle of radius `12`.

Where the comment is, add five things in this order:

1. `bx = bx + speedX;` then `by = by + speedY;`
2. the wall bounce: `if (by < 12 || by > 348) speedY = -speedY;`
3. the left paddle, exactly this line:
   `if (bx > 8 && bx < 48 && by > leftY - 12 && by < leftY + 92) speedX = -speedX;`
4. the right paddle, exactly this line:
   `if (bx > 592 && bx < 632 && by > rightY - 12 && by < rightY + 92) speedX = -speedX;`
5. the miss: when `bx` is less than `0` **or** greater than `640`, set `bx` to `320` and `by`
   to `180`

Change nothing else — not the two speeds, not the radius `12`, not the paddles you finished in
step 1. The checker runs three scripts. The first presses no keys for 240 frames and compares
frames 40, 120 and 240; in that time the ball bounces off the bottom at frame 34, reflects off
the right paddle at frame 69, bounces off the top at 102 and the bottom again at 170, and
reflects off the left paddle at 207, so leaving any one of the five lines out puts it somewhere
else. The second holds **Down** for 30 frames, which slides the right paddle out of the way; the
ball leaves the screen on the **right** twice, and only a program that puts it back at
`(320, 180)` has a ball left to draw at frames 90 and 200. The third holds **S** for 30 frames
instead, dropping the *left* paddle out of the way, so the ball comes back across the screen and
leaves on the **left** at frame 219 — the other half of the same test, and the reason the reset
has to catch both edges. Press **Check** when you are ready.
:::

## Step 3: the score

A score is a variable that goes up, and `Screen.Text` puts it on the canvas — nothing you have
not done since the very first graphics lesson. What is new is *which* number goes up, and that
comes for free out of the miss you just wrote: a ball that leaves on the left got past the left
player, so the **right** player scores.

So the one `if` that handled both misses becomes two, because the two sides no longer do the
same thing, and the last line of `Draw` draws the result:

```csharp
Screen.Text(300, 10, left + " : " + right, Colour.White);
```

`left + " : " + right` glues a number, a string and another number into one string, exactly as
`"Score: " + score` did in the collision lesson. Drawing it last means it sits on top of
everything else rather than under the ball.

:::key
Two counters, one `if` each. `Screen.Text(x, y, left + " : " + right, Colour.White)` turns them
into something the players can see — and a game whose score is only in a variable is a game
nobody can win.
:::

:::challenge c19c-score
The starter is your finished ball from step 2. Give it a score.

At the top of the file, beside the other variables, add `int left = 0;` and `int right = 0;`.

Then replace the miss — the `if (bx < 0 || bx > 640)` block — with two separate `if`s:

- `if (bx < 0)`: set `bx` to `320`, set `by` to `180`, and set `right` to `right + 1`
- `if (bx > 640)`: set `bx` to `320`, set `by` to `180`, and set `left` to `left + 1`

Finally, as the very last line of `Draw`, after all three shapes are drawn, add exactly:

`Screen.Text(300, 10, left + " : " + right, Colour.White);`

with a space either side of the colon, so a score reads `0 : 1`. Change nothing else.

The checker runs two scripts. In the first it holds **S** for 30 frames, so your left paddle
drops to the bottom and the ball gets past it: at frame 240 the canvas must read `0 : 1`. In the
second it holds **Down** for 30 frames instead, so the right paddle is the one out of the way:
at frame 100 the canvas must read `1 : 0`, and at frame 200 `2 : 0`. A program that counts only
one of the two sides passes one script and fails the other. Press **Check** when you are ready.
:::

## Make it yours

That is Pong: three steps, fifty lines, no idea in it that is younger than the last six lessons.
This lesson has no editable sample of its own, because you already have three editors full of
the game — go back up to any of the three challenges and keep typing in it. Nothing you do there
can break anything, and **Reset** always brings the starter back.

:::try
Two changes worth trying, both in the step 3 editor.

Make the rally get harder. Each of the two paddle `if`s carries a single statement with no
`{ }` around it, so put braces round it first — otherwise the line you add runs on every frame
instead of only on a hit. Then, next to `speedX = -speedX;`, add `speedY = speedY * 1.1;` so
every return makes the ball a little steeper. Play until you lose,
then try `1.3` and find out how quickly a good idea becomes an unplayable one.

Then add a second ball. The many-things lesson's `List<double>` is exactly the tool: keep
`bxs`, `bys`, `sxs` and `sys` as four lists, add two balls to them in `Setup`, and replace the
ball's own lines with a `for` loop over `bxs.Count` doing the same work at index `i`. It is
fiddly — four lists that must stay in step is a lot of bookkeeping for one bouncing circle.
Notice how fiddly, because that feeling is the reason the next part of the course exists.
:::

Four lists to describe two balls is a bad way to say a simple thing. In Part 3, Objects and real
games, the first lesson builds a `class Ball` that holds `x`, `y`, `speedX` and `speedY`
together as one thing you can make as many of as you like — and the whole of step 2 becomes a
single line inside it. Pong is the last game you will write with loose variables.
