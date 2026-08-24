using System.Linq;
using Xunit;

namespace CRE132.Tests;

public class DeployContractTests
{
    static string WwwRoot => Path.Combine(RepoPaths.Root, "web", "CRE132.Web", "wwwroot");

    [Fact]
    public void IndexHtml_contains_the_exact_base_href_the_deploy_rewrite_targets()
    {
        // deploy-web.yml rewrites this exact string to /CRE132-Web/. If it drifts
        // (formatting, quotes, spacing), the deploy still "succeeds" and the live
        // site 404s every asset.
        var html = File.ReadAllText(Path.Combine(WwwRoot, "index.html"));
        Assert.Contains("<base href=\"/\" />", html);
    }

    [Fact]
    public void Nojekyll_exists_so_Pages_serves_the_framework_directory()
    {
        // Without it, GitHub Pages runs Jekyll, which skips directories starting
        // with an underscore - exactly where Blazor puts its runtime.
        Assert.True(File.Exists(Path.Combine(WwwRoot, ".nojekyll")),
            ".nojekyll is missing from wwwroot");
    }

    [Fact]
    public void Web_csproj_keeps_InvariantGlobalization_on()
    {
        // The spec mandates it: it drops ~2.5 MB of ICU data AND pins '.' as the
        // decimal separator whatever locale the visitor's browser reports -
        // challenge expected-output matching depends on it.
        var csproj = File.ReadAllText(Path.Combine(RepoPaths.Root,
            "web", "CRE132.Web", "CRE132.Web.csproj"));
        Assert.Contains("<InvariantGlobalization>true</InvariantGlobalization>", csproj);
    }

    [Fact]
    public void The_six_reference_assemblies_are_copied_for_the_browser_compiler()
    {
        // Produced by CopyReferences.targets on every web build. If one is missing, in-browser
        // compilation fails at runtime; run `dotnet build web/CRE132.Web` before testing.
        string refs = Path.Combine(WwwRoot, "refs");
        string[] expected = { "System.Runtime", "System.Console", "System.Collections",
                              "System.Linq", "System.Runtime.Extensions", "Harness" };
        foreach (string name in expected)
            Assert.True(File.Exists(Path.Combine(refs, name + ".bin")),
                $"{name}.bin missing from wwwroot/refs — build web/CRE132.Web first");
    }

    [Fact]
    public void The_lazy_load_lists_in_csproj_and_CompilerLoader_match()
    {
        // Drift between these fails only at runtime, only in a browser. Comments say
        // "must match"; this test makes the build say it.
        string csproj = File.ReadAllText(Path.Combine(RepoPaths.Root,
            "web", "CRE132.Web", "CRE132.Web.csproj"));
        string loader = File.ReadAllText(Path.Combine(RepoPaths.Root,
            "web", "CRE132.Web", "CompilerLoader.cs"));

        var fromCsproj = System.Text.RegularExpressions.Regex
            .Matches(csproj, "BlazorWebAssemblyLazyLoad Include=\"([^\"]+)\"")
            .Select(m => m.Groups[1].Value).OrderBy(n => n).ToList();
        var fromLoader = System.Text.RegularExpressions.Regex
            .Matches(loader, "\"([A-Za-z0-9.]+\\.wasm)\"")
            .Select(m => m.Groups[1].Value).OrderBy(n => n).ToList();

        Assert.Equal(fromCsproj, fromLoader);
        Assert.NotEmpty(fromCsproj);
    }

    [Fact]
    public void The_facade_lists_in_targets_and_BrowserReferenceSource_match()
    {
        string targets = File.ReadAllText(Path.Combine(RepoPaths.Root,
            "web", "CRE132.Web", "CopyReferences.targets"));
        string source = File.ReadAllText(Path.Combine(RepoPaths.Root,
            "web", "CRE132.Web", "BrowserReferenceSource.cs"));

        var fromTargets = System.Text.RegularExpressions.Regex
            .Matches(targets, "'%\\(Filename\\)' == '([^']+)'")
            .Select(m => m.Groups[1].Value).OrderBy(n => n).ToList();
        var fromSource = System.Text.RegularExpressions.Regex
            .Matches(source, "\"([A-Za-z0-9.]+)\"(?=[,\\s})])")
            .Select(m => m.Groups[1].Value)
            .Where(n => !n.EndsWith(".bin")).OrderBy(n => n).ToList();

        Assert.Equal(fromTargets, fromSource);
        Assert.Equal(6, fromTargets.Count);
    }
}
