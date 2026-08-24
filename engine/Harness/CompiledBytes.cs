namespace CRE132.Engine;

// The compiler's raw product. Callers that need one run take CompileResult; callers that need
// many isolated runs (challenge cases) or a file on disk (the build tool) take the bytes and
// load per use via ProgramLoader. Lives in Harness for the same lazy-loading reason as
// CompileResult - see CompileContracts.cs.
public sealed record CompiledBytes(byte[]? Bytes, IReadOnlyList<CompileError> Errors)
{
    public bool Succeeded => Bytes is not null;
}
