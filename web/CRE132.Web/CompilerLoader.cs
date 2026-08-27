using CRE132.Compiler;
using CRE132.Engine;
using Microsoft.AspNetCore.Components.WebAssembly.Services;

namespace CRE132.Web;

// Pulls Roslyn down on demand, which is what keeps a first visit small while the compiler
// costs a few MB more, once, on the first Run.
//
// TWO RULES IN THIS FILE, both about lazy loading, both of which broke CodeSchool when ignored:
//
//  1. The `compiler` field is typed `object`, not `SourceCompiler`. The CLR resolves a type's
//     FIELD types when it loads the type, and this class loads during service registration at
//     startup - long before Roslyn is fetched.
//
//  2. CompileAsync must not hold a SourceCompiler in a local, because C# hoists an async
//     method's locals into fields of a generated state machine, and those resolve on first
//     call rather than after the await. Hence the non-async Bridge method below: no async
//     means no state machine, means no fields, means nothing resolves early.
public sealed class CompilerLoader
{
    readonly LazyAssemblyLoader loader;
    readonly HttpClient http;
    object? compiler;
    Task? loading;

    public CompilerLoader(LazyAssemblyLoader loader, HttpClient http)
    {
        this.loader = loader;
        this.http = http;
    }

    public bool IsLoaded => compiler is not null;
    public bool IsLoading { get; private set; }
    public event Action? StateChanged;

    // Returns a Harness type, so callers can safely hold the result across an await.
    public async Task<CompileResult> CompileAsync(string source)
    {
        await EnsureLoadedAsync();
        return await Bridge(source);
    }

    // Non-async on purpose: see rule 2 above.
    Task<CompileResult> Bridge(string source) => ((SourceCompiler)compiler!).CompileAsync(source);

    public async Task<CompiledBytes> CompileToBytesAsync(string source)
    {
        await EnsureLoadedAsync();
        return await BridgeBytes(source);
    }

    // Non-async on purpose: see rule 2 above.
    Task<CompiledBytes> BridgeBytes(string source) => ((SourceCompiler)compiler!).CompileToBytesAsync(source);

    // Gated: two samples' first Runs can overlap on one page, and each would otherwise start
    // its own LoadAssembliesAsync. The first caller starts the load; everyone awaits the same
    // Task. Single-threaded WASM makes the ??= race-free.
    Task EnsureLoadedAsync() => loading ??= LoadAsync();

    async Task LoadAsync()
    {
        if (compiler is not null) return;

        IsLoading = true;
        StateChanged?.Invoke();
        try
        {
            // Roslyn's dependencies are listed too. They are lazy in the csproj so the first
            // visit does not pay for them, which means they must be requested explicitly here.
            // THIS LIST MUST MATCH the BlazorWebAssemblyLazyLoad items in CRE132.Web.csproj.
            await loader.LoadAssembliesAsync(new[]
            {
                "System.Collections.Immutable.wasm",
                "System.Reflection.Metadata.wasm",
                "System.Text.Encoding.CodePages.wasm",
                "System.Private.Xml.wasm",
                "Microsoft.CodeAnalysis.wasm",
                "Microsoft.CodeAnalysis.CSharp.wasm",
                "Compiler.wasm"
            });

            compiler = Make();
        }
        catch (Exception ex)
        {
            // A failed download must not poison every later attempt: clear the gate so the
            // next Run retries the load instead of awaiting a permanently faulted Task.
            loading = null;
            // Retyped so the owners can say something a beginner can act on. The usual cause
            // is a deploy: the fingerprinted lazy assemblies this page was built against were
            // replaced minutes ago, and only a reload learns the new names - retrying cannot
            // help. The original failure rides along as InnerException for the browser console.
            throw new CompilerUnavailableException(ex);
        }
        finally
        {
            IsLoading = false;
            StateChanged?.Invoke();
        }
    }

    // Also non-async, and also deliberately: constructing SourceCompiler names the lazy type.
    object Make() => new SourceCompiler(new BrowserReferenceSource(http));
}
