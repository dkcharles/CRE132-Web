# First graphics

For twelve lessons, every program you wrote *printed*. From here on they *draw*. It is the same
C# — the same variables, `if`s, loops, methods and lists — but instead of lines of text
scrolling down a console you get a rectangle of pixels that redraws itself thirty times a
second.

Here is the smallest complete drawing program:

:::run s13-first-screen A blue screen with a yellow circle in the middle of it.

Press **Run this**. A canvas appears, and it keeps running until you leave the page.

## The shape of a drawing program

Every program in this half of the course has the same three parts, and all three are in the
sample above:

- `void Setup()` runs **once**, before anything is drawn. It is where you say how big the
  screen is.
- `void Draw()` runs **thirty times a second**, for as long as the program is running. Whatever
  you draw in here is what you see.
- `Game.Run(Setup, Draw);` is the last line of the file. It hands your two methods over and
  starts the clock.

`Setup` and `Draw` are ordinary methods — the same ones you wrote in the methods lesson — and
`Game.Run` is an ordinary method call that happens to take two methods as its arguments. You do
not have to understand *how* it takes them yet. You used `Console.WriteLine` in your very first
lesson long before you knew what a method was; `Game.Run(Setup, Draw);` is the same bargain.
Write it as the last line of every drawing program, and read on.

:::key
`Setup` runs **once**. `Draw` runs **30 times a second**. `Game.Run(Setup, Draw);` at the bottom
of the file is what starts them both.
:::

## Where things are

The screen is **640 pixels wide and 360 pixels tall**. Every position is two numbers: `x` across
and `y` down, measured from the **top-left corner**, which is `(0, 0)`.

`x` grows to the right, exactly as you would expect. `y` grows **downward** — `y = 0` is the top
edge, `y = 360` is the bottom. That is the opposite of the graphs you drew in maths, and it is
the one thing everybody gets wrong on their first afternoon: **a bigger `y` is further down the
screen.**

So `(320, 180)` is the middle, `(0, 0)` is the top-left, and `(640, 360)` is the bottom-right.

:::run s13a-shapes Three shapes and a line of text make a scene.

Read that `Draw` against the picture:

- `Screen.Rect(0, 300, 640, 60, Colour.Green)` — a filled rectangle whose **top-left corner** is
  at `(0, 300)`, `640` wide and `60` tall. Starting at `y = 300` out of `360` puts it along the
  bottom: the ground.
- `Screen.Circle(560, 70, 30, Colour.Yellow)` — a circle **centred** at `(560, 70)` with a
  radius of `30`. Big `x`, small `y`, so: top right. The sun.
- `Screen.Line(0, 300, 640, 300, Colour.White)` — a line from `(0, 300)` to `(640, 300)`. Two
  points, four numbers, drawn along the top of the ground as a horizon.
- `Screen.Text(20, 20, "My first scene", Colour.White)` — text, with its top-left at `(20, 20)`.

Colours are named: `Colour.Black`, `Colour.White`, `Colour.Grey`, `Colour.Red`, `Colour.Orange`,
`Colour.Yellow`, `Colour.Green`, `Colour.Cyan`, `Colour.Blue`, `Colour.Purple`, `Colour.Pink`.
Every drawing call takes one as its last argument.

## Clear comes first

`Draw` runs again thirty times a second, and the canvas keeps whatever was already on it. Leave
it alone and frame 2 paints on top of frame 1, frame 3 on top of that, and anything that moves
smears across the screen instead of moving.

`Screen.Clear(Colour.Black)` repaints the whole canvas in one colour. Make it the **first** line
of `Draw` and every frame starts from a clean sheet. You will see exactly what happens without
it in the next lesson.

:::key
`(0, 0)` is the **top-left** and `y` grows **downward**. Nothing is wiped for you — start `Draw`
with `Screen.Clear(...)`.
:::

## Your turn

This one keeps the circle's position in two variables, so you can move it by changing a number
instead of by rewriting the call:

:::edit s13b-coordinates The circle's position lives in `x` and `y`.

:::try
Change `x` to `500` and run — predict which way the circle moves before you press the button.
Put `x` back to `100` and change `y` to `300`: down the screen, not up it. Now set `y` to `400`
and run again. The circle vanishes — `400` is past the bottom edge at `360`, so it is being
drawn somewhere off the screen where you cannot see it. Nothing has broken; it is simply not
where you can look at it. The text at the top still reports the numbers, which is how you tell
"gone off the edge" from "never drawn at all".
:::

## Challenge

:::challenge c13-house
The starter already clears the screen and draws the green ground. Add a house standing on it,
using these **exact** numbers, in this order, after the ground:

- the walls: `Screen.Rect(220, 180, 200, 120, ...)`
- a round window: `Screen.Circle(320, 240, 16, ...)`
- the left roof slope: `Screen.Line(220, 180, 320, 100, ...)`
- the right roof slope: `Screen.Line(320, 100, 420, 180, ...)`

Choose any colour you like for each of the four shapes: the checker looks at **where** shapes
are and how big they are, never at what colour they are. Press **Check** when you are ready.
:::
