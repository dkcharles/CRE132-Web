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
}
