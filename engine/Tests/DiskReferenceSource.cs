namespace CRE132.Tests;

using CRE132.Engine;

// Reads the same .bin reference assemblies the browser fetches over HTTP, so these tests
// exercise the exact bytes students compile against. Produced by CopyReferences.targets:
// run `dotnet build web/CRE132.Web` first (CI already builds web before testing).
public sealed class DiskReferenceSource : IReferenceSource
{
    public Task<IReadOnlyList<byte[]>> GetAsync()
    {
        string refs = Path.Combine(RepoPaths.Root, "web", "CRE132.Web", "wwwroot", "refs");
        IReadOnlyList<byte[]> bytes = Directory.GetFiles(refs, "*.bin")
            .OrderBy(f => f)
            .Select(File.ReadAllBytes)
            .ToList();
        return Task.FromResult(bytes);
    }
}
