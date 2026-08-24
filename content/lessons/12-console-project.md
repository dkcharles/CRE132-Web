# Console project: The Snack Machine

Every idea from the last eleven lessons — variables, decisions, loops, methods, scope, lists —
comes together in one program: a snack machine. It has a fixed stock of snacks and prices, shows
a menu, takes item numbers, keeps a running total, and knows when someone's picked something it
doesn't sell. Here's the finished thing, given a short scripted list of choices:

:::run s12-demo A shopper buys a Chocolate, mistypes a number, then buys a Crisps.

Open the input panel above: four lines, one item number per `ReadLine()` call, ending in `0` to
finish. You'll build this exact program over the next three challenges, one piece at a time.

## The stock: two lists, one index

The machine's stock is two `List<T>`s of the same length, kept **in step** with each other — the
name at index `i` in one list always belongs to the price at index `i` in the other:

:::run s12-stock The stock: names and prices, read by the same index.

`names[0]` is `"Crisps"` and `prices[0]` is `1` — the same position in both lists describes the
same snack. The loop's `i` runs from `0` up to (not including) `names.Count`, reading both lists
at that one shared index each trip.

:::key
Parallel lists share an index: the same `i` reads the matching name and price out of two
separate lists, one position at a time.
:::

## Printing the menu with a method

A numbered menu line — `"1. Crisps - £1"` — is the same shape every time, just with different
numbers, so it's a natural fit for a method:

:::run s12a-menu The stock, printed as a numbered menu via a formatting method.

`MenuLine(int number, string name, int price)` builds and `return`s one formatted line; the loop
calls it once per snack, passing `i + 1` so the menu numbers from `1`, not `0` — the *display*
number and the *index* are related but not the same thing.

## Challenge: the menu

:::challenge c12a-menu
Using the `names` and `prices` lists already declared in the starter, print the menu: one line
per snack, in the form `N. Name - £Price`, numbered from `1` — the same format `MenuLine` builds
in `s12a-menu` above. For these lists, the program's complete output must be exactly:

```
1. Crisps - £1
2. Chocolate - £2
3. Water - £1
4. Juice - £2
```

Loop over the lists by index, from `0` up to (not including) their `.Count`; the menu number for
index `i` is `i + 1`. Press **Check** when you're ready.
:::

## Taking one order

With the menu on screen, the machine needs to read a choice and react to it: a valid item number
prints what was chosen, anything else says so:

:::edit s12b-order Reads one item number and checks it against the stock.

Open the input panel — `2` is already there, so this picks the second item. `choice - 1` turns
the *menu number* (from `1`) back into the *index* (from `0`) the lists actually use.
`choice >= 1 && choice <= names.Count` is the range check: anything outside it — `0`, a negative
number, or past the end of the list — falls to the `else`.

:::key
Check a number is in range **before** using it as an index — `choice - 1` is safe only once
`choice >= 1 && choice <= names.Count` has already confirmed it points at a real item.
:::

:::try
Change the input panel's `2` to `9` — a number with no matching snack — and run again to see the
`else` branch. Then change it to `0` and predict which branch runs before you press Run.
:::

## Challenge: taking an order

:::challenge c12b-order
The starter already prints the menu; extend it to read one item number and react to it. For a
valid item — between `1` and the number of snacks — print exactly:

```
You chose <Name> - £<Price>
```

using the real snack name and price. For anything else, print exactly:

```
Sorry, we don't have that
```

For input `2`, the program's complete output (menu included) must end with:

```
You chose Chocolate - £2
```

For input `9`, it must end with:

```
Sorry, we don't have that
```

Press **Check** when you're ready; it tries three different item numbers.
:::

## Keeping a running total

A real till doesn't stop after one item — it keeps reading choices, adding up prices as it goes,
until the shopper's done. That's an accumulator, wrapped around the order-taking you just wrote,
reading numbers until a `0` says "finished". Look back at `s12-demo`'s code at the top of this
lesson for the shape: one `Console.ReadLine()` before the loop starts, a `while (choice != 0)`
around everything, and another `Console.ReadLine()` as the very last thing inside the loop's
`{ }` — that second read is what lines up the *next* number for the loop to check. Copy the
*read-loop* shape, not the `Console.WriteLine("Enter an item number...")` prompt line above
it — like every challenge, the till prints only the menu, the chosen items, and the total.

:::key
An accumulator declared before a loop, and updated inside it, keeps a running result across
every trip through the loop — exactly how the machine's total survives from one order to the
next.
:::

## Challenge: the till

:::challenge c12c-till
Turn the single order into a till. Instead of reading one item number, keep reading them in a
loop until the shopper enters `0`. Add a total, starting at `0` and declared **before** the loop;
every time a valid item is chosen, add its price to it. Keep printing `You chose ...` or
`Sorry, we don't have that` for each number exactly as before. Once `0` is entered, stop reading
and print exactly:

```
That comes to £<total>
```

For the input `2`, `1`, `0` (Chocolate, then Crisps, then finish), the program's complete output
must end with:

```
You chose Chocolate - £2
You chose Crisps - £1
That comes to £3
```

For the input `9`, `3`, `0` (an invalid number, then Water, then finish), it must end with:

```
Sorry, we don't have that
You chose Water - £1
That comes to £1
```

Press **Check** when you're ready; it tries both orders — the machine you just finished works the
same way as `s12-demo` did at the top of this lesson.
:::
