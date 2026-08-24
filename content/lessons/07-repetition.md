# Repetition

Every program so far runs each line once, then stops. Lots of real problems need the same
steps run again and again — printing a countdown, checking every row of a grid, asking someone
to try again until they get it right. C# has two instructions built for exactly that: `while`
and `for`.

## Repeat while a condition holds: `while`

`while (condition) { ... }` runs its block over and over, checking `condition` before every
single trip through it. The moment the condition is `false`, the loop stops and the program
moves on:

:::run s07-while Counts down from 5, then lifts off.

`count` starts at `5`. Each trip through the loop prints it, then `count = count - 1` makes it
one smaller. C# checks `count > 0` again before every trip — once `count` reaches `0`, the
condition is `false`, the loop stops, and `"Liftoff!"` prints once, after the loop, not as part
of it.

:::key
`while (condition) { ... }` repeats its block for as long as `condition` is `true`, checking it
before every trip through the loop. The moment it's `false`, the loop ends and the program
carries on.
:::

## Packing start, check, and step together: `for`

A `while` loop that counts needs three separate things: somewhere to start, a condition to
check, and a step to take each time. `for` puts all three on one line:

:::run s07a-for Prints 1 through 10.

`for (int i = 1; i <= 10; i++)` means: start with `i` at `1`; keep going while `i <= 10`; add
one to `i` after every trip. `i++` is shorthand for `i = i + 1` — a step common enough that C#
gives it its own shorter spelling. Everything a counting `while` loop needs three separate
lines for, a `for` loop says in one.

:::key
`for (start; condition; step) { ... }` runs `start` once, then repeats: check `condition`, run
the block, run `step` — the same three jobs a counting `while` loop does, packed into one line.
:::

## Your turn

This loop counts from `start` up to `end`, `step` at a time:

:::edit s07b-steps Counts from `start` to `end`, `step` at a time.

:::try
Change `start`, `end`, or `step` and run again — a bigger `step` skips more numbers, a smaller
one counts more of them. Then try something riskier: add a loop that can never finish on its
own, as its own line inside the sample:

```
while (true) { }
```

Press Run and watch what happens instead of the page freezing. Nothing inside `true` ever
becomes `false`, so on its own this loop would run forever — but the site is counting every
trip around every loop you run, and when one takes far more trips than any real program needs,
it stops the run and shows you a friendly message rather than locking up the page. That safety
net runs quietly behind everything on this site, which means `while (true)` is always safe to
try, just to see what happens.
:::

## Challenge

:::challenge c07-times-table
Read a whole number `n`, then print its times table from `1 x n` through `5 x n`, one line
each, in the form `1 x n = ...`. For input `4`, the output must be **exactly**:

```
1 x 4 = 4
2 x 4 = 8
3 x 4 = 12
4 x 4 = 16
5 x 4 = 20
```

For input `7`, it must print exactly:

```
1 x 7 = 7
2 x 7 = 14
3 x 7 = 21
4 x 7 = 28
5 x 7 = 35
```

The starter reads `n` and prints only the first line — replace it with a `for` loop that
prints all five. Press **Check** when you're ready; it tries both numbers.
:::
