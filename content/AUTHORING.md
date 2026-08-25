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
- [ ] For a game lesson (13 onward): each game sample kit adds `<id>.frame.txt` (the golden
      final frame) alongside its `.cs` and `.out.txt`.
- [ ] For a game lesson, a game challenge case carrying a `game` script needs a matching
      `<id>.frames.txt` (the golden frames its `snapshot`s are checked against).
- [ ] For a game challenge, the task statement names the exact positions, sizes, count, or
      text expected — see "Game lessons (13 onward)" below.

Nothing in this kit is optional scaffolding — a `:::run` naming a sample with no `.cs` file,
or a `:::challenge` naming an id missing any of its three files, is a build error, not a
runtime one.

## Id conventions

- Lesson files: `<NN-name>.md`, two-digit zero-padded number, kebab-case name —
  `01-first-program.md`, `04-reading-input.md`.
- Sample ids: `s<NN>-<slug>` — `s01-hello`, `s01-edit-message`. The `s` prefix, the lesson
  number, and a short slug naming what the sample shows. A lesson's first sample carries no
  letter; each additional sample after it adds one, in order: `sNNa-`, `sNNb-`, `sNNc-`, ... —
  e.g. a lesson's samples run `s05-if`, `s05a-else`, `s05b-bool`.
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
  statement says `CRE132`, not "the course code." For a game challenge (see "Game lessons
  (13 onward)" below) the same rule covers geometry: state the exact position, size, count,
  or text expected, never "somewhere on the right" or a colour alone.
- A challenge's solution reads input **silently**: it prints only the answer output, never a
  prompt asking for what to type. Samples MAY prompt before a `ReadLine()` (that's fine, even
  helpful, in a `:::run`/`:::edit`); a challenge must not, because the checker compares the
  program's printed lines against `expected` starting from the first line — a prompt line
  would shift everything after it out of alignment and fail every case.

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
| 2 | Variables and types | Variables, types |
| 3 | Maths and operators | Arithmetic operators, `$"..."` interpolation |
| 4 | Reading input | `Console.ReadLine`, the input panel, `int.Parse` |
| 5 | Making decisions | `if`/`else`, comparisons, `bool` |
| 6 | More decisions | `else if`, `&&`/`\|\|`/`!`, `switch` |
| 7 | Repetition | `while`, `for` |
| 8 | Loop patterns | Accumulators, nested loops, ASCII art |
| 9 | Methods | Methods, parameters, return values |
| 10 | Scope | Variable scope |
| 11 | Collections | Arrays, `List<T>`, `foreach` |
| 12 | Console project: The Snack Machine | Console project (Snack Machine) |
| 13 | First graphics | `Setup`/`Draw`/`Game.Run`, `Screen.Size/Clear/Rect/Circle/Line/Text`, `Colour`, pixel coordinates |
| 14 | Motion | A variable changed every frame, speed, bouncing at edges, `Frame.Count` |
| 15 | The keyboard | `Keys.IsDown`, `Keys.WasPressed`, `Key`, clamping with `if` |
| 16 | The mouse | `Mouse.X/Y/IsDown/WasClicked`, point-in-rect tests |
| 17 | Many things | `List<double>` positions, `Rand.Range`, spawning, `RemoveAt`, index loops over lists |
| 18 | Collision | `Math.Sqrt`, `Math.Abs`, distance, circle/rect overlap as `bool` methods |
| 19 | Mini-game: Pong | Nothing new — a guided build |

Concretely: no `if` before Lesson 5, no loops before Lesson 7, no methods before Lesson 9, no
arrays or `List<T>` before Lesson 11. No `Rand` before Lesson 17, no `Math.Sqrt`/`Math.Abs`
before Lesson 18, no `RemoveAt` before Lesson 17. String building uses `+` concatenation until
Lesson 3 introduces `$"..."` interpolation — don't reach for interpolation in Lesson 2's
samples even though it would read more naturally, because the reader hasn't met it yet.

## Game lessons (13 onward)

Lessons 13–19 teach a Processing-style game API (`Screen`, `Colour`, `Keys`, `Mouse`, `Frame`,
`Rand`, `Game`) in place of console I/O. The kit for these lessons adds two things a console
lesson doesn't have: a golden **frame** (what the canvas looks like), alongside the golden
console output, and a headless renderer the checker uses to compare frames. Everything else —
the directive grammar, id conventions, a challenge's three-file shape and three constraints,
the output tolerance rule — still applies exactly as written above.

### The program shape

Every game sample and challenge follows the shape of the spec's complete Part 2 program:

```csharp
double x = 100;
double speed = 3;

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Screen.Circle(x, 180, 12, Colour.Yellow);
    x = x + speed;
    if (x > Screen.Width) x = 0;
}

Game.Run(Setup, Draw);
```

`Setup` runs once, before the first frame; `Draw` runs 30 times a second, once per frame. The
program ends with `Game.Run(Setup, Draw);`. Both `Setup` and `Draw` are ordinary local methods
(Lesson 9), and variables the game needs (`x` and `speed` above) are declared at the top of the
file, outside either method, so both can see and change them. `Console.WriteLine` still works
throughout — it prints to the console pane under the canvas, same as any console lesson, and is
how debugging a game is taught.

### The API

| Type | Members | Notes |
|------|---------|-------|
| `Screen` | `Size(int w, int h)`, `Width`, `Height`, `Clear()`, `Clear(Colour)`, `Rect(x, y, w, h, Colour)`, `Circle(x, y, r, Colour)`, `Line(x1, y1, x2, y2, Colour)`, `Text(x, y, string, Colour)` | Coordinates/sizes are `double`. `Size` may be called at any time (renderers read `Width`/`Height` every frame; a change clears the canvas); default 640×360. `Clear()` clears to black. `Rect` is filled; `Text` draws the string with its top-left at (x, y), one character per 16-px cell (the canvas uses a 16-px advance per character, not the font's natural width) so the text renderer and canvas agree on extent. |
| `Colour` | `readonly record struct Colour(byte R, byte G, byte B)`; statics `Black, White, Grey, Red, Orange, Yellow, Green, Cyan, Blue, Purple, Pink`; `Colour.Rgb(r, g, b)` | Named colours are the beginner path; `Rgb` is the escape hatch. |
| `Keys` | `IsDown(Key)`, `WasPressed(Key)` | `WasPressed` is true only on the first frame the key is down (edge detection in C#, from the per-frame snapshot). |
| `Key` (enum) | `Left, Right, Up, Down, Space, Enter, Escape, A … Z, D0 … D9` | Names are what the JS maps `e.key` to. |
| `Mouse` | `X`, `Y` (int, pixels in screen space), `IsDown`, `WasClicked` | `WasClicked` = first frame of a press. |
| `Frame` | `Count` (int, 0 on the first `Draw`), `Time` (double seconds = Count / 30.0), `Rate` (const 30) | |
| `Rand` | `Range(int min, int maxExclusive)`, `Range(double min, double max)` | Seeded by the host (see Determinism). |
| `Game` | `Run(Action setup, Action draw)` | Registers and returns. Calling it twice, or calling it with a null, throws with a plain message. No `Stop`: game-over screens arrive with enums in Lesson 23. |

Every game lesson holds to three rules:

- Pixels are 640×360 by default, `(0, 0)` is top-left, and y increases **downward** — up is
  negative y, the opposite of a maths graph.
- Speeds are pixels per frame at a fixed 30 fps: "speed 3" moves a shape 3 pixels each `Draw`
  call, i.e. 90 pixels a second.
- The canvas is **not** cleared for you. Call `Screen.Clear()` (or `Screen.Clear(Colour.Black)`)
  first in `Draw`, every frame — leaving it out draws trails, because every earlier frame's
  shapes stay on screen and the new frame's shapes pile on top.

### What the checker can see

The headless renderer draws each frame onto a 40×23 grid of 16-px cells: `#` for a filled rect,
`o` for a circle, `+` for a line, letters for `Screen.Text`, blank for nothing. Later shapes
overwrite earlier ones in the same cell. Colour is invisible to the grid.

Consequences for authoring:

- A challenge must ask for a change in **position, size, count, or text** — never colour alone.
  "Make the circle red" is not checkable; "move the circle to x = 300" is.
- Positions that differ by less than a cell may compare equal. This is a deliberate tolerance,
  not a bug — but don't rely on a 2-px difference to distinguish a right answer from a wrong
  one; separate them by at least a cell.
- A shape that leaves the screen disappears from the grid entirely, same as it would on the
  real canvas.

### Game sample kit

A game sample is `<id>.cs` + `<id>.frame.txt` + `<id>.out.txt` — the presence of `.frame.txt`
is what marks a sample as a game sample rather than a console one. `<id>.frame.txt` is the
golden final frame (the 40×23 grid after the last `Draw` call); `<id>.out.txt` is the console
golden, same as a console sample — an empty file if the sample prints nothing.

A sample may optionally carry `<id>.game.json`, test-only (it configures the checker's run; the
student never sees it):

```json
{ "frames": 60, "keys": { "Right": "10-30", "Space": "5" }, "mouse": { "x": 320, "y": 180, "down": "5-8" } }
```

Defaults are 60 frames and no input. Frame ranges are inclusive and 1-based (`"10-30"` means
frames 10 through 30, both included); a single number (`"5"`) is one frame.

### Game challenge kit

The usual three files (`<id>.start.cs`, `<id>.solution.cs`, `<id>.cases.json`), under the same
constraints as any challenge (see "Challenges" above). A game challenge's case may additionally
carry a `game` script:

```json
[
  { "game": { "frames": 60, "snapshot": [30, 60],
              "keys": { "Right": "10-30" }, "mouse": { "x": 100, "y": 50, "down": "5-8" } } },
  { "game": { "frames": 20, "snapshot": [20] }, "expected": "Score: 1" }
]
```

`frames` is how many `Draw` calls to run; `snapshot` is the list of 1-based frame numbers whose
grid is compared; `expected` is optional on a game case (when present, the console output is
compared as well as the frames); `input` still works exactly as on a console challenge.

Expected frames live in `<id>.frames.txt`, one block per case, one `--- frame N ---` block per
snapshot:

```
=== case 1 ===
--- frame 30 ---
<23 rows of 40 chars>
--- frame 60 ---
<23 rows>
=== case 2 ===
--- frame 20 ---
<23 rows>
```

You never hand-write `frames.txt`. Run `CRE132_UPDATE_GOLDENS=1 dotnet test engine/Tests
--filter Content` once and the solution gate writes any missing `frames.txt` / `.frame.txt`
from the reference solution (or the sample) — review the grids in the diff, then commit them.
Without the environment variable, a missing golden fails the test and prints the grid it would
have written, so you can see what's wrong without generating anything.

### Determinism rules

Under the checker, `Rand` is seeded (12345) and time is fixed, so the reference solution and a
student's program get the *same* random numbers only if they call `Rand.Range` the same number
of times, in the same order. A challenge that uses `Rand` must state exactly when and with what
arguments to call it — or avoid `Rand` entirely. Keys in a `game` script are held down for
every frame of their range, so `WasPressed` fires on the first frame of that range, exactly as
a real keypress would.

### Two gates

A game challenge's reference solution must reproduce `frames.txt` exactly, the same as its
console `expected` output is checked — and the **starter must not already pass**, the same
rule that governs every challenge, console ones included.

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
- **Colour is invisible to the checker.** The frame grid records shape, position, size, count,
  and text — never colour. A challenge that only asks the student to change a colour has no way
  to be checked; ask for a geometry or text change instead.
- **`Rand` call order.** The checker seeds `Rand` the same way for the reference solution and
  the student's program, so they only see the same random numbers if they call `Rand.Range` the
  same number of times, in the same order. A `Rand`-using challenge must pin that order down in
  the task statement.
- **`Screen.Text` is one character per 16-px cell.** Text longer than 40 characters runs past
  the right edge of the grid and the overflow simply disappears — it never wraps.
- **A game sample that never calls `Screen.Clear` accumulates.** Every earlier frame's shapes
  stay on screen and pile up. That's intended for a trails demo; anywhere else, it's a bug —
  check that every `Draw` you write starts with `Screen.Clear()`.
