namespace CRE132.Web;

// Fetches and caches precompiled sample programs. Bytes are cached (they are immutable per
// deploy); every RUN still loads a fresh assembly copy via ProgramLoader, so caching bytes
// cannot leak static state between runs.
public sealed class Samples
{
    readonly HttpClient http;
    readonly Dictionary<string, byte[]> cache = new();

    public Samples(HttpClient http) => this.http = http;

    public async Task<byte[]> FetchAsync(string id)
    {
        if (cache.TryGetValue(id, out byte[]? hit)) return hit;
        byte[] bytes = await http.GetByteArrayAsync($"samples/{id}.bin");
        cache[id] = bytes;
        return bytes;
    }
}
