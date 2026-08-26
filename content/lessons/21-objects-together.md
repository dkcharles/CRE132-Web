# Objects together

One ball is an object. A game is a list of them.

The collections lesson gave you `List<float>` — a list of numbers. A list can hold anything with
a type, and `Ball` is a type now, so `List<Ball>` is a list of balls:

```csharp
List<Ball> balls = new List<Ball>();
balls.Add(new Ball(80, 60, 3, 2));
```

`.Add` takes one whole ball — its position, its speeds, its radius and its two methods, all in
one item. Once they are in there, moving every ball on the screen is three lines:

:::run s21-list-of-balls Five balls, one plan, one loop.

`foreach (Ball ball in balls)` hands you each ball in turn, and `ball.Move()` moves that one.
This is a `foreach`, not the index loop the many-things lesson insisted on — and that is not an
oversight. The rule there was that `foreach` gives you a *copy* of each value, so it is no use
when you need to write a new value back into the list. Nothing here writes back into the list:
`ball` is the ball, and `ball.Move()` changes the ball's own fields. The list is untouched.

The loop that fills the list lives in `Setup`, which runs once. Written in `Draw` it would add
five more balls thirty times a second.

:::key
`List<Ball>` holds objects the same way `List<float>` holds numbers, and `foreach` over it is
enough when each item updates itself. Fill the list in `Setup` — `Draw` runs thirty times a
second.
:::

## Compared with two lists

The many-things lesson did this with `xs` and `ys`: two parallel lists, one index reaching into
both, and a standing agreement that the drop at index `3` is `xs[3]` paired with `ys[3]`. It
works, and it is exactly one number per drop away from falling over. Give each drop a speed and
that is a third list to keep in step; a size, and a fourth. Every `Add` has to hit all of them,
every `RemoveAt` has to hit all of them, and the day one of them misses, drop `3` is wearing
drop `4`'s speed.

A list of objects has no pairing to keep, because the pairing is inside the object. One `Add`,
one `RemoveAt`, and a drop's speed cannot come adrift from its position because they are the
same box.

## Spawning and removing

Making and destroying objects while the game runs is the same two moves as before —
`Frame.Count % n == 0` for the *when*, `RemoveAt(i)` for the taking away:

:::run s21a-spawn-remove A drop every fifteen frames, gone once it is past the bottom.

Here the loop **is** an index loop, and for the reason the many-things lesson gave: this one
changes the list while it walks it. `drops[i]` is the drop at that index — `drops[i].Move()`
moves it, `drops[i].y` reads its field. Removing during a `foreach` is not merely a bad idea; C#
stops the program with an error the moment you try, because the loop has lost its place.

`RemoveAt(i)` and `i--` are unchanged from the many-things lesson, and there is now only one of
each to write. One list, one removal.

:::key
Change a list while looping over it and you need `for (int i = 0; i < list.Count; i++)`, plus
`i--` after every `RemoveAt(i)`. A `foreach` that removes throws.
:::

## Each one owns its update

Every star below falls at its own speed and is drawn at its own size, and `Draw` says nothing
about any of it — it calls `star.Move()` and `star.Draw()` and the star does the rest:

:::edit s21b-each-owns-update Six stars, six speeds, six sizes, one loop.

`speed` and `size` are fields like any other, filled in by the constructor, so the loop in
`Setup` can make each star different by passing different numbers. That is the pattern the rest
of Part 3 leans on: the game loop asks every object to update itself, and what "update itself"
means is the object's business.

:::try
Give each star a colour of its own. Add `public Colour tint;` to the field list, add a
`Colour startTint` parameter to the end of the constructor with `tint = startTint;` in its body,
and draw with `Screen.Circle(x, y, size, tint);`. Then in `Setup`, pass a colour that depends on
the loop counter:

```csharp
Colour colour = Colour.Cyan;
if (i % 2 == 1) colour = Colour.Pink;
stars.Add(new Star(60 + i * 100, i * 60, 2 + i, 12 + i * 4, colour));
```

`Colour` is a type, so a field can hold one — the same as `float` or `Ball`. Then try giving
`Star` a `Reset()` method that puts it back at `y = 0`, and calling it from `Move()` instead of
the wrapping line.
:::

## Challenge

:::challenge c21-fireworks
A firework is a spark thrown upwards that gravity pulls back down. The starter has `class Spark`
with three fields — `x`, `y` and `speedY` — a constructor that fills all three in, an empty
`Move()`, an empty `Draw()`, and a `List<Spark> sparks` that nothing ever adds to. Five comments
mark the five things to write.

Where comment 1 is, in the top-level `Draw`: when `Frame.Count % 10 == 0`, add exactly **one**
spark with

```csharp
sparks.Add(new Spark(Rand.Range(100, 540), 360, -8));
```

Call `Rand.Range` once and only once per launch, with those two numbers, or your random `x`
positions will not line up with the checker's. `360` is the bottom of the screen and `-8` is
upwards, because up is negative y.

Where comment 2 is, inside the loop: if that spark's `sparks[i].y` is greater than `380` — a
little past the bottom, so it is out of sight — remove it with `sparks.RemoveAt(i);` and then
step the index back with `i--;`.

Where comment 3 is, after the loop:

```csharp
Screen.Text(10, 10, "Sparks: " + sparks.Count, Colour.White);
```

That exact text, at that exact position.

Where comment 4 is, inside `Move()`, two lines in this order: `speedY = speedY + 0.3f;` then
`y = y + speedY;`. Gravity is added to the speed first, so a spark leaving at `-8` slows,
stops, and comes back down.

Where comment 5 is, inside `Draw()`, one line: `Screen.Circle(x, y, 12, Colour.Orange);`.

The checker runs 150 frames and looks at frames 45 and 150. By frame 45 five sparks are in the
air on five different arcs, which only comes out right if the gravity line is there and runs
before the movement line. By frame 150 fifteen have been launched and ten have fallen back past
`380`, so the screen should read `Sparks: 5` — leave the removal out and it reads `Sparks: 15`.
Press **Check** when you are ready.
:::
