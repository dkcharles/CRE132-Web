namespace CRE132.Engine;

// These live in Harness, not in the Compiler assembly, and that placement is load-bearing.
//
// The browser loads Roslyn lazily. C# hoists an async method's locals into FIELDS of a
// generated state machine, and the CLR resolves field types when it loads that type - which
// happens when the method is first called, BEFORE any await inside it has run. So an eager
// assembly cannot have an async method with a local typed from the lazy assembly, or the whole
// component fails with "Could not load file or assembly 'Compiler'".
//
// Keeping the contracts here means callers can hold a CompileResult across an await safely.
// Only SourceCompiler itself, which touches Roslyn types directly, stays in the lazy assembly.

public record CompileError(int Line, int Column, string Id, string Message);

// Run invokes the compiled program's entry point once. It is an Action rather than the
// Assembly or MethodInfo so nothing downstream needs reflection types to execute a program.
public record CompileResult(Action? Run, IReadOnlyList<CompileError> Errors)
{
    public bool Succeeded => Run is not null;
}

// Supplies the raw bytes of the assemblies student code compiles against. Abstracted because
// the two callers get them from completely different places: a test reads them off disk, while
// the browser fetches them over HTTP as .bin files.
public interface IReferenceSource
{
    Task<IReadOnlyList<byte[]>> GetAsync();
}
