# Reading input

Every program so far has decided everything in advance — you wrote the numbers and the
messages straight into the code. A program that reads input can react to whatever the person
running it types, which is most of what real programs do.

`Console.ReadLine()` is the instruction that reads one line of text typed by whoever is running
the program. In a real terminal, the program would pause the moment it reaches that line and
wait — however long it takes you to type something and press Enter.

This site can't pause and wait for your keyboard, so it works differently: above the console
you'll see a box labelled **Input — one line per `Console.ReadLine()`**. Click it open, type
what you want the program to "hear" — one line per `ReadLine()` call it will make — and *then*
press Run. The panel plays the keyboard's part: it hands its lines to the program's
`ReadLine()` calls in order, one at a time, exactly as if you'd typed them and pressed Enter
each time.

:::run s04-greeting Reads a name, then greets it.

Open the input panel above and you'll see `Ada` already sitting there. Press **Run this** — the
program's `Console.ReadLine()` receives `"Ada"` as if it had been typed, and stores it in the
variable `name`.

:::key
Type input into the panel **before** pressing Run — the panel plays the keyboard's part. A real
terminal would pause and wait for you instead; this page reads what's already there.
:::

## `ReadLine` always gives you text

Whatever the person types — a name, a number, anything — `Console.ReadLine()` always hands it
back as a `string`. Even if they type `7`, you get the *text* `"7"`, not the number `7`. Read
two lines and add what they'd mean as numbers, and you can see the difference:

:::run s04a-numbers Reads two numbers as text, then converts them before adding.

`Console.ReadLine()` gives `firstText` and `secondText` as plain text. `int.Parse(firstText)`
converts that text into a real `int` — only then can `+` add them as numbers instead of
joining them as text. Try opening the input panel and changing `7` and `5` to two numbers of
your own before running again.

:::key
`Console.ReadLine()` always returns a **`string`**, never a number. `int.Parse(...)` converts
text that looks like a whole number into an actual `int` you can do arithmetic with.
:::

:::try
`s04a-numbers` expects two lines of input, one per `ReadLine()` call. Open its input panel,
delete the second line so only one number is left, and press Run — read the error you get. That
error is what happens when a program asks for input that never arrives; it's the same kind of
computer-telling-you-what-it-needs help you met with a missing semicolon.
:::

## Your turn

:::edit s04b-echo Reads whatever you type back to you.

:::try
Change the code so it prints the text **twice**, on two separate lines. Then open the input
panel, replace `CRE132` with a message of your own, and press Run to see both changes together.
:::

## Challenge

:::challenge c04-age-next-year
The starter code reads a name, then reads an age as text but never converts or uses it. Finish
it so the program reads a name and an age, then prints the name, the word `will be`, the age
plus one, and `next year` — all on one line. For the input `Sam` then `20`, the output must be
**exactly**:

```
Sam will be 21 next year
```

For the input `Priya` then `17`, it must print exactly `Priya will be 18 next year` — same
pattern, different name and age. You'll need `int.Parse` to turn the age into a number before
you can add `1` to it. Your program should read **silently**: print only the one answer
line, and don't print a question first — the checker compares every line you print, starting
with the first. Press **Check** when you're ready — it runs your program against both.
:::
