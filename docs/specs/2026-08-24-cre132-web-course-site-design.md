# CRE132 Web Course Site — Design

**Date:** 2026-08-24
**Status:** Approved design, pre-implementation
**Replaces:** the Quarto/Jupyter notebook site (CRE132-2026-27) as the student-facing course site

## Purpose

A zero-setup, browser-only course site for teaching beginner C# on CRE132. Students open a
URL; nothing to install, no Codespaces, no .NET SDK, no kernel. All code runs in the page via
WebAssembly, compiled in the browser by Roslyn. The site replaces the notebook site, whose
.NET Interactive setup is too complex for beginners.

The teaching content and the student-facing API are entirely new. The machinery is derived
from the proven CodeSchool/PressStart engine
(`C:\_Work\Teaching\CRE132\AY2627\LabPrep\CodeSchool`, deployed at
<https://dkcharles.github.io/CRE132-CodeSchool/>), with its Unity shim removed.

## Decisions (agreed 2026-08-24)

1. **New repo, full course site** — becomes THE CRE132 site for the whole module.
2. **Console + canvas** — fundamentals teach through console output; later game topics use a
   browser canvas. Raylib is stripped entirely (native library; cannot run under
   browser-WASM Roslyn).
3. **Plain C#** — students write ordinary complete C# programs (top-level statements first,
   then methods and classes), plus a small purpose-built game API for the visual weeks. No
   Unity shim, no Raylib-shaped API.
4. **Fresh progression** — a new logical teaching sequence, mapped back onto teaching weeks;
   notebook content mined where it fits.
5. **Auto-checked challenges** — challenges compare student console output against expected
   output, pass/fail in the page.
6. **Whole site built before term starts.**

## Architecture

New repository, named `CRE132-Web` unless a better name is chosen at creation. Blazor
WebAssembly app + small engine, deployed as static files to GitHub Pages via Actions.

```
/content/            lessons as markdown + companion .cs sample/solution files (the course)
/engine/
  Compiler/          Roslyn compile service (transplanted from CodeSchool nearly as-is)
  Harness/           catalog, run orchestration, challenge checking (adapted)
  LessonDoc/         markdown → JSON generator + build-time C# highlighter (transplanted)
  Tests/             xUnit: golden outputs, challenge expectations, narration validation
/web/                Blazor WASM front-end: page shell, editor, console pane, canvas
/docs/               specs, plans, internal notes
```

### Transplanted from CodeSchool (light changes)

- Lazy Roslyn loading (compiler downloads only on first edit+Run; unmodified samples run
  precompiled so readers never pay for the compiler).
- Reference assemblies shipped as `wwwroot/refs/*.bin` (not `.dll` — proxy-safe; not Webcil).
- Markdown → JSON narration pipeline and directives (`:::run`, `:::edit`, `:::try`,
  `:::key`, `:::figure`), with build-time syntax colouring (no highlighter or Markdig in the
  browser payload).
- Local-storage saves keyed by id with source fingerprints (renumbering lessons self-heals;
  stale saves are ignored, never resurrected).
- Hash-based deep links (`#7`), contents-list navigation, footer pager.
- Fetch-failure handling: every content fetch catches transport errors and shows a notice,
  never Blazor's red bar; load tokens prevent stale loads writing over newer ones.
- Pages deploy details: rewritten `<base href>`, `wwwroot/.nojekyll`,
  `InvariantGlobalization` on (size + pins `.` as decimal separator so expected output
  matches on any locale).

### Removed

UnityShim, MonoBehaviour lifecycle, Unity transfer notes, the Coin Collector.

### New

Plain-program execution model, `:::challenge` directive + checking engine, the 2D game API,
loop-budget instrumentation.

## Execution model (plain C#)

Students write ordinary complete C# programs. Week 1's first program is literally
`Console.WriteLine("Hello");` (top-level statements). Later lessons introduce methods and
classes in the same files.

1. **Output** — before invoking student code, `Console.SetOut` redirects to a writer that
   streams into the page's console pane. Students use the real `Console.WriteLine`;
   everything transfers unchanged to VS Code later.
2. **Input** — `Console.ReadLine` cannot block a single-threaded browser. Pages that need
   input show an **Input panel**: students type input lines before pressing Run, and
   `Console.SetIn` feeds them from a `StringReader`. Never blocks; teaches the real API.
   The curriculum is authored so interactive prompt-response mid-run is never required.
3. **Infinite-loop protection** — a Roslyn syntax rewriter instruments every loop body (and
   any `goto`) with a budget check that throws a friendly "Your program ran too long — look
   for a loop that never ends" after a generous step/time budget. This is the one genuinely
   novel engine component; it is built and tested in Phase 1 before content depends on it.
4. **Compile errors** — shown in-page with line numbers, in beginner-readable form.

## Page format

Each lesson is one markdown file rendered to one page. Elements:

- **Prose** — teaching text; non-runnable fragments as build-time-highlighted code fences.
- **Runnable samples** (`:::run`) — complete program + ▶ Run button; output in a console
  pane beneath. Precompiled.
- **Editable samples** (`:::edit`) — same, but the code is an editor; "try changing…"
  prompts live in the prose. Edits persist per-sample with fingerprints.
- **Challenges** (`:::challenge`, new) — task statement, starter-code editor, **Check**
  button. Checking compiles and runs the student's code against one or more test cases
  (declared stdin lines → expected stdout). Verdict renders in-page: pass ✓, or a
  side-by-side of *your output* vs *expected output* with the first differing line
  highlighted. Expected outputs live beside the challenge definition; the test suite runs
  each challenge's reference solution and fails the build if it does not produce the
  declared expected output.
- **Key ideas** (`:::key`) — two or three highlighted takeaways per page.

**Output comparison tolerance:** trailing whitespace and leading/trailing blank lines
forgiven; everything else exact (`Hello` ≠ `hello` is a lesson, an invisible trailing space
is not).

## Game API (Parts 2–3)

Processing-style: students define `Setup()` (once) and `Draw()` (every frame, ~30fps); the
engine finds them by reflection and drives the loop — inverted, so nothing blocks the WASM
thread.

```csharp
void Setup() { Screen.Size(40, 24); }

void Draw()
{
    Screen.Clear();
    Screen.Circle(x, y, 3, Colour.Red);
    x = x + 1;
}
```

Deliberately small surface, introduced progressively: `Screen` (Size, Clear, Rect, Circle,
Line, Text), `Colour`, `Keys.IsDown(...)`, `Mouse.X/Y/IsPressed`, `Frame.DeltaTime`,
`Rand.Range(...)`. Sufficient for bouncing balls, Pong, Snake, a platformer, steering
behaviours and cellular automata.

Two renderers over one call stream: the browser **canvas**, and a **headless text renderer**
for the test suite, so game lessons have golden tests without a browser. `Console.WriteLine`
still works alongside — the console pane sits under the canvas; that is how debugging is
taught.

The API is a plain C# library, not a framework: students' own classes (`class Ball`) appear
naturally inside `Setup`/`Draw` when the course reaches classes. No entity system is
provided.

**Checking for graphics lessons** checks state, not pixels: a challenge runs the student's
`Draw` for N frames headlessly and asserts positions/counts via the text renderer, or falls
back to console-output checks. Pure "make it look right" moments use compare-with-the-demo
(the working version runs directly above the student's editor).

## Curriculum

Three parts, 27 lessons (0–26). Console-only until Part 2. Every lesson: prose + runnable
samples + editable try-its + at least one auto-checked challenge. Nothing is used before it
is taught.

### Part 1 — Foundations (console only)

| # | Lesson | Core content | Challenge flavour |
|---|--------|--------------|-------------------|
| 0 | Welcome | How the site works; run your first program | Change a message and run it |
| 1 | Your first program | Statements, `Console.WriteLine`, strings, comments | Print a formatted block |
| 2 | Variables & types | `int`, `double`, `string`, `bool`; declare, assign, change | Swap/track values |
| 3 | Maths & operators | Arithmetic, precedence, `%`, interpolation | Calculator tasks |
| 4 | Reading input | `Console.ReadLine`, `int.Parse`, the input panel | Greeting/echo, sums |
| 5 | Making decisions | `if`/`else`, comparisons, `bool` expressions | Pass/fail grader |
| 6 | More decisions | `else if`, `&&`/`||`/`!`, `switch` | Menu chooser, sorter |
| 7 | Repetition | `while`, `for`, counting patterns | Times tables, countdowns |
| 8 | Loop patterns | Accumulators, nested loops, ASCII art | Shapes with loops |
| 9 | Methods | Defining, parameters, return values | Refactor repetition into methods |
| 10 | Scope | Variable lifetime, method vs program scope | Predict-then-verify |
| 11 | Collections | Arrays, `List<T>`, indexing, `foreach` | High-score list, word games |
| 12 | Console project | Guided text mini-game pulling 1–11 together | Multi-step auto-checked build |

### Part 2 — Graphics & motion (game API arrives)

| # | Lesson | Core content |
|---|--------|--------------|
| 13 | First graphics | `Setup`/`Draw`, `Screen`, coordinates, colour |
| 14 | Motion | Velocity, `Frame.DeltaTime`, bouncing off edges |
| 15 | The keyboard | `Keys.IsDown`, moving a player, clamping |
| 16 | The mouse | `Mouse.X/Y`, clicking, hover |
| 17 | Many things | Lists of positions, spawning, falling balls |
| 18 | Collision | Distance checks, circle/rect overlap |
| 19 | **Mini-game: Pong** | Guided build; paddles, ball, score |

### Part 3 — Objects & real games (classes)

| # | Lesson | Core content |
|---|--------|--------------|
| 20 | Your first class | `class Ball` — fields, methods, constructor, `new` |
| 21 | Objects together | Lists of objects, spawn/remove, each object owns its update |
| 22 | Vectors | A simple `Vec2`: direction, magnitude, normalise, chase |
| 23 | Game state | Enums + `switch`: title / playing / game-over screens |
| 24 | Animation & timing | Timers, frame sequences, cooldowns |
| 25 | **Mini-game: Snake** | Guided build using classes, state, collections |
| 26 | Going further | Showcase: steering behaviours, cellular automata — read/run/tinker |

Snake is the second mini-game (classes + collections + state, zero physics); the platformer
can join Lesson 26 as a showcase.

**Week mapping (indicative):** Weeks 1–3 = Part 1, Weeks 4–5 = Part 2, Weeks 6–7 = Part 3 —
the same overall shape as the notebook course, so the lecture plan does not have to move.

## Build order

Ordered so risky engine work is proven before 27 lessons are authored on it, and so a
deployed site exists from the first week of work.

- **Phase 0 — Scaffold & deploy.** New repo; Blazor shell stripped to a walking skeleton;
  Actions → Pages pipeline live with a placeholder page. Deploying first verifies base-href,
  `.nojekyll` and proxy-safe refs on the real host from the start.
- **Phase 1 — Execution engine.** Transplant `Compiler/`; plain-program runner (entry-point
  invocation, `Console.SetOut/SetIn`, loop-budget instrumentation, precompiled-vs-edited
  path, friendly compile errors). xUnit coverage from day one.
- **Phase 2 — Lesson pipeline + pilot.** Transplant `LessonDoc/`; add `:::challenge` and the
  checking engine. Build **one pilot lesson (Lesson 1) end-to-end and deploy it** — settles
  layout, tone and challenge UX while change is cheap. User reviews in the browser before
  mass authoring.
- **Phase 3 — Part 1 content (0–12).** Authored in order; every challenge's reference
  solution lives in the test suite. Milestone: the console course is teachable on its own.
- **Phase 4 — Game API + Part 2 (13–19).** `Screen`/`Keys`/`Mouse`/`Frame`/`Rand`, canvas
  renderer, headless text renderer, golden-frame tests; then lessons 13–19 ending with Pong.
- **Phase 5 — Part 3 content (20–26).**
- **Phase 6 — Polish.** Styling/masthead/phone layout (CodeSchool CSS as the base),
  accessibility check, whole-course clarity read-through, final browser-only device test.

Each phase gets its own implementation plan when reached; this spec fixes *what*, plans fix
*how*.

## Testing (throughout, not a phase)

- Golden output tests for every runnable sample.
- Reference-solution tests for every challenge (build fails if a challenge's own solution
  does not produce its declared expected output).
- Narration validation: a `:::run`/`:::challenge` naming a missing id or file fails the
  build.
- Instrumentation tests: an infinite loop must be caught; a legitimate long-running loop
  must not be.
- Fingerprint/storage tests.
- CI runs the suite before every deploy.

## Error handling

- Student compile errors: in-page, line-numbered, beginner-readable; never a crash.
- Student runtime exceptions: caught at the invocation boundary, message + line shown in the
  console pane.
- Runaway loops: budget instrumentation (above).
- Content-fetch failures: notice in place, app state preserved (CodeSchool's pattern,
  including load-token checks).

## Out of scope

- IntelliSense/autocomplete in the browser editor (no language service in WASM; syntax
  colouring + real compiler errors only).
- Interactive mid-run `Console.ReadLine` prompting (input panel instead).
- Raylib, Unity shim, notebook/Jupyter content.
- Server-side anything: the site is static files.
- Marking/grade capture — challenges are formative self-checking only.
