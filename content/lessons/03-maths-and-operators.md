# Maths and operators

C# does arithmetic with the same symbols you already know, mostly. Here are all four on a
pair of `int` variables:

:::run s03-arithmetic Addition, subtraction, multiplication, division on two whole numbers.

`+`, `-`, and `*` do exactly what you'd expect. `/` is the one to watch closely — it's the
whole next section.

:::key
`+ - * /` work on variables the same way they work on numbers written directly in the code.
`a + b` means "add whatever `a` and `b` currently hold."
:::

## Whole numbers divide differently

Divide two `int`s and C# throws away anything after the decimal point — it doesn't round, it
just drops it. Divide when at least one side is a `double` and you get the full answer:

:::run s03a-division `7 / 2` versus `7.0 / 2` — same numbers, different type.

`a` and `b` are both `int`, so `a / b` computes `3` and stops — the `.5` is gone, not rounded
away, just never calculated. `x` is a `double` holding `7.0`, so `x / b` gives the real answer,
`3.5`. Same numbers, different type, different answer — worth remembering the first time your
own program divides two whole numbers and the decimals go missing.

:::key
`/` between two `int`s **throws away the remainder** — `7 / 2` is `3`, not `3.5`. Make one side
a `double` to get a decimal answer.
:::

## The leftover: `%`

`%` — read "modulo" or just "remainder" — gives you what division threw away, instead of the
divided answer. It's the tool for "is this number even?" and for splitting a total into units:

:::run s03b-remainder `%` used two ways: even/odd, and turning seconds into minutes and seconds.

`n % 2` is `0` for an even number and `1` for an odd one — that's the whole trick, worth
remembering. In the second example, `totalSeconds / 60` gives whole minutes and `totalSeconds
% 60` gives the seconds left over once those minutes are removed — `/` and `%` used together
turn one number into two.

## Putting values inside text: `$"..."`

Building strings with `+` gets clunky fast once there are several values to join. C# has a
shortcut: put a `$` before the opening quote, and anything inside `{ }` is evaluated and
dropped straight into the text.

:::edit s03c-interpolation

`{a}`, `{b}`, and `{a + b}` — the last one isn't just a variable, it's a whole expression, and
C# computes it before printing. Try changing the values of `a` and `b`, or changing `{a + b}`
to `{a - b}`, and run it again.

:::try
Rewrite one of your `+`-concatenated lines from earlier in this lesson using `$"..."` instead,
and check the output is identical either way. Both work; `$"..."` usually reads more clearly
once a line has more than one value in it.
:::

:::key
`$"...{expression}..."` evaluates whatever is inside the `{ }` and inserts the result into the
text. It's a shorter, clearer alternative to joining pieces with `+`.
:::

## Challenge

:::challenge c03-sweets
The starter code declares `sweets = 23` and `friends = 4`. Using `/` and `%`, make the program
print **exactly** these two lines:

```
Each friend gets 5 sweets
There are 3 left over
```

`/` gives you the whole sweets each friend gets; `%` gives you what's left once they're shared
out evenly. Press **Check** when both lines match.
:::
