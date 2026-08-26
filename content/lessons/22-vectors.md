# Vectors

Every moving thing so far has moved along an axis you chose in advance: `x = x + speed` goes
across, `y = y + speed` goes down, and a diagonal is both at once. This lesson answers the
question those cannot: **how do I move towards something, at a speed I pick?**

The answer is a pair of numbers used as an arrow. `(3, 4)` as a position means "three across and
four down from the corner". The same `(3, 4)` used as an arrow means "three across and four down
**from wherever you are**" — a direction, and a distance, in two numbers. That is a **vector**,
and the class lesson gave you exactly the box to keep one in:

:::run s22-vec2 A vector's length, and the same vector with its length taken away.

`Length()` is the collision lesson's distance formula with a shorter name:
`MathF.Sqrt(x * x + y * y)`, the long side of the triangle whose short sides are `x` and `y`. The arrow `(3, 4)` is `5` long.

`Normalised()` divides both parts by that length. The arrow still points the same way, and it is
now exactly `1` long — `(0.6, 0.8)`, whose own `Length()` comes back `1`. A vector of length `1`
is nothing but a direction, and that is the point of it: once the size is gone you can put back
whichever size you want.

:::key
A vector is an `x` and a `y` used as an arrow: a direction and a distance together. `Length()` is
how long it is. `Normalised()` is the same arrow shrunk to length `1` — direction with the size
thrown away.
:::

## Two more methods

`Add` puts two arrows end to end, and `Scale` stretches one:

```csharp
public Vec2 Add(Vec2 other)
{
    return new Vec2(x + other.x, y + other.y);
}

public Vec2 Scale(float amount)
{
    return new Vec2(x * amount, y * amount);
}
```

Both **return a new `Vec2`** rather than changing the one they were called on, which is why
`position.Add(step)` on its own does nothing: the answer has to be caught, as in
`position = position.Add(step);`.

With those four methods, chasing is one line of English. The arrow from where you are to where
you want to be is the target minus your position:

```csharp
Vec2 toTarget = new Vec2(target.x - position.x, target.y - position.y);
```

Normalise it to throw the distance away, scale it by the speed you want, and add it to where you
are:

```csharp
position = position.Add(toTarget.Normalised().Scale(speed));
```

That is a constant-speed chase, in any direction, with no `if` deciding which way is which:

:::run s22a-chase The yellow dot moves four pixels a frame towards the pointer. Move the pointer around the canvas.

The `if (toTarget.Length() > speed)` guard is doing two jobs. It stops the dot jittering on the
spot once it has arrived, and it keeps the program away from the one arrow that cannot be
normalised: an arrow of length `0` has no direction, and dividing by that length divides by
zero. Check the length before you normalise, every time.

:::key
Direction is **target minus position**. Constant-speed movement is
`position = position.Add(direction.Normalised().Scale(speed));` — normalise to get the direction,
scale to set the speed. Never normalise an arrow of length `0`.
:::

Unity's `Vector2` is this class, ready-made: the same `x` and `y`, the same `magnitude` and
`normalized`, with `+` and `*` written as symbols instead of methods. Everything you do with
`Vec2` here you will do there with the same words in the same order.

:::edit s22b-flee The pink dot runs away instead, and only when you get within 200 pixels of it.

Reversing a chase is one subtraction the other way round: `position - pointer` points from the
pointer towards the dot, which is the direction "away". `distance` is worked out once and used
twice, which reads better than calling `Length()` in both halves of the test.

Run it and herd the dot into a corner. It walks straight off the edge of the screen, because
nothing in the program says the screen has edges.

:::try
Give it walls. After the `if` that moves it, add four lines. `radius` is the `16` already
declared at the top of the file, so the dot stops with its rim against each wall:

```csharp
if (position.x < radius) position.x = radius;
if (position.x > Screen.Width - radius) position.x = Screen.Width - radius;
if (position.y < radius) position.y = radius;
if (position.y > Screen.Height - radius) position.y = Screen.Height - radius;
```

Now corner it and it stays cornered. Then add a hunter: `Vec2 hunter = new Vec2(60, 60);` at the
top of the file, and in `Draw`, before the two `Screen.Circle` lines:

```csharp
Vec2 toDot = new Vec2(position.x - hunter.x, position.y - hunter.y);
if (toDot.Length() > 3) hunter = hunter.Add(toDot.Normalised().Scale(3));
Screen.Circle(hunter.x, hunter.y, 14, Colour.Red);
```

The red dot chases the pink one at `3` pixels a frame while you chase it at `4`.
:::

## Challenge

:::challenge c22-seek
The starter has the finished `Vec2` class — `Length`, `Normalised`, `Add` and `Scale`, all
written for you — a yellow `seeker` at `(100, 300)` drawn with radius `14`, a red `target` at
`(540, 60)` drawn with radius `12`, and `float speed = 4;`. The seeker never moves.

Where the comment is, before the two `Screen.Circle` lines, do two things.

First, build the arrow from the seeker to the target: a `new Vec2` whose two numbers are
`target.x - seeker.x` and `target.y - seeker.y`. Call it `toTarget`.

Then step along it, but only while there is still a step's worth of ground to cover:

```csharp
if (toTarget.Length() > speed)
```

Inside that `if`, one line: set `seeker` to `seeker.Add(...)` of `toTarget` **normalised** and
then **scaled** by `speed` — `Normalised()` throws the distance away and `Scale(speed)` puts back
the `4` pixels you want, exactly as `s22a-chase` does above. The `if` is not decoration: without
it the seeker overshoots the target and jitters across it forever instead of stopping.

Change nothing else — not the two positions, not the two radii, not `speed`. The checker runs
two scripts. The first is 60 frames long and looks at frames 30 and 60, by which time the
seeker has covered `120` and `240` pixels of a straight line from `(100, 300)` to `(540, 60)` —
about a quarter of the way and about half: a seeker moving at the wrong speed, or not
normalising at all, is nowhere near either mark. The second script runs 200 frames and looks at
the last one. The gap is about `500` pixels wide, so `4` pixels a frame arrives at around frame
125 — by frame 200 a seeker that stops is resting on the target, and one that does not is still
bouncing past it. Press **Check** when you are ready.
:::
