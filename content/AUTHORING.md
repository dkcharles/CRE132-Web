# Authoring a CRE132 lesson

This is the working guide for writing content in `content/`. Read it before writing a
lesson, and use it as a checklist while you work. The pilot lesson,
`content/lessons/01-first-program.md`, is the exemplar for voice and structure — when in
doubt about tone, read it again.

## The directive grammar

A lesson `.md` file is ordinary Markdown plus six block directives. Every directive line
starts with `:::` in column one. There are two shapes:

**Single-line directives** — the whole directive is one line; nothing follows it but more
markdown. `:::run` and `:::edit` are single-line:

```
:::run s01-hello A complete C# program: one statement.
```

**Body directives** — the directive line opens a block that is closed by a bare `:::` on its
own line. Everything between the two is the body. `:::try`, `:::key`, and `:::challenge`
are body directives:

```
:::key
A program is a list of **statements** that run **in order, top to bottom**. Each statement
ends with a semicolon `;`.
:::
```

A stray `:::` — an extra bare `:::` left in prose, a body directive never closed, a typo'd
directive name — fails the build. It does not fail quietly: `LessonDoc` reports the file and
line, and the site is not published until it's fixed.

## The file kit for a lesson

Every lesson needs, at minimum:

- [ ] `content/lessons/<NN-name>.md` — the narration itself.
- [ ] 3–5 sample kits in `content/samples/`, each `<id>.cs` + `<id>.out.txt` (the golden
      output), plus `<id>.in.txt` only if the sample reads input.
- [ ] At least one challenge kit in `content/challenges/`: `<id>.start.cs` +
      `<id>.solution.cs` + `<id>.cases.json`.
- [ ] One row in `WebCatalog.Entries` (`web/CRE132.Web/WebCatalog.cs`), inserted in course
      order, pointing at `lessons/<NN-name>.json`.

Nothing in this kit is optional scaffolding — a `:::run` naming a sample with no `.cs` file,
or a `:::challenge` naming an id missing any of its three files, is a build error, not a
runtime one.

## Id conventions

- Lesson files: `<NN-name>.md`, two-digit zero-padded number, kebab-case name —
  `01-first-program.md`, `04-input-and-parsing.md`.
- Sample ids: `s<NN>-<slug>` — `s01-hello`, `s01-edit-message`. The `s` prefix, the lesson
  number, and a short slug naming what the sample shows.
- Challenge ids: `c<NN>-<slug>` — `c01-three-lines`.
- Catalog `Id` (the first argument to `Entry`, and the `#hash` a lesson is addressed by): the
  lesson number as a *string*, no padding — `"1"`, `"4"`, `"12"`. This is deliberately not the
  same string as the `NN` in the filename (`"04-..."`): the catalog id is what appears in the
  URL hash and the contents list ("4. Input and parsing"), the filename padding is only so
  files sort in course order on disk.

## Challenges

A challenge kit is three files sharing an id: `<id>.start.cs`, `<id>.solution.cs`,
`<id>.cases.json`. Three constraints govern them, none of them optional:

- The starter (`<id>.start.cs`) must **compile and run as shipped**. It may print the wrong
  thing, or an incomplete thing, or nothing at all beyond what's already there — that's the
  point, the student finishes it — but it must never fail to compile. A student's first
  encounter with a challenge can't be a red error list before they've touched anything.
- The solution (`<id>.solution.cs`) is the **minimal idiomatic answer using only concepts
  taught by that lesson**, and no others. No reaching ahead for a shortcut a student in that
  lesson couldn't yet write themselves — a Lesson 5 challenge is solved with `if`/`else` and
  what came before, never with a loop or a method just because it's shorter.
- The challenge's task statement (the body of its `:::challenge` block, in the lesson md)
  must **state the exact expected output text**. A student should never have to guess
  capitalisation, punctuation, spacing, or wording — if the output is `CRE132`, the task
  statement says `CRE132`, not "the course code."

## The `cases.json` format

Each challenge's `<id>.cases.json` is a JSON array of `{ "input": "...", "expected": "..." }`
objects, 1–3 entries. `input` is fed to the program's `Console.ReadLine()` calls exactly as
written (each line the program reads ends in `\n`); `expected` is compared against the
program's output using the tolerance rule below — it does not need a trailing newline.

Worked example, `c01-three-lines.cases.json` (a challenge that takes no input):

```json
[
  { "input": "", "expected": "CRE132\nLearning C#\nWeek 1" }
]
```

A challenge whose program reads input would instead have entries like
`{ "input": "16\n", "expected": "You entered 16" }`. Every case's `solution.cs` must satisfy
every one of its own cases — `ContentTests` runs the reference solution against its own
`cases.json` at build time and fails the build if it doesn't match, so a challenge's cases
are checked before a student ever sees them.

## The output-comparison tolerance rule

Golden files (`.out.txt`) and challenge `expected` strings are compared with
`OutputComparer.FirstDifference`, not raw string equality:

- Line endings are normalised (`\r\n` and `\r` both become `\n`).
- Each line has trailing whitespace trimmed.
- Leading and trailing *blank* lines are ignored.
- Every remaining line must match exactly, in order, including case, spacing within a line,
  and punctuation.

In practice: don't hand-write a golden file. Run the sample, copy what it printed. For a
challenge, run the reference solution and copy that. The build re-checks both anyway (a
sample against its golden, a challenge's own solution against its own cases) and fails if
they've drifted — but getting the golden right the first time by generating it saves you the
round trip.

## "Nothing before it's taught"

A lesson may use only C# it — or an earlier lesson — has already introduced. This is the
single hardest rule to break silently (a sample that happens to compile with a
not-yet-taught feature won't be caught by any test), so hold to the topic table below when
choosing what a sample or challenge may contain:

| # | Lesson | Introduces |
|---|--------|------------|
| 0 | Welcome | How the site works |
| 1 | Your first program | Statements, `Console.WriteLine`, comments |
| 2 | Variables & types | Variables, types |
| 3 | Maths, operators, interpolation | Arithmetic operators, `$"..."` interpolation |
| 4 | Input and parsing | `Console.ReadLine`, the input panel, `int.Parse` |
| 5 | Making decisions | `if`/`else`, comparisons, `bool` |
| 6 | More decisions | `else if`, `&&`/`\|\|`/`!`, `switch` |
| 7 | Repeating yourself | `while`, `for` |
| 8 | Loops in depth | Accumulators, nested loops, ASCII art |
| 9 | Methods | Methods, parameters, return values |
| 10 | Scope | Variable scope |
| 11 | Collections | Arrays, `List<T>`, `foreach` |
| 12 | Putting it together | Console project (Snack Machine) |

Concretely: no `if` before Lesson 5, no loops before Lesson 7, no methods before Lesson 9, no
arrays or `List<T>` before Lesson 11. String building uses `+` concatenation until Lesson 3
introduces `$"..."` interpolation — don't reach for interpolation in Lesson 2's samples even
though it would read more naturally, because the reader hasn't met it yet.

## Style

- Second person ("you"), short paragraphs.
- A concrete example before the term that names it — show the code, then name the concept,
  not the other way round.
- Errors are framed as help, not failure: a compiler error is "the computer telling you
  exactly what it needs", something to read and learn from, never a punishment.
- 2–3 `:::key` callouts per lesson, capturing the one or two ideas worth re-reading.
- At least one `:::try` per lesson, inviting the reader to experiment beyond what the samples
  show.
- A lesson reads in 5–10 minutes — don't pad it to look thorough.
- Samples fit on one screen; if a sample needs scrolling to read, it's doing too much.

The pilot lesson, `content/lessons/01-first-program.md`, embodies all of the above and is the
reference to match when a rule in this document leaves a judgement call.

## Gotchas

- **A stray `:::`.** A leftover closing `:::` in prose, or a `:::key`/`:::try`/`:::challenge`
  body that itself contains a line starting with `:::`, breaks parsing. No body line may
  start with `:::` — if you need to talk *about* the directive syntax, you can't fence it as
  a directive inside a directive (see below).
- **No `#n` cross-lesson links.** Prose may not link to another lesson by its catalog id
  (`#4`, `#12`, ...) — the id a lesson is addressed by can change as lessons are inserted or
  reordered, and a hard-coded link would silently point at the wrong lesson. Refer to another
  lesson by name in prose instead ("covered in the input lesson"), never by a `#n` hash.
- **You cannot fence a demo of the directive syntax.** There is no way to show `:::run` or
  `:::key` as literal text inside a lesson — any line starting with `:::` is parsed as a
  directive, including inside a fenced code block. If you need to explain the directive
  grammar itself (as this document does), write it somewhere that isn't a lesson `.md`.
- **`.in.txt` is real, from Task 2 onward.** Earlier in the project, a sample with an
  `.in.txt` file failed the build outright (the site had no way to feed it stdin). That gate
  is gone: an `.in.txt` beside a sample's `.cs` now becomes the prefilled contents of an input
  panel shown above the sample's console, and its contents are what the program's
  `Console.ReadLine()` calls receive unless the student edits the panel first. A sample with
  no `.in.txt` shows no input panel and behaves exactly as before.
- **The leading H1 is dropped.** If a lesson `.md` opens with a level-one heading
  (`# Title`), the parser drops it rather than rendering it — the page already prints the
  catalog's `Title` as the lesson's `<h1>`, so a leading `# ...` would duplicate it. Start the
  file with an H1 anyway if you like (it makes the raw file readable on its own on disk /
  GitHub); just don't expect it to appear on the page.
