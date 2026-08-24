# Making decisions

Every program you've written so far runs every line, every time. Real programs need to skip
some lines and run others instead, depending on what's true at the moment — that's what `if`
does.

:::run s05-if Checks a condition, then keeps going either way.

`temperature > 25` is a **condition** — a question with exactly one of two answers, `true` or
`false`. `if (temperature > 25) { ... }` runs the code between `{ }` only when the answer is
`true`; when it's `false`, that block is skipped entirely and the program carries straight on to
whatever comes after it. `temperature` is `30`, so `30 > 25` is `true`, and the `"It's hot
today."` line runs. The last line always runs — it isn't inside the `if`.

:::key
`if (condition) { ... }` runs its block only when `condition` is `true`. When it's `false`, the
block is skipped completely and the program moves on to whatever comes next.
:::

## Choosing between two paths: `else`

An `if` on its own can only skip something. Add `else`, and you get a second block that runs
whenever the `if`'s condition was `false` — one of the two always runs, never both, never
neither:

:::run s05a-else Pass or fail, depending on a mark.

`mark >= 50` is `false` — `42` is not `50` or more — so the `if` block is skipped and the `else`
block runs instead, printing `"Fail"`. Change `mark` to `65` in your head and you can see the
other branch would run instead.

C# compares values with `>`, `<`, `>=`, `<=` for "greater/less than (or equal)", `==` for "is
equal to", and `!=` for "is not equal to". Watch that last pair: `=` on its own *assigns* a
value (`mark = 42`), while `==` *compares* two values (`mark == 50`) — mixing them up is one of
the easiest slips to make, and the compiler will usually catch it for you.

:::key
`if (...) { ... } else { ... }` runs **exactly one** of the two blocks — the `if` block when the
condition is `true`, the `else` block when it's `false`. There's no case where both run, or
neither does.
:::

## Conditions can be a `bool` on its own

A condition doesn't have to compare two values — a `bool` variable already *is* a true/false
answer, so it can go inside `if ( )` by itself:

:::edit s05b-bool A `bool` variable used directly as a condition.

:::try
Change `isMember` to `false` and run it again — the branch that runs flips. Then try writing
`if (isMember == true)` instead of `if (isMember)`: it behaves identically, but `if (isMember)`
is the way experienced C# is actually written, because the condition already *is* the true/false
answer.
:::

## Challenge

:::challenge c05-bouncer
Read someone's age. If they're **18 or over**, print exactly:

```
Come in
```

Otherwise, print exactly:

```
Sorry, not tonight
```

The starter reads the age but always prints `Come in` — add the `if`/`else` that makes it
depend on the age. Press **Check** when you're ready; it tries more than one age.
:::
