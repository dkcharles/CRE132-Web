namespace CRE132.Tests;

using Xunit;

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
}
