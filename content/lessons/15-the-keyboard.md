# The keyboard

A picture that moves on its own is a screensaver. A picture that moves when *you* do something
is a game, and the difference is one method call.

`Keys.IsDown(Key.Right)` asks a single question — *is the right arrow held down right now?* —
and answers `true` or `false`. It is a `bool`, exactly like the ones in the decisions lesson, so
it drops straight into an `if`:

:::run s15-move Four arrows, one square. Click the canvas, then hold an arrow key.

Click on the canvas first so it is listening to the keyboard, then hold an arrow key.

Every frame, `Draw` asks all four questions and moves `x` or `y` by `speed` — `5` here — for each
one that comes back `true`. The same `5` in all four `if`s would be four numbers to keep in step,
so it is written once at the top of the file and used four times; `size`, the square's width and
height, is there for the same reason. Nothing remembers that you pressed anything: `Keys.IsDown`
reports what is happening *this frame only*, so you have to ask again on the next one. That is
why the four `if`s live inside `Draw` and not in `Setup`.

Hold two arrows at once and both `if`s are true, so the square moves diagonally — for free, with
no extra code.

The key names are `Key.Left`, `Key.Right`, `Key.Up`, `Key.Down`, `Key.Space`, `Key.Enter`,
`Key.Escape`, the letters `Key.A` to `Key.Z`, and the digits `Key.D0` to `Key.D9` (a name in C#
cannot start with a digit, so the number keys get a `D` in front of them).

:::key
`Keys.IsDown(Key.Right)` is a `bool` that is `true` while the key is held down. Ask it **every
frame**, inside `Draw`.
:::

## Staying on the screen

Hold `Right` long enough on that sample and the square walks off the edge and keeps going: `x`
becomes `700`, then `2000`, and nothing stops it. Two `if`s do:

```csharp
if (x < 0) x = 0;
if (x > Screen.Width - size) x = Screen.Width - size;
```

The first catches `x` sliding under the left edge and pins it at `0`. The second catches the
right-hand edge: `size` is the square's width, `40`, and the square is drawn from its left
corner, so the furthest right it can sit and still be fully on screen is `640 - 40`, which is
`600`. Squashing a value back into a range like this is called **clamping**, and it is always
the same shape — one `if` for the low end, one for the high end.

:::run s15a-clamp The same square, clamped on all four sides, with its position drawn on screen.

Hold `Right` until the square stops, then hold `Down` until it stops. Up and down are clamped
the same way, against `Screen.Height - size` instead of `Screen.Width - size` — the screen is
`360` tall, so the square's `y` never gets past `320`. The `Screen.Text` line draws the live
values of `x` and `y` onto the canvas: putting a number on screen is the quickest way to see
what your program thinks is going on.

:::key
Clamping is two `if`s: one for the low end, one for the high end. For a shape `w` pixels wide
the high end is `Screen.Width - w`, because a shape is drawn from its **left** edge.
:::

## Held, or pressed?

`IsDown` is true on **every** frame the key is down — thirty times a second while you hold it.
That is exactly right for movement, and exactly wrong for everything else: a key that fires a
shot would fire thirty shots before you let go.

`Keys.WasPressed(Key.Space)` is true on **one** frame only, the frame the key went down. Hold
the key all day and it stays `false` until you release it and press again. It is the right
question for anything that should happen once per press — a jump, a shot, pausing the game:

:::edit s15b-pressed Counting presses of the space bar.

:::try
Click the canvas and tap the space bar a few times: the count goes up by exactly one per tap,
however long you hold each one. Now change `Keys.WasPressed(Key.Space)` to
`Keys.IsDown(Key.Space)` and run again. Hold the space bar down and watch the count race away —
thirty a second, one for every frame the key is down. Change it back, and keep the difference:
`IsDown` is *held*, `WasPressed` is *the moment*.
:::

## Challenge

:::challenge c15-paddle
The starter draws a paddle `paddleWidth` (`100`) wide and `paddleHeight` (`16`) tall, at
`x = 270` and `paddleY` (`330`) — a Pong paddle that cannot move. Give it the arrow keys, and
keep it on the screen. Two more names are declared at the top of the file ready for you:
`paddleSpeed` is `6`, and `paddleMaxX` is `screenWidth - paddleWidth`, which comes to `540`.

Where the comment is, add:

- when `Key.Left` is down, `x = x - paddleSpeed`
- when `Key.Right` is down, `x = x + paddleSpeed`
- then clamp `x`: never below `0`, never above `paddleMaxX` — that is `640 - 100`, the screen's
  width less the paddle's, or `540`

Leave the `Screen.Rect` call exactly as it is. The checker holds `Right` down and looks twice: at
frame 40 the paddle should have reached `x = 510`, and at frame 60 it should be pinned against
the right-hand edge at `x = 540` rather than past it. Then it holds `Left` for 80 frames and
expects the paddle pinned at `x = 0`. Press **Check** when you are ready.
:::
