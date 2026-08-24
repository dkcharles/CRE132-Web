# Methods

Every program you've written so far is a list of statements in one file, top to bottom. As a
program grows, the same few lines start showing up more than once — printing a greeting for
three different people, say. A **method** lets you write those lines once, give them a name,
and run them again by name instead of copying them out each time.

## Defining and calling a method

C# lets you add methods to the same file as your statements — declared *after* the statements,
at the bottom. The statements at the top still run first, in order, exactly as before; a method
only runs when something calls it by name:

:::run s09-first-method Greets three people by calling the same method three times.

`Greet("Ada")`, `Greet("Grace")`, and `Greet("Alan")` are the three statements that actually
run, top to bottom, same as always. `void Greet(string name) { ... }`, declared below them, is
the method itself — it doesn't run on its own just by being there; each call above jumps into
it, runs its body with `name` set to whatever was passed in, then returns to the next line.
`void` means this method doesn't hand anything back to its caller — it just does something and
finishes.

:::key
A method is a named block of code you can run by writing its name and `( )`, as many times as
you like, instead of writing the same statements out each time.
:::

## Getting a value back: `return`

A method can also hand a value back to whatever called it, using `return`. Write the type of
that value — `int`, `string`, whatever it is — in place of `void`, and the call itself becomes
an expression you can use like any other value:

:::run s09a-return Adds two numbers using a method, twice.

`int Add(int a, int b)` takes two `int` parameters and `return`s their sum. `Add(3, 4)` doesn't
just run — it *evaluates to* `7`, so `int total = Add(3, 4)` stores that `7` in `total`, and
`Console.WriteLine(Add(10, 5))` prints `15` without a variable at all. The moment `return` runs,
the method stops and hands that value straight back to wherever it was called from.

:::key
Parameters are the values a method needs, listed in its parentheses; a `return` value is the
single result it hands back to its caller, usable anywhere a value of that type could go.
:::

## Your turn

These six lines print the same two-line message for three people — with nothing pulled into a
method yet:

:::edit s09b-refactor Welcomes three people, one repeated pair of lines at a time.

:::try
Write a method `void Welcome(string name)` below the statements, with a body that prints
`"Welcome, "` plus `name` plus `"!"` on one line and `"Enjoy your stay."` on the next. Then
replace all six lines above with three calls: `Welcome("Ada");`, `Welcome("Grace");`,
`Welcome("Alan");`. Run it and check the output still matches what you started with — the same
result from a third of the code.
:::

## One more tool: string methods

Strings have methods of their own, too. `"hello".ToUpper()` returns a new string with every
letter capitalised — `"HELLO"` — leaving the original unchanged. You'll use it in this lesson's
challenge.

## Challenge

:::challenge c09-shout
Finish the method `string Shout(string message)` so it returns `message` in capitals with an
`!` on the end — `.ToUpper()` gives you the capitals, `+ "!"` appends the mark. For input
`hello`, the program must print exactly:

```
HELLO!
```

For input `Cre132 rocks`, it must print exactly:

```
CRE132 ROCKS!
```

The starter already reads a line and prints whatever `Shout` returns — right now that's always
empty, because the method just returns `""`. Fill in the method's body; nothing else needs to
change. Press **Check** when you're ready; it tries both lines.
:::
