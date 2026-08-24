# Collections

Every variable so far has held one value. Most real data comes in groups — a week's worth of day
names, a list of scores from a class, a shopping list that grows as you add to it. C# gives you
two ways to hold many values in one variable: **arrays** and **`List<T>`**.

## A fixed group: arrays

An array holds a fixed number of values of the same type, all in one variable, each one reached
by its **index** — its position, counting from `0`:

:::run s11-array Day names, reached by index.

`string[] days = { "Mon", "Tue", "Wed", "Thu", "Fri" };` creates an array of five strings.
`days[0]` is the *first* one, `"Mon"` — counting starts at `0`, not `1` — so `days[2]` is the
*third*, `"Wed"`. `days.Length` tells you how many items there are, `5`, without you counting
them yourself.

:::key
An index counts from **`0`**: the first item is `[0]`, the second is `[1]`, and the last item in
an array of `Length` items is `[Length - 1]`.
:::

## A group that can grow: `List<T>`

An array's size is fixed the moment you create it. A `List<T>` — a *list of* `T`, whatever type
you put in the `< >` — can grow while your program runs, with `.Add(...)` and `.Count` doing the
jobs `[ ]` and `.Length` do for an array:

:::run s11a-list Scores, added to and counted.

`List<int> scores = new List<int> { 10, 20, 30 };` starts the list with three scores already in
it. `scores.Add(40)` puts a fourth on the end — the list itself grew, nothing was recreated — so
`scores.Count` is now `4` and `scores[3]` is that new last score, `40`.

:::key
`List<T>.Add(value)` grows the list by one; `.Count` always tells you its current size. Indexing
a `List<T>` works exactly like an array — `[0]` is still the first item.
:::

## Visiting every item: `foreach`

Reading through a whole collection by counting up an index works, but `foreach` does it more
directly: it hands you each item in turn, in order, without you managing an index yourself:

:::edit s11b-foreach A list of fruits, visited with `foreach`.

`foreach (string fruit in fruits) { ... }` runs its block once per item in `fruits`, with `fruit`
set to that item each time — first `"Apple"`, then `"Banana"`, then `"Cherry"`. There's no index
anywhere in sight; `foreach` handles moving from one item to the next itself.

:::try
Add a fourth line, `fruits.Add("Date");`, right before the `foreach` loop and run again — predict
how many lines print before you do. Then try changing `string fruit in fruits` to `string f in
fruits` — the loop variable's name is yours to choose, it just has to match on both sides of
`in`.
:::

## Challenge

:::challenge c11-high-score
The starter declares a `List<int>` of scores and already prints each one with a `foreach` loop.
Add code that also works out the **highest** score — using a loop and a comparison, the same way
you'd compare any two numbers with `if` — and prints it as `Highest: <n>`. For this list, the
program's complete output must be exactly:

```
42
88
17
65
90
23
Highest: 90
```

Nothing about the existing `foreach` loop needs to change; add your highest-score logic after
it. Press **Check** when you're ready.
:::
