# More decisions

`if`/`else` gives you two paths. Most real decisions have more than two — a grade isn't just
pass or fail, it's a band; a ticket price depends on more than one thing at once. This lesson
adds the tools for that: chained conditions, combining conditions, and `switch`.

## Chaining conditions: `else if`

Add `else if (...)` between `if` and `else` to test another condition only when the first was
`false`. C# checks each one top to bottom and runs the **first** block whose condition is
`true` — every condition after that is never even checked:

:::run s06-grades An else-if chain sorting a mark into a grade band.

`mark` is `72`. C# checks `mark >= 70` first — `true` — so `"Grade: A"` prints, and none of the
`else if`s or the final `else` even run, even though `mark >= 60` would also have been `true`.
Order matters: if the bands had been written smallest-first, `72` would wrongly match the first
one it satisfies.

:::key
An `else if` chain runs the **first** block whose condition is `true`, top to bottom, and skips
every condition after it — even ones that would also have been `true`.
:::

## Combining conditions: `&&`, `||`, `!`

Sometimes a decision depends on more than one thing at once. `&&` ("and") needs **both** sides
to be `true`; `||` ("or") needs **at least one**; `!` ("not") flips a `true` into a `false` or
back:

:::run s06a-and-or Ticket pricing combining an age check with a `bool`.

`age < 18 && isStudent` needs both to be `true` — `age` is `20`, so `age < 18` is already
`false`, and the whole `&&` is `false` no matter what `isStudent` is. C# moves to the `else if`:
`age >= 65 || isStudent` needs only one side true, and `isStudent` is `true`, so the whole
condition is `true` and `"Ticket: £7"` prints. Writing `!isStudent` anywhere would mean "is
**not** a student" — the same variable, flipped.

:::key
`&&` needs **both** sides `true` to be `true` itself; `||` needs **at least one**. `!` flips a
`bool` the other way — `!isStudent` is `true` exactly when `isStudent` is `false`.
:::

## Matching one value against several: `switch`

An `else if` chain of equality checks against the *same* variable gets repetitive. `switch`
says it once: give it a value, then list `case` labels for each possibility, each ending in
`break;` so only that one case runs, plus a `default` for anything not listed:

:::edit s06b-switch A menu choice read from input, matched with `switch`.

:::try
Open the input panel and change `2` to `1` or `3` and run again to see a different `case`
match. Then change it to `5` — a number with no matching `case` — and watch `default` catch it
instead. In the code itself, try adding a fourth `case 4:` of your own for another menu item.
:::

:::key
`switch (value) { case x: ...; break; }` matches `value` against each `case` in turn and runs
that block; `default` runs when nothing else matched. Don't forget `break;` — it's what stops
one case falling into the next.
:::

## Challenge

:::challenge c06-cinema
Read an age, then a day. Print **exactly** one of these three lines:

- Under 12: `Child ticket: £5`
- 12 or over, and the day is `Tuesday`: `Tuesday special: £6`
- Anything else: `Standard ticket: £9`

Age wins first — even on a Tuesday, an age under 12 always prints `Child ticket: £5`. The
starter reads both values but always prints the standard price; add the `if`/`else if`/`else`
that checks age first, then day. Press **Check** when you're ready; it tries three combinations.
:::
