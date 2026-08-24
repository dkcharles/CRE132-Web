# Loop patterns

`while` and `for` repeat a block — on their own that's simple. What makes loops useful is what
you combine them with: a variable that carries a result forward, or a second loop nested
inside the first. This lesson covers both.

## Carrying a result across the loop: accumulators

A variable declared *before* a loop and updated *inside* it can carry a running result from one
trip to the next. Add every number from `1` to `100` and you'd never write a hundred `+`s by
hand — a loop and a variable to hold the running total do it in a few lines:

:::run s08-sum Adds every whole number from 1 to 100.

`sum` starts at `0`, *before* the loop begins. Each trip through the loop adds that trip's `i`
to whatever `sum` already holds — it doesn't reset, so the total from every earlier trip is
still there, growing by one more number each time. A variable used this way is called an
**accumulator**: it accumulates a result across every turn of the loop.

:::key
An accumulator is a variable declared before a loop and updated inside it, carrying a running
result forward from one trip through the loop to the next.
:::

## A loop inside a loop

Some patterns need a loop for each row *and* a loop for each column — a loop nested inside
another loop. The outer loop runs once per row; each time it does, the whole inner loop runs
from start to finish before the outer loop takes its next step:

:::run s08a-nested A 4-by-6 rectangle of `*`.

The outer loop runs four times, once per `row`. Each time, `line` starts empty, then the inner
loop runs six times, appending one more `"*"` to `line` on every trip — that's the same
accumulator idea from before, just building a string instead of a number. Once the inner loop
finishes, `Console.WriteLine(line)` prints the whole row, and the outer loop moves to the next
one.

:::key
Nested loops are a loop per row and a loop per column: the outer loop's body contains a
complete inner loop, which runs fully on every single trip the outer loop takes.
:::

## Your turn

Change how many characters the inner loop builds, and the shape changes with it. This loop
makes each row longer than the last, one `#` at a time — a right triangle:

:::edit s08b-triangle A right triangle of `#`, `height` rows tall.

:::try
Change `height` and run again — the triangle grows or shrinks with it. Then look at the inner
loop's condition, `col <= row`: it's what makes row `1` print one `#` and row `5` print five.
Try changing `"#"` to a different character, or changing the inner condition so every row
prints the *same* number of characters instead — you'll get a rectangle like the sample above.
:::

## Challenge

:::challenge c08-staircase
Read a whole number `height`, then print a staircase of `#`: `height` rows, where row `1` has
one `#`, row `2` has two, and so on up to row `height`. For input `3`, the output must be
**exactly**:

```
#
##
###
```

For input `5`, it must print exactly:

```
#
##
###
####
#####
```

The starter reads `height` and prints only a single `#` — replace it with nested loops, one
for the rows and one for the `#`s in each row. Press **Check** when you're ready; it tries
both heights.
:::
