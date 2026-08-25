using CRE132.Compiler;
using CRE132.Engine;
using Xunit;

namespace CRE132.Tests;

public class SourceCompilerTests
{
    static readonly SourceCompiler Compiler = new(new DiskReferenceSource());

    static async Task<RunResult> CompileAndRun(string source, string input = "", long budget = 1_000_000)
    {
        CompileResult result = await Compiler.CompileAsync(source);
        Assert.True(result.Succeeded, string.Join("\n",
            result.Errors.Select(e => $"line {e.Line}: {e.Message} ({e.Id})")));
        return ProgramRunner.Run(result.Run!, input, budget);
    }

    [Fact]
    public async Task Hello_world_with_top_level_statements_and_no_usings()
    {
        RunResult r = await CompileAndRun("Console.WriteLine(\"Hello, CRE132!\");");
        Assert.Equal("Hello, CRE132!\n", r.Output.Replace("\r\n", "\n"));
        Assert.Null(r.Error);
    }

    [Fact]
    public async Task A_classic_Main_method_works_too()
    {
        RunResult r = await CompileAndRun(
            "class Program { static void Main(string[] args) { Console.WriteLine(\"classic\"); } }");
        Assert.StartsWith("classic", r.Output);
    }

    [Fact]
    public async Task Representative_beginner_programs_compile_against_the_shipped_facades()
    {
        // One program touching everything Part 1 of the course needs. If a facade is missing
        // from refs, THIS fails - fix the facade lists, not this test.
        RunResult r = await CompileAndRun("""
            int n = int.Parse(Console.ReadLine() ?? "0");
            double root = Math.Sqrt(n);
            var scores = new List<int> { 3, 1, 2 };
            scores.Sort();
            string joined = string.Join(",", scores);
            Console.WriteLine($"{root:0.0} {joined} {scores.Count}");
            """, input: "16\n");
        Assert.Equal("4.0 1,2,3 3", r.Output.Trim());
    }

    [Fact]
    public async Task A_compile_error_reports_the_students_line_number()
    {
        CompileResult result = await Compiler.CompileAsync(
            "Console.WriteLine(\"one\");\nConsole.WriteLine(\"two\")\n");
        Assert.False(result.Succeeded);
        CompileError error = result.Errors[0];
        Assert.Equal(2, error.Line);   // the missing ; is on line 2 - never shifted by instrumentation
        Assert.Equal("CS1002", error.Id);
    }

    [Fact]
    public async Task An_infinite_loop_is_stopped_with_the_friendly_message()
    {
        RunResult r = await CompileAndRun("int x = 0;\nwhile (true) { x++; }", budget: 10_000);
        Assert.Contains("loop that never ends", r.Error);
    }

    [Fact]
    public async Task A_legitimate_long_loop_is_not_stopped()
    {
        RunResult r = await CompileAndRun(
            "long sum = 0;\nfor (int i = 0; i < 100000; i++) sum += i;\nConsole.WriteLine(sum);",
            budget: 1_000_000);
        Assert.Null(r.Error);
        Assert.Equal("4999950000", r.Output.Trim());
    }

    [Fact]
    public void Concurrent_build_stays_off_because_WASM_cannot_wait_on_monitors()
    {
        // Roslyn's CLS-compliance check spawns workers and joins them when concurrentBuild is
        // on; single-threaded WebAssembly throws the moment managed code waits on a monitor.
        // No desktop test can catch this by behaviour - blocking is legal there - so the
        // option itself is pinned.
        Assert.False(SourceCompiler.CompilationOptions.ConcurrentBuild);
    }

    [Fact]
    public async Task An_async_Main_is_rejected_at_compile_time_with_a_friendly_error()
    {
        CompileResult result = await Compiler.CompileAsync("""
            class Program
            {
                static async System.Threading.Tasks.Task Main()
                {
                    await System.Threading.Tasks.Task.Delay(1);
                }
            }
            """);
        Assert.False(result.Succeeded);
        CompileError error = result.Errors[0];
        Assert.Equal("CRE0002", error.Id);
        Assert.Equal(
            "This playground can't run async programs — remove async, await and Task from your Main method.",
            error.Message);
    }

    [Fact]
    public async Task CompileToBytes_produces_bytes_that_ProgramLoader_runs()
    {
        CompiledBytes compiled = await Compiler.CompileToBytesAsync("Console.WriteLine(\"bytes\");");
        Assert.True(compiled.Succeeded);
        RunResult r = ProgramRunner.Run(ProgramLoader.FromBytes(compiled.Bytes!), "");
        Assert.StartsWith("bytes", r.Output);
    }

    [Fact]
    public async Task Each_FromBytes_load_is_fresh_so_static_state_cannot_leak_between_cases()
    {
        CompiledBytes compiled = await Compiler.CompileToBytesAsync("""
            class Program
            {
                static int calls = 0;
                static void Main() { calls++; Console.WriteLine(calls); }
            }
            """);
        Assert.True(compiled.Succeeded);
        string first  = ProgramRunner.Run(ProgramLoader.FromBytes(compiled.Bytes!), "").Output;
        string second = ProgramRunner.Run(ProgramLoader.FromBytes(compiled.Bytes!), "").Output;
        Assert.Equal(first, second);   // both "1" - a shared load would print "2" the second time
    }

    [Fact]
    public async Task Student_code_sees_the_game_api_without_a_using()
    {
        CompiledBytes c = await Compiler.CompileToBytesAsync(
            "void Setup() { Screen.Size(640, 360); }\nvoid Draw() { Screen.Circle(1, 2, 3, Colour.Red); }\nGame.Run(Setup, Draw);\n");
        Assert.True(c.Succeeded, string.Join("\n", c.Errors.Select(e => e.Message)));
    }
}
