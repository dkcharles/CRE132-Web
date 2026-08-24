using System.Reflection;

namespace CRE132.Engine;

// Turns a compiled program's bytes into something runnable. EVERY call loads a fresh copy of
// the assembly: static fields in student code then start from zero on every run, which is what
// lets a challenge run one program against several cases without runs contaminating each
// other. Loaded assemblies cannot be unloaded - acceptable, they are a few KB each.
public static class ProgramLoader
{
    public static Action FromBytes(byte[] assemblyBytes)
    {
        Assembly assembly = Assembly.Load(assemblyBytes);
        MethodInfo entry = assembly.EntryPoint
            ?? throw new InvalidOperationException("The compiled program has no entry point.");

        return () =>
        {
            object?[]? args = entry.GetParameters().Length == 1
                ? new object?[] { Array.Empty<string>() }
                : null;
            object? result = entry.Invoke(null, args);
            if (result is Task task)
            {
                // Blocking on a pending Task can never succeed on single-threaded WebAssembly.
                // SourceCompiler rejects async entry points at compile time (CRE0002); this is
                // the second net for bytes that arrived some other way.
                if (!task.IsCompleted)
                    throw new InvalidOperationException(
                        "This playground can't run programs that wait on async work — remove async/await.");
                task.GetAwaiter().GetResult();
            }
        };
    }
}
