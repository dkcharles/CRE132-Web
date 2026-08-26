# Variables and types

A program that only ever prints fixed text gets boring fast. What makes programs useful is
that they can remember a value, and change it later. Here is the smallest example:

:::run s02-first-variable A variable declared, printed, changed, printed again.

`int score = 0;` does two things at once. It creates a named place to store a number, called
`score`, and it puts `0` into it straight away. `Console.WriteLine("Score: " + score)` prints
the *label* `"Score: "` joined to the *value* `score` is currently holding, using `+`.

Then `score = 10;` changes what is stored in `score` — no `int` this time, because the box
already exists, you are just putting a new value in it. The second `Console.WriteLine` prints
the same line of code, but a different result, because `score` now holds something else.

:::key
A variable is a **named box**. `int score = 0;` creates the box and puts a value in; `score =
10;` replaces what's inside without creating a new box.
:::

## A type for every kind of value

`int` is one **type** — it means *the box holds a whole number*. C# has a type for every kind
of value you'll store. The four you'll use constantly:

- `int` — whole numbers, like `20` or `-5`.
- `float` — numbers with a decimal point, like `4.5f`.
- `string` — text, always in double quotes, like `"CRE132"`.
- `bool` — one of exactly two values: `true` or `false`.

:::run s02a-types One variable of each type, printed with a label.

Notice the type comes right before the name every time a variable is created: `float price =
4.5f;`, `bool passed = true;`. The type tells C# — and tells *you*, reading it later — what kind
of value belongs in that box. Put the wrong kind of value in and C# will refuse to compile the
program, which is C# helping you, not punishing you.

The little `f` on the end of `4.5f` is part of the number: the `f` tells C# this is a float —
the number type games use. A whole number needs no `f`, so `float speed = 3;` is written just
like that. C# has a second decimal type called `double`, which you will meet in other people's
code, but this course writes `float` everywhere, because `float` is the type Unity uses.

:::key
The type says **what fits in the box**. A `string` box holds text; an `int` box holds a whole
number; putting the wrong kind of value in either one is a compiler error, not a surprise
later.
:::

## Changing a variable

Once a variable exists, you can put a new value in it any time, as long as the new value is
still the right type. Try it yourself:

:::edit s02b-change

:::try
Change `"Hello"` and `"Goodbye"` to two messages of your own. Then try adding a third
`Console.WriteLine(greeting);` after a third reassignment, and predict what it will print
before you press Run.
:::

## Naming your variables

`score`, `price`, `greeting` — a variable's name is yours to choose, and a good name says what
the value *is*. C# variable names start with a lowercase letter, can't contain spaces, and by
convention use `camelCase` for names with more than one word — `totalScore`, not
`total_score` or `TotalScore`. Naming things well is one of the few parts of programming that
stays hard forever; starting the habit now pays off later.

## Challenge

:::challenge c02-about-you
The starter code below declares `name` and `age` and prints:

```
Sam is 20
```

Add one more variable, `string course = "CRE132";`, and change the `Console.WriteLine` so the
program prints **exactly**:

```
Sam is 20 and studies CRE132
```

Use all three variables — don't just type the sentence as fixed text. Press **Check** when
you're ready.
:::
