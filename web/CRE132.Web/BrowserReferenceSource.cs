using CRE132.Engine;

namespace CRE132.Web;

// Fetches the reference assemblies the browser compiles against.
//
// Served as .bin rather than .dll deliberately: some enterprise proxies block .dll downloads,
// and this project has to survive a locked-down university network. They also cannot come from
// _framework, which holds Webcil that Roslyn cannot read.
//
// System.Private.CoreLib is absent on purpose: compiling against the reference pack binds to
// the System.Runtime facade. The names here must match CopyReferences.targets, which fails the
// build if it cannot find all six.
public sealed class BrowserReferenceSource : IReferenceSource
{
    static readonly string[] Names =
    {
        "System.Runtime", "System.Console", "System.Collections",
        "System.Linq", "System.Runtime.Extensions", "Harness"
    };

    readonly HttpClient http;
    IReadOnlyList<byte[]>? cached;

    public BrowserReferenceSource(HttpClient http) => this.http = http;

    public async Task<IReadOnlyList<byte[]>> GetAsync()
    {
        if (cached is not null) return cached;

        var bytes = new List<byte[]>(Names.Length);
        foreach (string name in Names)
            bytes.Add(await http.GetByteArrayAsync($"refs/{name}.bin"));

        cached = bytes;
        return cached;
    }
}
