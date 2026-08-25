# The mouse

The keyboard lesson gave your program a way to be *told* things. The mouse gives it something
different: a **place**. Wherever the pointer is, your program can know, and can draw there.

`Mouse.X` and `Mouse.Y` are the pointer's position, in exactly the same pixels everything else
on the canvas uses — `Mouse.X` counts across from the left edge, `Mouse.Y` counts down from the
top. They are two ordinary numbers, and they change on their own between one frame and the next:

:::run s16-follow A circle drawn wherever the mouse is, with its position printed on the canvas.

Move the pointer across the canvas. The circle is not "following" anything — every frame `Draw`
asks where the mouse is *right now* and draws a circle there. Take the mouse off the canvas and
the numbers simply stop changing, because there is nothing new to report.

That is the whole of it. `Mouse.X` and `Mouse.Y` go anywhere a number goes: as the centre of a
circle, as the corner of a rectangle, as one end of a line, or into a variable of your own.

:::key
`Mouse.X` and `Mouse.Y` are just two numbers that change by themselves. Read them inside `Draw`
and use them anywhere a coordinate is wanted.
:::

## Is the mouse inside that box?

A button is a rectangle that knows when the pointer is over it. A rectangle drawn at `bx`, `by`
with width `bw` and height `bh` covers everything from `bx` to `bx + bw` across, and from `by`
to `by + bh` down. So the pointer is inside it when **all four** of those things are true at
once — which is exactly what `&&` is for:

```csharp
bool inside = Mouse.X > bx && Mouse.X < bx + bw && Mouse.Y > by && Mouse.Y < by + bh;
```

Read it left to right: far enough right, not too far right, far enough down, not too far down.
Miss one comparison out and the "button" reaches all the way to an edge of the screen.

:::run s16a-hover A rectangle that lights up, and says so, while the pointer is over it.

The result of those four comparisons is stored in a `bool` called `inside`, and then used twice
— once to choose the colour, once to decide whether to draw the word. Giving the test a name
like that is worth doing even when you only use it once: `if (inside)` says what the program
means, where four comparisons repeated in a row only say what it does.

:::key
Point-in-rectangle is four comparisons joined with `&&`: past the left edge, before the right
edge, past the top edge, before the bottom edge. Store the answer in a `bool` and give it a name.
:::

## Held, or clicked?

The mouse button works exactly like a key. `Mouse.IsDown` is `true` on **every** frame the
button is held — thirty times a second, which is what you want for dragging or painting.
`Mouse.WasClicked` is `true` on **one** frame only, the frame the button went down, which is
what you want for anything that should happen once per press:

:::edit s16b-click Each click moves the target to the pointer.

Because the click happens on one frame only, `tx` and `ty` keep their new values for as long as
nothing else changes them — which is why the target sits still between clicks instead of dashing
after the pointer.

:::try
Change `Mouse.WasClicked` to `Mouse.IsDown` and run again: hold the button down and the target
never leaves the pointer, because now it is being moved thirty times a second. Change it back,
then make the target fussier — it should only move when you click **inside** it. The target's
centre is `tx`, `ty` and its radius is `30`, so swap the condition for this:

`if (Mouse.WasClicked && Mouse.X > tx - 30 && Mouse.X < tx + 30 && Mouse.Y > ty - 30 && Mouse.Y < ty + 30)`

That is the same four comparisons as the button test, measured out from a centre instead of in
from a corner. Now clicks in empty space are ignored, and only a click on the target picks it up.
:::

:::key
`Mouse.IsDown` is *held*; `Mouse.WasClicked` is *the moment*. Same pair of ideas as `Keys.IsDown`
and `Keys.WasPressed`, on a different button.
:::

## Challenge

:::challenge c16-target
The starter draws a yellow circle of radius `20` at `tx = 320`, `ty = 180`, and nothing moves it.
Where the comment is, add an `if` that runs when `Mouse.WasClicked` is true and, inside it, sets
`tx` to `Mouse.X` and `ty` to `Mouse.Y`.

Leave the radius `20` and the two starting values alone, and do not touch the `Screen.Circle`
line. The checker runs three short scripts: one clicks at `(100, 60)` and expects the circle
centred there afterwards; one clicks at `(560, 296)` and expects it there instead; and one moves
the pointer to `(500, 300)` without ever pressing the button, and expects the circle still at
`(320, 180)`. Press **Check** when you are ready.
:::
