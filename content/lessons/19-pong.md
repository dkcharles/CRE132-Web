# Mini-game: Pong

Six lessons of parts: shapes, motion, the keyboard, the mouse, lists, collision. This lesson
introduces nothing new at all. It puts the parts together into a game two people can actually
play sitting at the same keyboard — the oldest video game there is, and still one of the best.

:::run s19-pong The finished game. Click the canvas first, then W and S move the left paddle, the up and down arrows the right.

Everything in those seventy-odd lines is something you have already written: two rectangles whose
`y` a key changes, a circle whose `x` and `y` change every frame, a `-` in front of a speed to
turn it round, an overlap test, a number that goes up. What you have not done is put them
together, so the rest of this lesson builds that exact program from nothing, in three steps.

Roughly a third of the file is the block of variables at the top, and that is deliberate. Every
number the game is built from — how wide a paddle is, how fast it moves, how far down it may go —
is written down exactly once there, with a name saying what it is, and the rest of the program
says the name. It is the habit the motion and keyboard lessons started; a game is where it starts
paying you back, because the same handful of numbers turn up in the drawing, the clamping and the
collision test all at once.

Each step is a challenge, and each challenge's starter is the step before it, finished. Get one
working and you carry it forward — which is how real programs get written, one working version
at a time, never all at once.

## Step 1: two paddles

A Pong paddle is the keyboard lesson's paddle turned on its side: a `Screen.Rect` whose `y` two
keys change, clamped so it cannot walk off the top or the bottom.

There are two of them, and they must not fight over the same keys, so the left paddle takes
`Key.W` and `Key.S` and the right one takes `Key.Up` and `Key.Down`. Every number they are built
from has a name at the top of the file: `paddleWidth` is `16`, `paddleHeight` is `80`,
`paddleSpeed` is `6`, and `leftX` is `20`. The right paddle's `rightX` is not typed out at all —
it is declared as `screenWidth - paddleWidth - leftX`, which works out at `604`, the same margin
in from the other edge without you doing the sum.

```csharp
if (Keys.IsDown(Key.W)) leftY = leftY - paddleSpeed;
if (Keys.IsDown(Key.S)) leftY = leftY + paddleSpeed;
if (leftY < 0) leftY = 0;
if (leftY > paddleMaxY) leftY = paddleMaxY;
```

`paddleMaxY` is declared the same way, as `screenHeight - paddleHeight`: `360 - 80`, or `280`.
The screen's height less the paddle's, because a rectangle is drawn from its **top** edge — the
same sum as the keyboard lesson's `640 - 100`, rotated a quarter turn. Written as the subtraction
rather than as a flat `280`, a taller paddle still stops in exactly the right place.

:::key
Two paddles are one paddle's code, twice, with different variables and different keys. Each one
needs its own pair of clamps — `0` at the low end, `paddleMaxY` (the screen's height less the
paddle's) at the high end. Both paddles share the one `paddleSpeed`, so making the game faster is
one number in one place.
:::

:::challenge c19a-paddles
The starter draws both paddles and nothing moves them. Their numbers are already declared at the
top of the file: `paddleWidth` is `16`, `paddleHeight` is `80`, `paddleSpeed` is `6`, `leftX` is
`20`, `rightX` comes out at `604`, and `paddleMaxY` comes out at `280`. Both paddles start at
`140` — the middle of a `360` tall screen.

Where the `// Move the left paddle` comment is, add four lines for the left paddle:

- when `Key.W` is down, `leftY = leftY - paddleSpeed`
- when `Key.S` is down, `leftY = leftY + paddleSpeed`
- if `leftY` is less than `0`, set it to `0`
- if `leftY` is more than `paddleMaxY`, set it to `paddleMaxY`

Where the `// Move the right paddle` comment is, add the same four lines for `rightY`, using
`Key.Up` and `Key.Down` and the same `paddleSpeed`, `0` and `paddleMaxY`.

Leave the two `Screen.Rect` calls exactly as they are. The checker runs two scripts. In the
first it holds **W** and **Up** together for 30 frames, so both paddles run into the top edge
and stop at `0` rather than sliding off it. The second holds **S** and **Down** together for 60
frames and looks twice. At frame 20 each paddle should have taken exactly twenty steps of
`paddleSpeed`, which is `6`, down from `140`, which puts both at `260` — a paddle that moves by
`5` or by `7` is somewhere else by then. At frame 60 both should be pinned against the bottom at
`paddleMaxY`, which is `280`, rather than far past it. Press **Check** when you are ready.
:::

## Step 2: the ball

The ball is the motion lesson's bouncing circle with one extra rule. It has a position and two
speeds, it moves every frame, and it turns round when it meets the top or the bottom:

```csharp
bx = bx + ballSpeedX;
by = by + ballSpeedY;
if (by < ballRadius || by > screenHeight - ballRadius) ballSpeedY = -ballSpeedY;
```

`ballRadius` is `12`, so the test turns the ball round when its rim touches the wall rather than
when half of it is already outside; `screenHeight - ballRadius` is `360 - 12`, or `348`, at the
other end. The two speeds are called `ballSpeedX` and `ballSpeedY` rather than plain `speedX` and
`speedY`, because there is a `paddleSpeed` in this program too and a name has to say *whose*
speed it is.

Left and right are different. The wall is a paddle that moves, and hitting it flips `ballSpeedX`
instead of `ballSpeedY`. The test is the collision lesson's rectangle-grown-by-a-radius — and
with the paddle's numbers already named, it more or less writes itself:

```csharp
if (bx > leftX - ballRadius && bx < leftX + paddleWidth + ballRadius &&
    by > leftY - ballRadius && by < leftY + paddleHeight + ballRadius)
    ballSpeedX = -ballSpeedX;
```

Each of the four comparisons is one edge of the paddle pushed out by the ball's radius. Work them
out and you get the numbers you would otherwise have had to type: `20 - 12` is `8`,
`20 + 16 + 12` is `48`, and `80 + 12` is `92`. The right paddle's test is those same four
comparisons with `rightX` in place of `leftX` and `rightY` in place of `leftY` — `604 - 12` is
`592`, `604 + 16 + 12` is `632` — which is the whole point of the names: the second test is the
first one with two words changed, not four fresh numbers to get right.

The condition is split over two lines with the `&&` left at the end of the first. C# does not
care where you break a line: it reads on until the `)` closes the condition, and the statement
that follows is still the one statement the `if` controls.

And if the ball gets past a paddle it keeps going, off the screen, forever. That is a miss, and
a miss puts the ball back in the middle:

```csharp
if (bx < 0 || bx > screenWidth)
{
    bx = centreX;
    by = centreY;
}
```

`centreX` and `centreY` are declared as `screenWidth / 2` and `screenHeight / 2` — `320` and
`180`, the middle of the screen, and the very place `bx` and `by` started.

:::key
A bounce off a wall flips `ballSpeedY`; a bounce off a paddle flips `ballSpeedX`, but only when
the overlap test says the paddle was actually there. Grow the paddle's rectangle by
`ballRadius` and the test is four comparisons you have already written once.
:::

:::challenge c19b-ball
The starter is your two paddles plus a ball that sits in the middle doing nothing. Four more
names are declared for you at the top of the file: `ballRadius` is `12`, `ballSpeedX` is `4`,
`ballSpeedY` is `5`, and `centreX`/`centreY` come out at `320` and `180`, where `bx` and `by`
start.

Where the comment is, add five things in this order:

1. `bx = bx + ballSpeedX;` then `by = by + ballSpeedY;`
2. the wall bounce:
   `if (by < ballRadius || by > screenHeight - ballRadius) ballSpeedY = -ballSpeedY;`
3. the left paddle test, exactly as printed below this list
4. the right paddle test: those same three lines again, with `rightX` in place of `leftX` and
   `rightY` in place of `leftY`
5. the miss: when `bx` is less than `0` **or** greater than `screenWidth`, set `bx` to `centreX`
   and `by` to `centreY`

The left paddle test, split over three lines with the `&&` at the end of the first:

```csharp
if (bx > leftX - ballRadius && bx < leftX + paddleWidth + ballRadius &&
    by > leftY - ballRadius && by < leftY + paddleHeight + ballRadius)
    ballSpeedX = -ballSpeedX;
```

Those work out to the same numbers either way — `8` and `48` across the left paddle, `592` and
`632` across the right, `12` and `348` at the walls, `(320, 180)` in the middle — so typing the
numbers instead passes too. The names are there so that you do not have to.

Change nothing else — not the two ball speeds, not `ballRadius`, not the paddles you finished in
step 1. The checker runs three scripts. The first presses no keys for 240 frames and compares
frames 40, 120 and 240; in that time the ball bounces off the bottom at frame 34, reflects off
the right paddle at frame 69, bounces off the top at 102 and the bottom again at 170, and
reflects off the left paddle at 207, so leaving any one of the five lines out puts it somewhere
else. The second holds **Down** for 30 frames, which slides the right paddle out of the way; the
ball leaves the screen on the **right** twice, and only a program that puts it back at
`(centreX, centreY)`, which is `(320, 180)`, has a ball left to draw at frames 90 and 200. The third holds **S** for 30 frames
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

Then replace the miss — the `if (bx < 0 || bx > screenWidth)` block — with two separate `if`s:

- `if (bx < 0)`: set `bx` to `centreX`, set `by` to `centreY`, and set `right` to `right + 1`
- `if (bx > screenWidth)`: set `bx` to `centreX`, set `by` to `centreY`, and set `left` to
  `left + 1`

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

That is Pong: three steps, seventy lines, no idea in it that is younger than the last six
lessons — and a third of those lines is just the numbers of the game, each written down once.
This lesson has no editable sample of its own, because you already have three editors full of
the game — go back up to any of the three challenges and keep typing in it. Nothing you do there
can break anything, and **Reset** always brings the starter back.

:::try
Two changes worth trying, both in the step 3 editor.

Make the rally get harder. Each of the two paddle `if`s controls a single statement with no
`{ }` around it, so put braces round that statement first — otherwise the line you add runs on
every frame instead of only on a hit. Then, next to `ballSpeedX = -ballSpeedX;`, add
`ballSpeedY = ballSpeedY * 1.1;` so every return makes the ball a little steeper. Play until you
lose, then try `1.3` and find out how quickly a good idea becomes an unplayable one.

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
