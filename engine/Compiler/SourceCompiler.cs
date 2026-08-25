using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using CRE132.Engine;

namespace CRE132.Compiler;

// Turns student source text into a runnable program.
//
// This is the only type that names Roslyn, which is why it alone lives in the lazily-loaded
// assembly. CompileResult, CompileError and IReferenceSource live in Harness so that callers
// can hold them across an await without dragging this assembly in early - see the comment in
// CompileContracts.cs.
//
// TWO passes over two trees, deliberately. Errors are reported from the student's own tree so
// line numbers are theirs; execution uses the LoopBudgetRewriter-instrumented tree so infinite
// loops terminate. Instrumenting first and compiling once would shift every diagnostic below
// an injected statement.
public sealed class SourceCompiler
{
    // concurrentBuild MUST stay false. Roslyn's CLS-compliance check spawns worker tasks when
    // it is on and then blocks joining them, and single-threaded WebAssembly throws
    // PlatformNotSupportedException the moment managed code waits on a monitor. Exposed so a
    // test can guard it, because no test running on desktop .NET can catch this by behaviour.
    public static CSharpCompilationOptions CompilationOptions { get; } =
        new(OutputKind.ConsoleApplication, concurrentBuild: false);

    // Mirrors the ImplicitUsings a student's own VS Code project would have, so week 1 code is
    // `Console.WriteLine("Hello");` and nothing else - and still transfers out unchanged.
    static readonly SyntaxTree GlobalUsings = CSharpSyntaxTree.ParseText(
        "global using System;\nglobal using System.Collections.Generic;\nglobal using System.Linq;\nglobal using CRE132.Game;\n");

    readonly IReferenceSource references;
    IReadOnlyList<MetadataReference>? cached;

    public SourceCompiler(IReferenceSource references) => this.references = references;

    public async Task<CompileResult> CompileAsync(string source)
    {
        CompiledBytes compiled = await CompileToBytesAsync(source);
        return compiled.Succeeded
            ? new CompileResult(ProgramLoader.FromBytes(compiled.Bytes!), Array.Empty<CompileError>())
            : new CompileResult(null, compiled.Errors);
    }

    public async Task<CompiledBytes> CompileToBytesAsync(string source)
    {
        cached ??= (await references.GetAsync())
            .Select(bytes => (MetadataReference)MetadataReference.CreateFromImage(bytes))
            .ToList();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(source);

        // Pass 1: the student's own tree - their line numbers, their diagnostics.
        var checkCompilation = CSharpCompilation.Create(
            assemblyName: "StudentCode_" + Guid.NewGuid().ToString("N"),
            syntaxTrees: new[] { GlobalUsings, tree },
            references: cached,
            options: CompilationOptions);

        using (var probe = new MemoryStream())
        {
            EmitResult check = checkCompilation.Emit(probe);
            if (!check.Success)
            {
                var errors = check.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .Select(ToError)
                    .ToList();
                return new CompiledBytes(null, errors);
            }
        }

        // An async entry point would deadlock single-threaded WebAssembly the moment it hit a
        // real await: Roslyn's synthesized wrapper blocks INSIDE the entry point, where no
        // runtime guard can see it. Rejecting at compile time is the only reliable net -
        // checked after pass 1 so a program with ordinary errors reports those, not this.
        IMethodSymbol? entrySymbol = checkCompilation.GetEntryPoint(CancellationToken.None);
        if (entrySymbol is not null && entrySymbol.ReturnType.Name == "Task")
        {
            FileLinePositionSpan? at = entrySymbol.Locations
                .FirstOrDefault(l => l.IsInSource)?.GetLineSpan();
            return new CompiledBytes(null, new[]
            {
                new CompileError(
                    at.HasValue ? at.Value.StartLinePosition.Line + 1 : 1,
                    at.HasValue ? at.Value.StartLinePosition.Character + 1 : 1,
                    "CRE0002",
                    "This playground can't run async programs — remove async, await and Task from your Main method.")
            });
        }

        // Pass 2: same compilation, instrumented tree. The student's code already compiled, so
        // a failure here is OUR bug, reported as such rather than blamed on them.
        SyntaxTree instrumented = LoopBudgetRewriter.Instrument(tree);
        var runCompilation = checkCompilation.ReplaceSyntaxTree(tree, instrumented);

        using var stream = new MemoryStream();
        EmitResult emit = runCompilation.Emit(stream);
        if (!emit.Success)
            return new CompiledBytes(null, new[]
            {
                new CompileError(1, 1, "CRE0001",
                    "Something went wrong on our side preparing your program to run. "
                  + "Your code is fine - please tell your tutor what you typed.")
            });

        return new CompiledBytes(stream.ToArray(), Array.Empty<CompileError>());
    }

    static CompileError ToError(Diagnostic d)
    {
        FileLinePositionSpan span = d.Location.GetLineSpan();
        return new CompileError(
            span.StartLinePosition.Line + 1,       // Roslyn is 0-based; editors are 1-based
            span.StartLinePosition.Character + 1,
            d.Id,
            d.GetMessage());
    }
}
