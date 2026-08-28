# Authoring a CRE132 lesson

This is the working guide for writing content in `content/`. Read it before writing a
lesson, and use it as a checklist while you work. The pilot lesson,
`content/lessons/01-first-program.md`, is the exemplar for voice and structure — when in
doubt about tone, read it again.

## Editing a lesson week to week

The short version, for the lecturer amending pages as the module runs. Everything below
this section is the full rulebook; this is the routine.

**Where things live.** A lesson is one file, `content/lessons/NN-name.md`. The programs it
shows are `content/samples/<id>.cs`, named by the `:::run` / `:::edit` lines in the
lesson. A challenge is four files in `content/challenges/`: `<id>.start.cs` (what the
student sees), `<id>.solution.cs` (the reference answer), `<id>.cases.json` (what is
checked), `<id>.hint.md` (one nudge, offered after the first failed Check; the solution is
offered after the third). Goldens sit beside them (`<id>.out.txt`, `<id>.frame.txt`, `<id>.frames.txt`)
and are generated, never hand-written.

**Prose-only edits** (wording, a key idea, a try-it, a challenge statement's words but not
its numbers): edit the `.md`, then commit and push — or say "check and push". The build
validates every directive line and fails loudly (file and line) if a `:::` is stray or a
sample id is wrong; nothing deploys until it passes, and the previous site stays up. Two
rules that bite: `:::run`/`:::edit` are single lines with no closing `:::`, and
`:::try`/`:::key`/`:::challenge` bodies close with a bare `:::`. Never link to another
lesson with `#n`; name it in words.

**Code edits** (a sample, a starter, a solution, a number in a statement): the goldens
must be regenerated and read. Either describe the change and let Claude make it, or edit
the file and say "check and push" — the routine is then: build; run
`CRE132_UPDATE_GOLDENS=1` for the web build and the content tests (see "Game challenge
kit" below); read every regenerated grid; make sure the challenge statement still names
the exact numbers the solution uses and that every behaviour it asks for is still pinned
by a snapshot (see "Two gates" and "Determinism rules"); run the plain build and the full
test suite; commit; push; open the deployed lesson and press Run. A change to a sample's
numbers is safe; a change to a challenge's rules usually also needs its `cases.json` and
its statement to move together.

**New lesson or new sample**: a new sample needs its `.cs` and an empty `.out.txt` (plus
`.frame.txt` generated if it draws) and the `:::run`/`:::edit` line in the lesson; a new
lesson also needs a row in `web/CRE132.Web/WebCatalog.cs` and, if it falls outside the
last part's numbering, that part's `LastId` raised — the tests say so if you forget.

**Checking it landed.** The deploy takes about two minutes after a push (Actions tab on
GitHub); the Pages cache can show the old page for ten more, so hard-refresh or add
`?v=anything` to the address when looking. If a Check button ever hangs right after a
deploy, reload the page once — the browser fetched a file from before the deploy.

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
      output), plus `<id>.in.txt` only if the sample reads input. (A guided-build lesson may ship
      a single reference sample instead — see "Guided-build lessons" below.)
- [ ] At least one challenge kit in `content/challenges/`: `<id>.start.cs` +
      `<id>.solution.cs` + `<id>.cases.json` + `<id>.hint.md`. (A showcase lesson ships none at all — its kit is
      samples only; see "Showcase lessons" under "Classes (20 onward)".)
- [ ] One row in `WebCatalog.Entries` (`web/CRE132.Web/WebCatalog.cs`), inserted in course
      order, pointing at `lessons/<NN-name>.json`. A lesson numbered past the last `Part`'s
      `LastId` also needs that span raised (or a new `Part` added) with a planned title or an
      Entry in every slot it now covers, or `WebCatalogTests` fails and names the gap.
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
- Challenge ids: `c<NN>-<slug>` — `c01-three-lines`. A lesson with more than one challenge
  letters them the way samples are lettered, in the order the lesson works through them:
  `c19a-paddles`, `c19b-ball`, `c19c-score`.
- Catalog `Id` (the first argument to `Entry`, and the `#hash` a lesson is addressed by): the
  lesson number as a *string*, no padding — `"1"`, `"4"`, `"12"`. This is deliberately not the
  same string as the `NN` in the filename (`"04-..."`): the catalog id is what appears in the
  URL hash and the contents list ("4. Input and parsing"), the filename padding is only so
  files sort in course order on disk.

## Challenges

A challenge kit is four files sharing an id: `<id>.start.cs`, `<id>.solution.cs`,
`<id>.cases.json`, `<id>.hint.md`. Four constraints govern them, none of them optional:

- The starter (`<id>.start.cs`) must **compile and run as shipped**. It may print the wrong
  thing, or an incomplete thing, or nothing at all beyond what's already there — that's the
  point, the student finishes it — but it must never fail to compile. A student's first
  encounter with a challenge can't be a red error list before they've touched anything.
- The solution (`<id>.solution.cs`) is the **minimal idiomatic answer using only concepts
  taught by that lesson**, and no others. No reaching ahead for a shortcut a student in that
  lesson couldn't yet write themselves — a Lesson 5 challenge is solved with `if`/`else` and
  what came before, never with a loop or a method just because it's shorter.
- The hint (`<id>.hint.md`) is plain markdown, one to three sentences, **no `:::` directives**:
  name the construct and the mistake a student is most likely making, not the finished code.
  For a prescriptive game challenge the useful hint is diagnostic — "if frame 120 fails but 45
  passes, the removal is missing". The page offers the hint after the first failed Check and
  the solution itself after the third (a Check only counts when the code changed since the
  last one), so a hint that *is* the solution collapses that ladder. `ContentTests` fails the
  build for a challenge with no hint, and the kit refuses an empty one. The solution shown is
  `<id>.solution.cs` verbatim, highlighted, with a "Copy into the editor" button — one more
  reason it must be the minimal idiomatic answer.
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
| 2 | Variables and types | Variables, types (`int`, `float` with its `f` suffix, `string`, `bool`) |
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
| 13 | First graphics | `Setup`/`Draw`/`Game.Run`, `Screen.Size/Clear/Rect/Circle/Line/Text`, `Colour`, pixel coordinates, variables declared at the top of the file and used inside `Draw` |
| 14 | Motion | A variable changed every frame, speed, wrapping and bouncing at edges, `Screen.Width`, `Screen.Height`, `Frame.Count` |
| 15 | The keyboard | `Keys.IsDown`, `Keys.WasPressed`, `Key`, clamping with `if` |
| 16 | The mouse | `Mouse.X/Y/IsDown/WasClicked`, point-in-rect tests |
| 17 | Many things | `List<float>` positions, `Rand.Range`, spawning, `RemoveAt`, index loops over lists |
| 18 | Collision | `MathF.Sqrt`, `MathF.Abs`, distance, circle/circle and rect/rect overlap as `bool` methods, circle-vs-rect as a rectangle grown by the radius |
| 19 | Mini-game: Pong | Nothing new — a guided build |
| 20 | Your first class | `class`, public fields, methods, a constructor, `new`, classes at the bottom of the file |
| 21 | Objects together | `List<Ball>`, `foreach` over objects, spawning and removing objects |
| 22 | Vectors | A student-written `Vec2` class: `Length`, `Normalised`, `Add`, `Scale` |
| 23 | Game state | `enum`, a state field, `switch` on state, restart |
| 24 | Animation & timing | Countdown timers, `Frame.Count % n`, frame sequences, grid-step movement, cooldowns |
| 25 | Mini-game: Snake | `List<T>.Insert(0, ...)` (introduced here); otherwise a guided build |
| 26 | Going further | `int[,]`, `Rand.Range(float, float)` (first use in the course); showcase only |

Concretely: no `if` before Lesson 5, no loops before Lesson 7, no methods before Lesson 9, no
arrays or `List<T>` before Lesson 11. No `Rand` before Lesson 17, no `MathF.Sqrt`/`MathF.Abs`
before Lesson 18, no `RemoveAt` before Lesson 17. No `class` before Lesson 20, no `enum` before
Lesson 23, no `List<T>.Insert` before Lesson 25, and `int[,]` in Lesson 26 and nowhere else.
String building uses `+` concatenation until Lesson 3 introduces `$"..."` interpolation — don't
reach for interpolation in Lesson 2's samples even though it would read more naturally, because
the reader hasn't met it yet.

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
float x = 100;
float speed = 3;

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
| `Screen` | `Size(int w, int h)`, `Width`, `Height`, `Clear()`, `Clear(Colour)`, `Rect(x, y, w, h, Colour)`, `Circle(x, y, r, Colour)`, `Line(x1, y1, x2, y2, Colour)`, `Text(x, y, string, Colour)` | Coordinates/sizes are `float` in student code (the parameters are `double`, and a `float` argument converts implicitly, so student code never needs a cast). `Size` may be called at any time (renderers read `Width`/`Height` every frame; a change clears the canvas); default 640×360. `Clear()` clears to black. `Rect` is filled; `Text` draws the string with its top-left at (x, y), one character per 16-px cell (the canvas uses a 16-px advance per character, not the font's natural width) so the text renderer and canvas agree on extent. |
| `Colour` | `readonly record struct Colour(byte R, byte G, byte B)`; statics `Black, White, Grey, Red, Orange, Yellow, Green, Cyan, Blue, Purple, Pink`; `Colour.Rgb(r, g, b)` | Named colours are the beginner path; `Rgb` is the escape hatch. |
| `Keys` | `IsDown(Key)`, `WasPressed(Key)` | `WasPressed` is true only on the first frame the key is down (edge detection in C#, from the per-frame snapshot). |
| `Key` (enum) | `Left, Right, Up, Down, Space, Enter, Escape, A … Z, D0 … D9` | Names are what the JS maps `e.key` to. |
| `Mouse` | `X`, `Y` (int, pixels in screen space), `IsDown`, `WasClicked` | `WasClicked` = first frame of a press. |
| `Frame` | `Count` (int, 0 on the first `Draw`), `Time` (float seconds = Count / 30f), `Rate` (const 30) | `Time` is `float` so a `float` variable holds it without a cast. |
| `Rand` | `Range(int min, int maxExclusive)`, `Range(float min, float max)` | Seeded by the host (see Determinism). `Range(double, double)` also exists and returns `float`, so `Rand.Range(1.5, 2.5)` works without the suffix — but write `1.5f` in content, per Style. |
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
- A circle's cell is marked only when that cell's **centre** is within the radius. Worst case,
  the circle's centre lands exactly on a cell corner, and the four surrounding cell centres are
  all √(8²+8²) ≈ 11.3 px away — so a circle with radius ≤ 11 can render as **zero** cells. Use
  radius ≥ 12 for anything a challenge must be able to see at all, and prefer ≥ 16 when you want
  a solid 2×2 blob to show up clearly in a failure diff.

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

The usual four files (`<id>.start.cs`, `<id>.solution.cs`, `<id>.cases.json`, `<id>.hint.md`),
under the same constraints as any challenge (see "Challenges" above). A game challenge's case may additionally
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

For a brand-new game challenge there's no `frames.txt` yet at all, and the content tests compile
against the refs from `dotnet build web/CRE132.Web` — which normally refuses to build a game
case with no `frames.txt`. So set the environment variable for *both* steps, in order: `$env:
CRE132_UPDATE_GOLDENS='1'` (PowerShell) or `CRE132_UPDATE_GOLDENS=1` (bash), then `dotnet build
web/CRE132.Web` (this only warns about the missing file instead of failing), then `dotnet test
engine/Tests --filter Content` to write it. Unset the variable and run a plain `dotnet build
web/CRE132.Web` plus the full `dotnet test engine/Tests` — both must be green before you commit.

The same trick covers *widening* an existing challenge — adding a new case, or a new snapshot
frame to one that's already there — instead of creating one from scratch. The old `frames.txt`
is now stale: it's still present, but no longer covers everything the script needs. With
`CRE132_UPDATE_GOLDENS=1` set, the build only warns about the gap and proceeds, and the content
tests regenerate the file to fill it in. Without the variable, the build fails outright, naming
the exact case or frame block that's missing.

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

### Guided-build lessons

A mini-game lesson — Lesson 19 (Pong) and Lesson 25 (Snake) are the two shipped examples, and
Snake follows Pong's shape exactly, classes and all — is not "lesson plus challenge". It is one
program built in steps, and the shape it uses bends three of the rules above — deliberately, so
copy this shape rather than fighting it:

- **One reference sample, not 3–5.** A single `:::run` at the top shows the finished game, so the
  reader knows what they are building towards. It is the sanctioned exception to "samples fit on
  one screen": Pong's is 76 lines, about a quarter of them the named numbers the game is built
  from, and Snake's is 135, because a class, a list of objects, a grid step and a three-state
  machine all have to be in the file at once. The whole point is that it is a whole program, and
  the lesson then takes it apart. Every other sample the lesson would have had is a challenge step
  instead. The reference sample and the last step's `solution.cs` are the same program: Snake's
  `s25-snake.cs` and `c25c-game-over.solution.cs` are byte-identical, and Pong's two differ only
  by the why-comments the sample carries and the solution does not.
- **Chained challenge kits, one per step.** Each step gets its own three-file kit, lettered in
  order, and **each starter is the previous step's solution** — `c19b-ball.start.cs` is
  `c19a-paddles.solution.cs` with the next comment in it. A student who finishes step 1 carries
  it forward; a student who did not can still start step 2 from working code. The 1–3 cases rule
  is per kit and still holds: it is the *kits* that multiply, never one kit's case list.
- **The `:::try` may target a challenge editor.** With no `:::edit` sample of its own, the
  lesson's `:::try` sends the reader back up to a named challenge's editor to experiment
  ("both changes go in the step 3 editor"), and the prose says so plainly first. The 2–3
  `:::key` rule is unchanged — one per step reads naturally.

Nothing else bends. Every step's statement still names exact numbers, every starter still
compiles, every solution still uses only what the course has taught, and the closing paragraph
points at the next part of the course by name, never by a `#n` hash.

## Classes (20 onward)

Lessons 20–26 put classes on top of the Part 2 game API: Lesson 14's ball stops being five loose
variables and becomes one object that owns its own data and the code that moves it. Nothing else
changes — a class lesson is a game lesson, with the same kit, the same goldens, the same checker
and the same directive grammar as "Game lessons (13 onward)" above. What follows is only what is
different about writing one.

### The file layout

A student program is still top-level statements, and C# allows type declarations *after*
top-level code, so every class program has the same four bands, in this order: the variables and
objects the game needs, then `Setup` and `Draw`, then `Game.Run(Setup, Draw);`, then the `class`
and `enum` declarations underneath it.

```csharp
Ball ball = new Ball(320, 180);

void Setup()
{
    Screen.Size(640, 360);
}

void Draw()
{
    Screen.Clear(Colour.Black);
    ball.Move();
    ball.Draw();
}

Game.Run(Setup, Draw);

class Ball
{
    public float x, y;                                   // fields: what a ball is
    public Ball(float startX, float startY) { ... }      // constructor: how a new one starts
    public void Move() { ... }                           // methods: what a ball does
    public void Draw() { ... }
}
```

Lesson 20 shows this layout once and hands it over as a given, the way Lesson 13 hands over
`Game.Run` — the reader does not need to know *why* the classes sit at the bottom, only that
that is where they go. It is not a house style: a class declared above the top-level statements
is a compiler error, so this is the only order that builds.

### Everything is `public`

Every field and every method of a student class is written `public`. The reason, for you: the
top-level code that calls `ball.Move()` is compiled into a class of its own — the compiler's
`Program` — so the call comes from *outside* `Ball`, and an unmarked C# member is private, which
would not compile. The one line a student reads is: "`public` means code outside the class can
use it." Say it once, in Lesson 20, and move on: `private` is never taught, so there is no second
case to weigh it against and no access levels to explain.

### What is not taught

Fields, methods, constructors, `new`, and lists of objects — that is the whole of it. **No
inheritance, no properties, no `static` members, no interfaces, no operator overloading, no
structs, no `private`.** None of these may appear in a sample, a starter or a solution, however
much shorter they would make it; the same "nothing before it's taught" rule as everywhere else,
except that these are never taught at all.

A student class must also not be named `Game`, `Screen`, `Keys`, `Mouse`, `Frame`, `Rand`,
`Colour` or `Key` — those are the API's own types, and a class of the same name shadows one, so
the API call that used to work stops compiling for reasons a beginner cannot read. `Colour` and
`Key` are on that list because students use them as types (`Colour.Red`, `Key.Space`), not just
as namespaces to call into. Lesson 20 says this in prose, once.

### `Update` and `Draw` on an object are ordinary methods

The engine knows exactly two methods: the top-level `Setup` and the top-level `Draw` handed to
`Game.Run`. A method called `Draw()` or `Update()` on a student class is an ordinary method that
happens to carry a conventional name — nothing calls it for you. `ball.Draw()` inside the
top-level `Draw` is what puts the ball on the screen, and the prose has to say so plainly the
first time, or a reader who has met Unity will assume the engine finds it. Name an object's
per-frame methods `Update` and `Draw` — the Unity habit Part 3 is feeding into — or a plain
verb like `Move` where that reads better; the engine only knows the top-level `Setup` and
`Draw`, so either is fine. Lessons 20–22 use `Move`.

### Checking a class program

Nothing changes. `Screen.Circle` called from inside `Ball.Draw()` marks the same cell of the
same 40×23 grid as the identical call written inline, so the frame goldens, `frames.txt`, the
snapshot rule, the two gates and the golden bootstrap all behave exactly as described above.
The loop budget reaches inside class methods too — the rewriter instruments every loop in the
file, not only the ones in `Draw` — so a runaway `while` in a method is caught the same way.

### Enums

An `enum` is a type declaration, so it goes at the bottom of the file with the classes, and the
field that holds the state goes at the top with the other variables:

```csharp
State state = State.Title;

// Setup elided; the shape below is what matters.
void Draw()
{
    Screen.Clear(Colour.Black);
    switch (state)
    {
        case State.Title:
            Screen.Text(200, 170, "PRESS SPACE", Colour.White);
            break;
        case State.Playing:
            Screen.Circle(x, y, 16, Colour.Yellow);
            break;
    }
}

Game.Run(Setup, Draw);

enum State { Title, Playing, GameOver }
```

The `switch` is Lesson 6's, unchanged — same `case` and `break`, with `State.Playing` where a
number or a string used to be. That is the whole trick, and it is worth saying so: a title
screen and a game-over screen are not an engine feature, they are one variable and a `switch`.

**Space starts, Enter returns to the title** — the convention from Lesson 23 that Snake follows,
and that every later state machine should. The two keys are deliberately different: space is the
key the player was leaning on when they lost, so a game-over screen that also listens for space
clears itself before it can be read. A `Reset()` that puts every one of the round's variables
back is the other half of it, and *where* it is called follows from what the title screen draws:
a title screen that shows nothing of the round resets on the way into a round (`s23a-restart`),
and one that draws the board resets on the way out of game over (Snake, and `c23-game-over`),
so the title never shows the corpse of the last round.

### 2D arrays

`int[,] cells = new int[cols, rows];`, read and written as `cells[x, y]`, appears in **Lesson 26
and nowhere else** — Conway's Life needs a grid and nothing before it does. Everything else that
holds many things is a `List<T>` (Lesson 11) or a list of objects (Lesson 21). Lesson 26's prose
calls it "a grid of ints" and leaves it there; array rank is not a concept the course teaches.

### Showcase lessons (Part 4)

Lesson 26 is the first of a different kind of page: read it, run it, tinker with it. A showcase
lesson is `:::run` samples and `:::try` prompts and **no challenges at all** — nothing is being
tested, so there is nothing to check beyond each sample's own goldens. Its file kit is therefore
samples only: `<id>.cs` + `<id>.frame.txt` + `<id>.out.txt` per sample, and nothing in
`content/challenges/`. Ship one `:::edit` copy of a sample so the `:::try` prompts have an
editor to land in and the reader can change a number without leaving the page. The 2–3 `:::key`
rule and the "at least one `:::try`" rule still hold; "at least one challenge" is the single
line of the file kit a showcase lesson is exempt from.

A showcase sample is also allowed past the one-screen limit, for the same reason a guided build's
is: it is a whole program on purpose. Lesson 26 ships three of them — `s26-steering` (agents that
seek, flee and wander), `s26a-life` (Conway's Life on the course's only `int[,]`) and
`s26b-particles` (a burst of short-lived objects), plus `s26c-particles-edit`, the `:::edit` copy
of the third. `s26-steering` runs to about a hundred lines because thirty-one of them are Lesson
22's `Vec2`, copied in verbatim — a showcase may not reach for a shortcut a reader has not been
taught, so the class comes with it rather than being replaced by something shorter. Say so in the
prose when it happens, so a reader who counts the lines knows why they are there. A `:::try` on a
sample with no `:::edit` of its own points at the **Playground** by name, exactly as a guided
build's points at a named challenge editor.

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
- Samples fit on one screen; if a sample needs scrolling to read, it's doing too much. There are
  two sanctioned exceptions, both whole programs on purpose: a guided-build lesson's single
  reference sample (Pong's 76 lines, Snake's 135 — see "Guided-build lessons") and a showcase
  lesson's samples (Lesson 26's three, the longest around a hundred lines — see "Showcase
  lessons").
- **`float`, never `double` — and the `f` goes on decimals only.** Every decimal number a student
  writes is a `float`, because that is the type Unity uses and the course feeds into Unity. The
  convention, held to everywhere in `content/`: a **decimal** literal carries the suffix
  (`float gravity = 0.5f;`, `speedY * 0.8f`), a **whole** number does not (`float speed = 3;`,
  `float radius = 20;`) — the implicit int-to-float conversion covers it and the bare number reads
  better to a beginner. `List<double>` is `List<float>`; `Math.Sqrt`/`Math.Abs` are
  `MathF.Sqrt`/`MathF.Abs`, which return `float`, so no cast is ever needed and no cast is ever
  taught. Prose follows the same rule: a quoted line or a `:::key` naming the type says `float`.
  Lesson 2 is the one place `double` is named at all, in a single sentence saying the course does
  not use it.
- **A number used twice gets a name.** When the same literal appears two or more times in one
  program with the same meaning — a radius, a paddle height, a speed, a screen edge — declare it
  once as a plain variable at the top of the file (`float radius = 20;`, `int screenWidth = 640;`)
  and use the name everywhere, writing derived values as expressions of it (`640 - radius`, not
  `620`). A literal used once stays a literal. Plain variables only: `const`, `static` and
  `readonly` are never taught, and Lessons 0–1 have no variables at all.
- **Comment the *why*, never the *what*.** One short line in plain English above a block whose
  reason a beginner would not guess — `// Step the index back, or the item that slid into the gap
  is skipped` — and none at all above a line that already says what it does. One to four per
  sample is typical. A starter's `// TODO`-style markers are load-bearing (the prose points at
  them by position); leave them exactly as they are and don't add extra comments beside them.
- **Names say what they hold.** `starCount`, not `n`; `sweetsEach`, not `val`; `wallHeight`, not
  `wh`. The exceptions are the conventional short names — loop indices `i`/`j`, coordinates
  `x`/`y`, deltas `dx`/`dy` — and any name the lesson prose itself introduces and discusses, which
  must not be renamed without rewriting the prose that teaches it.

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
  no `.in.txt` shows no input panel and behaves exactly as before. A challenge gets the same
  panel automatically whenever any of its `cases.json` entries has a non-empty `input`,
  prefilled with the first case's input: ▶ Run feeds the panel to the program, Check ignores
  it and uses the cases. Nothing to author — it follows from `cases.json`.
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
