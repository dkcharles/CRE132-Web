namespace CRE132.Tests;

/// <summary>Finds the repository root from the test assembly's location,
/// so file-reading tests work locally and in CI without configuration.</summary>
public static class RepoPaths
{
    public static string Root { get; } = Find();

    static string Find()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !Directory.Exists(Path.Combine(dir.FullName, ".git")))
            dir = dir.Parent;
        return dir?.FullName
            ?? throw new InvalidOperationException("No .git directory above " + AppContext.BaseDirectory);
    }
}
