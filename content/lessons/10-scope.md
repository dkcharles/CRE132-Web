# Scope

Every variable you declare lives inside a pair of braces — the `{ }` around an `if` block, a
loop, a method. That pair of braces is the variable's whole world: it exists from the moment its
declaration runs to the closing `}`, and nowhere outside it.

## A variable's world: the braces around it

Here's a variable declared inside an `if` block. Before you run it, predict: does `"Done
checking."` print no matter what `score` is, or only when the `if` runs?

:::run s10-braces A variable declared inside an `if` block, used inside it.

`message` is declared with `string message = ...;` **inside** the `if`'s `{ }`. It's used on the
very next line, still inside those same braces — that's the whole reason it works. The final
`Console.WriteLine("Done checking.")` sits outside the `if` entirely, on its own line after the
closing `}`, so it runs whether or not the `if` did; its printing has nothing to do with
`message`, which by that point doesn't exist anymore.

:::key
A variable declared inside `{ }` only exists between that opening brace and its matching closing
brace. Once the program passes the `}`, the variable is gone — as if it had never been declared.
:::

## Two methods, two separate worlds

Because a method's body is its own pair of braces, two different methods can each declare a
variable with the *same name* and never conflict — each one only exists inside its own method:

:::run s10a-method-scope Two methods, each with its own local called `count`.

`CountBooks` declares its own `count` and prints it; `CountApples` declares a completely
different `count`, in a completely different world, and prints that one. Neither method can see
the other's `count` — they just happen to share a name, the way two strangers can both be called
Sam without being confused for each other.

:::key
Two variables in two different methods can share a name without conflict — each method's `{ }`
is its own world, and a name declared in one is invisible in the other.
:::

## When the world ends too soon

A loop's `{ }` is a world too — including the counter variable itself. Predict what would happen
if the commented-out line below were switched on, *then* uncomment it and press Run to check your
prediction:

:::edit s10b-predict A loop counter, then a commented-out line that reaches for it after the loop ends.

The loop prints `i` fine, three times, from inside its own `{ }`. The commented line tries to
print `i` again *after* the loop's closing `}` — outside the world `i` was declared in. Uncomment
it and run: you'll get a compiler error, `i` "does not exist in the current context", because by
that point it doesn't. Put the `//` back afterwards so the sample compiles again.

:::try
Uncomment the last line in `s10b-predict`, predict the exact error you'll get, then press Run and
compare. Then look at `s10a-method-scope` again as a thought experiment: if you added a third
method with its own `count` set to a different number and called it from the top, would it
conflict with the other two? Test the same idea for real in `s10b-predict` instead — declare a
second variable of your own inside the loop's `{ }`, give it a value, and predict whether you
could print it after the loop's closing `}` before you try.
:::

## Declaring where the value needs to survive

A variable that needs to outlast one trip through a loop — a running total, say — has to be
declared **outside** the loop's braces, before it starts. Declare it inside instead, and you get
a brand new, empty variable every single trip, with no memory of the last one.

:::key
Decide where a value needs to still exist, then declare it there. A total that must survive
every trip through a loop is declared **before** the loop, not inside it.
:::

## Challenge

:::challenge c10-fix-the-scope
The starter below prints a total — but only from the loop's very last trip, because `total` is
declared **inside** the loop, so it's reset to `0` and rebuilt from scratch every time around.
Watching just that last trip, it computes `0 + 5`, giving:

```
Total: 5
```

Move **one line** — the declaration `int total = 0;` — from inside the loop to just before it
starts, so the same `total` survives every trip and keeps growing instead of resetting. With
that one change, the program must print exactly:

```
Total: 15
```

Nothing else needs to change; only where that one line sits. Press **Check** when you're ready.
:::
