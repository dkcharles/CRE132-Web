# Many things

Every moving thing so far has had its own pair of variables — `x` and `y`, `speedX` and
`speedY`. That works for one ball. It does not work for a hundred raindrops, and you are not
going to type `x1` to `x100`.

The collections lesson already gave you the answer: a `List<float>` holds as many numbers as
you like and grows while the program runs. Use **two** lists, one for the across positions and
one for the down positions, and agree that the drop at index `3` is `xs[3]` paired with `ys[3]`:

:::run s17-rain Ten drops, two lists, one loop.

`xs` and `ys` are declared at the top of the file, outside both methods, exactly like a single
`x` would be — they have to survive from frame to frame. Each has ten numbers in it, so there
are ten drops, and `xs[i]` with `ys[i]` is where drop number `i` is.

The loop is a `for` loop over the index, not a `foreach`:

```csharp
for (int i = 0; i < xs.Count; i++)
```

`foreach` hands you a copy of each value, which is fine for reading but useless for moving:
`ys[i] = ys[i] + 4;` needs the **index** so it can write the new value back into the list. The
same `i` reaches into both lists at once, which is the whole trick — one counter, two lists,
kept in step. `if (ys[i] > Screen.Height) ys[i] = 0;` is what makes it rain forever instead of
once: a drop that falls past the bottom is sent straight back to the top rather than removed.
`Screen.Height` rather than a typed-out `360`, for the reason the motion lesson gave — the
screen's size is written down once, in `Setup`, and asking for it beats copying it out again.

:::key
Two parallel lists and one index: `xs[i]` and `ys[i]` are the same thing's two coordinates. Loop
with `for (int i = 0; i < xs.Count; i++)` whenever the loop **changes** the list.
:::

## Making new ones

A list that starts full is a fixed cast. A list that starts empty and grows is a game. `.Add(...)`
appends one more item, and `Frame.Count` gives you a schedule to add on:

```csharp
if (Frame.Count % 10 == 0)
```

`Frame.Count % 10` is the remainder from the maths lesson: it is `0` on frames `0`, `10`, `20`,
`30`, and so on — three times a second at thirty frames a second. A bigger number means rarer.

For the *where*, `Rand.Range(20, 620)` picks a whole number from `20` up to but not including
`620` — a new one every time it is called. It is the same idea as rolling a die, and the two
numbers are the low end and the just-past-the-high-end:

:::run s17a-spawn Starts with nothing. Every tenth frame, one more drop appears at a random x.

Both lists have to grow together, or the pairing breaks: `xs.Add(...)` and `ys.Add(0)` on the
same frame keeps drop `i` a real drop. `xs.Count` on the canvas shows the list filling up — and
filling up, and filling up, because nothing ever takes a drop out again.

:::key
`Frame.Count % n == 0` is a metronome: true once every `n` frames. `Rand.Range(low, high)` picks
a number from `low` up to just below `high`. Add to **both** lists together or the pairing breaks.
:::

## Taking them out again

A drop that has fallen past the bottom of the screen is still in the list, still being moved and
drawn every frame, forever. `xs.RemoveAt(i)` deletes the item at index `i` and closes the gap —
and closing the gap is exactly where beginners lose a drop, because everything after the hole
slides down by one:

```
before RemoveAt(1)      after RemoveAt(1)

index   0  1  2  3      index   0  1  2
value   A  B  C  D      value   A  C  D
           ^                       ^
           remove this             C sits at index 1 now
```

The loop was at `i = 1` when it removed `B`. Then `i++` makes `i = 2`, and index `2` now holds
`D` — so `C` never gets its turn this frame. The fix is one line: step the index back after a
removal, so the next `i++` lands on it again.

```csharp
xs.RemoveAt(i);
ys.RemoveAt(i);
i--;
```

`i--` is `i++` in reverse: shorthand for `i = i - 1`.

:::edit s17b-remove Drops appear at the top, fall, and are removed at the bottom.

The count on the canvas settles instead of climbing: drops are being added and taken away at the
same rate, so the program stays the same size however long it runs.

:::try
Change the `10` in `Frame.Count % 10 == 0` to `4` and run — a downpour. Change the `6` in
`ys[i] = ys[i] + 6;` to `2` and watch the count settle at a much bigger number, because slower
drops take longer to leave. Then delete the `i--` line and run again: with a heavy enough
downpour you can see drops skip a frame as their neighbours are removed out from under them.
:::

:::key
Removing from a list you are looping over shifts everything after the hole down one place. Undo
the shift with `i--` right after `RemoveAt`, or the item that slid into the gap gets skipped.
:::

## Challenge

:::challenge c17-falling-stars
The starter has the two lists (both empty), and a loop that already draws each star as a white
circle of radius `12` at `xs[i]`, `ys[i]` and moves it down `4` pixels a frame. Nothing ever
appears, because nothing is ever added. Fill in the three comments:

1. Where comment 1 is: when `Frame.Count % 15 == 0`, add exactly **one** star — `xs.Add(...)`
   with `Rand.Range(20, 620)`, and `ys.Add(0)`. Call `Rand.Range` once and only once per spawn,
   with those two numbers, or the checker's random positions will not line up with yours.
2. Where comment 2 is, inside the loop: if that star's `ys[i]` is greater than `370` (a little
   past the bottom edge, so the star is completely off the screen), remove it from **both** lists
   with `RemoveAt(i)` and then step `i` back with `i--`.
3. Where comment 3 is, after the loop: `Screen.Text(10, 10, "Stars: " + xs.Count, Colour.White);`
   — that exact text, at that exact position.

Do not change the radius `12`, the speed `4`, or the order of the lines already in the loop. The
checker runs 120 frames and looks at frames 45 and 120: by frame 120 the oldest stars have
fallen off the bottom, so the count on screen only comes out right if you are removing them.
Press **Check** when you are ready.
:::
