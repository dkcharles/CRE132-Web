# Your first class

Here is the bouncing ball from the motion lesson, as far as the top of the file is concerned:

```csharp
float x = 320;
float y = 180;
float speedX = 4;
float speedY = 3;
float radius = 20;
```

Five loose variables, and somewhere further down, the four or five lines that move them and the
one line that draws them. It works. Now add a second ball: `x2`, `y2`, `speedX2`, `speedY2`,
`radius2`, and a second copy of every line. A third and you are typing `x3` and reaching for the
scroll bar.

The five numbers belong together — they are all *one ball* — and so does the code that uses
them. C# has a box for exactly that, and it is called a **class**.

## The shape of a class program

A class is a *plan* for a box: what is in it, and what it can do. Here is the whole file, and the
layout it uses is the one every program from here on will use:

:::run s20-ball-class A ball that moves and draws itself.

Four bands, in this order: the objects the program needs, then `Setup` and `Draw`, then
`Game.Run(Setup, Draw);`, then the classes. The classes go **at the bottom, underneath
`Game.Run`** — not because it looks tidy, but because C# insists on it. A `class` written above
the top-level lines is a compiler error, so there is only one order that builds, and that is it.

Inside `class Ball` are two kinds of thing.

**Fields** are the variables a ball is made of. `public float x, y, speedX, speedY;` declares
four of them on one line, separated by commas — it is the same as four separate `float` lines,
just shorter. `public float radius = 20;` declares a fifth *and* gives it a starting value, so
every new ball begins with a radius of `20` without anyone setting it.

**Methods** are what a ball can do. `Move()` and `Draw()` are ordinary methods, exactly as the
methods lesson taught them, with one difference: inside the class they can say `x` and `speedX`
with no dot in front, because a ball's methods can see the ball's own fields.

`new Ball()` builds one — one actual ball, with its own five numbers — and `Ball ball = ...`
holds on to it. The word before the name is the type, the same as `float x` or `List<float> xs`:
`Ball ball` is "a Ball called ball".

`ball.x = 320;` reaches into that ball from outside and sets one of its fields, and `ball.Move();`
calls one of its methods. Both are only allowed because every field and every method is marked
**`public`**, which means "code outside this class may use this". Every field and method you
write in this course is `public`; leave the word off and the line calling it stops compiling.

:::key
A class is a plan for a box: **fields** are the variables one of them is made of, **methods** are
what it can do. `new Ball()` builds one. Everything in the class is `public` so the rest of the
program can use it, and the class itself lives at the **bottom of the file**, under `Game.Run`.
:::

One method name is worth pausing on. `Ball.Draw()` is *not* special. The engine knows exactly two
methods — the top-level `Setup` and `Draw` you hand to `Game.Run` — and nothing else. The ball
appears on screen because the top-level `Draw` calls `ball.Draw();` by hand. Take that line out
and the ball is still moving, still remembering where it is, and completely invisible.

One more rule, and it costs nothing to keep: do not name a class `Game`, `Screen`, `Keys`,
`Mouse`, `Frame`, `Rand`, `Colour` or `Key`. Those are the names the drawing API already uses,
and a class of your own with the same name hides it — after which `Screen.Circle` stops
compiling for a reason no error message will make obvious.

## Filling a new one in

Setting four fields by hand after every `new` is dull, and a ball you forget to set sits at
`(0, 0)` with a speed of nothing. A **constructor** does the filling in for you:

```csharp
public Ball(float startX, float startY, float startSpeedX, float startSpeedY)
```

It is a method with two peculiarities: its name is the class's name, and it has no return type,
not even `void`. It runs once, when `new Ball(...)` is written, and the arguments in the brackets
arrive as its parameters. So `new Ball(160, 100, 4, 3)` builds a ball already at `(160, 100)`,
already travelling `4` across and `3` down — one line instead of five.

One catch: once `Ball` has that constructor, the empty `new Ball()` from the previous sample no
longer compiles. A class with a constructor asks for exactly the arguments it names, so from here
on you have to pass all four values.

:::run s20a-constructor Two balls, built from one plan, each with its own starting numbers.

Two `Ball` objects, one `class Ball`. The plan is written once; `new` was written twice, and each
of those two balls carries its own `x`, its own `y`, its own pair of speeds. `first.Move()` moves
one of them and leaves the other exactly where it was.

:::key
A **constructor** is a method named after its class with no return type. It runs when `new` is
written and fills the new object's fields in from the arguments. One class, as many objects as
you like — each with its own copy of every field.
:::

:::edit s20b-two-balls The same two balls, with the second one given a bigger radius.

`second.radius = 30;` proves the point: `first.radius` is still `20`, because the two balls do
not share a `radius`, they each have one.

:::try
Add a third ball. One line at the top — `Ball third = new Ball(320, 60, 5, -4);` — and two lines
in `Draw`: `third.Move();` and `third.Draw();`. Then give `Ball` a way to start over. Add this
method inside the class, under `Draw`:

```csharp
public void Reset()
{
    x = 320;
    y = 180;
}
```

and call `third.Reset();` inside an `if (Frame.Count % 90 == 0)` in `Draw`, so the third ball
jumps back to the middle every three seconds. It keeps its speeds — `Reset` only touches what it
is told to touch.
:::

## Challenge

:::challenge c20-ball-methods
The starter's `Ball` has the five fields, a constructor, and a `Move()` that only adds the speeds
on — so `new Ball(320, 180, 4, 3)` heads off the bottom-right corner and never comes back. Its
`radius` is `20`. The top-level `Draw` is still drawing the circle itself, reaching into the ball
for `ball.x`, `ball.y` and `ball.radius`.

Where comment 1 is, inside the class, add two methods.

`public void Bounce()`, with the two lines the motion lesson used:

- reverse `speedX` when `x` is less than `radius` **or** greater than `Screen.Width - radius`
- reverse `speedY` when `y` is less than `radius` **or** greater than `Screen.Height - radius`

`public void Draw()`, with one line in it:

```csharp
Screen.Circle(x, y, radius, Colour.Yellow);
```

Neither method takes any parameters, and neither says `ball.` anywhere — inside the class the
fields are just `x`, `y`, `speedX`, `speedY` and `radius`.

Then, where comment 2 is, delete the `Screen.Circle(...)` line, so the body of the top-level
`Draw` reads exactly:

```csharp
Screen.Clear(Colour.Black);
ball.Move();
ball.Bounce();
ball.Draw();
```

Do not change the starting position, the two speeds, or the radius. The checker runs 200 frames
and looks at frames 60 and 200. The ball first reaches the bottom edge on frame 54 and the
right-hand edge on frame 76, so frame 60 catches a missing `speedY` bounce and frame 200 catches
a missing `speedX` one: leave either line out and the ball is somewhere else by then, or off the
screen altogether. Press **Check** when you are ready.
:::
