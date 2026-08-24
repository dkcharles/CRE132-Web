# Your first program

Every program you have ever used — every game, every app, every website — is a list of
instructions that a computer follows, one after another. In C#, one instruction is called a
**statement**. Here is a complete program made of exactly one statement:

:::run s01-hello A complete C# program: one statement.

Press **Run this**. The program runs *in your browser* — nothing is installed, nothing is
sent anywhere.

`Console.WriteLine(...)` is an instruction that means *print one line of text*. The text to
print goes between the quotes. The semicolon `;` marks the end of the statement — every
statement ends with one, the way a sentence ends with a full stop.

:::key
A program is a list of **statements** that run **in order, top to bottom**. Each statement
ends with a semicolon `;`.
:::

## More than one statement

Programs get interesting when statements work together. Each `Console.WriteLine` prints its
own line:

:::run s01-quotes Three statements, three lines, in order.

## Notes to yourself: comments

A line starting with `//` is a **comment** — the computer skips it entirely. Programmers use
comments to leave notes for the humans who read the code later (usually themselves):

:::run s01-comments Comments never print.

## Your turn

Now edit some code yourself. Change the message between the quotes — keep the quotes! — and
press Run:

:::edit s01-edit-message

:::try
Try adding a second `Console.WriteLine` line of your own below the first one. Then try
deleting the semicolon and pressing Run — read the error message you get. Errors are the
computer telling you exactly what it needs; you will meet many, and they are help, not
punishment. Press **Reset** any time to get the original back.
:::

:::key
Text you want printed goes **between double quotes**. What's between quotes is yours; the
punctuation around it — quotes, parentheses, semicolon — belongs to C# and has to be exact.
:::

## Challenge

:::challenge c01-three-lines
Make the program print **exactly** these three lines:

```
CRE132
Learning C#
Week 1
```

The first line is already done. Add statements for the other two, then press **Check** —
your output has to match exactly, including capital letters.
:::
