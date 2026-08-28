using System.Text;
using Markdig;

namespace CRE132.LessonDoc;

// Markdown with six block directives. A directive owns its whole line; everything else
// accumulates and goes through Markdig as ordinary markdown.
public static class NarrationParser
{
    static readonly MarkdownPipeline Pipeline =
        new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

    public static IReadOnlyList<Block> Parse(string markdown)
    {
        var blocks = new List<Block>();
        var prose = new StringBuilder();
        string[] lines = markdown.Replace("\r\n", "\n").Split('\n');

        // A file may open with its own title for disk readers. The page prints the catalog
        // title as its h1, so a leading level-one heading is dropped rather than shown twice.
        int start = 0;
        while (start < lines.Length && lines[start].Trim().Length == 0) start++;
        if (start < lines.Length && lines[start].StartsWith("# ", StringComparison.Ordinal)) start++;
        else start = 0;

        void FlushProse()
        {
            string text = prose.ToString().Trim();
            prose.Clear();
            if (text.Length > 0) blocks.Add(new Block("prose", Html: Render(text)));
        }

        for (int i = start; i < lines.Length; i++)
        {
            string line = lines[i];
            if (!line.StartsWith(":::", StringComparison.Ordinal))
            {
                prose.Append(line).Append('\n');
                continue;
            }

            FlushProse();
            int lineNumber = i + 1;
            string[] parts = line[3..].Trim().Split(' ', 2, StringSplitOptions.TrimEntries);
            string name = parts[0];
            string rest = parts.Length > 1 ? parts[1] : "";

            switch (name)
            {
                case "run":
                case "edit":
                {
                    string[] a = rest.Split(' ', 2, StringSplitOptions.TrimEntries);
                    if (a[0].Length == 0)
                        throw new NarrationException(lineNumber, $":::{name} needs a sample id");
                    blocks.Add(new Block(name, Id: a[0], Caption: a.Length > 1 ? a[1] : ""));
                    break;
                }

                case "figure":
                {
                    string[] a = rest.Split(' ', 2, StringSplitOptions.TrimEntries);
                    if (a[0].Length == 0) throw new NarrationException(lineNumber, ":::figure needs a name");
                    blocks.Add(new Block("figure", Name: a[0], Caption: a.Length > 1 ? a[1] : ""));
                    break;
                }

                case "challenge":
                {
                    if (rest.Length == 0)
                        throw new NarrationException(lineNumber, ":::challenge needs a challenge id");
                    var body = new StringBuilder();
                    int j = i + 1;
                    while (j < lines.Length && lines[j].Trim() != ":::") body.Append(lines[j++]).Append('\n');
                    if (j >= lines.Length)
                        throw new NarrationException(lineNumber, ":::challenge is never closed by a bare :::");
                    blocks.Add(new Block("challenge", Id: rest,
                                         Html: Render(body.ToString().Trim())));
                    i = j;
                    break;
                }

                case "try":
                case "key":
                {
                    var body = new StringBuilder();
                    int j = i + 1;
                    while (j < lines.Length && lines[j].Trim() != ":::") body.Append(lines[j++]).Append('\n');
                    if (j >= lines.Length)
                        throw new NarrationException(lineNumber, $":::{name} is never closed by a bare :::");
                    blocks.Add(new Block("callout", Html: Render(body.ToString().Trim()), Variant: name));
                    i = j;
                    break;
                }

                default:
                    throw new NarrationException(lineNumber, $"unknown directive ':::{name}'");
            }
        }

        FlushProse();
        return blocks;
    }

    // True when the file is narration at all. A markdown file with no directive is
    // documentation that happens to sit beside the lessons, and the generator leaves it alone.
    public static bool HasDirectives(string markdown) =>
        markdown.Replace("\r\n", "\n").Split('\n')
                .Any(l => l.StartsWith(":::", StringComparison.Ordinal));

    static string Render(string markdown) => Highlight(Markdown.ToHtml(markdown, Pipeline).Trim());

    // The same pipeline for markdown that lives outside a lesson file - a challenge's hint.md.
    public static string RenderMarkdown(string markdown) => Render(markdown);

    // A ```csharp fence in the prose gets the same colouring as a sample's listing. Markdig has
    // already HTML-encoded the fence body, and the highlighter encodes for itself, so the body
    // is decoded back to source first. Entities are the only escapes Markdig introduces and the
    // markdown is our own, so a four-entity decode is exact rather than approximate.
    static readonly System.Text.RegularExpressions.Regex CsharpFence = new(
        "<pre><code class=\"language-csharp\">(.*?)</code></pre>",
        System.Text.RegularExpressions.RegexOptions.Singleline);

    static string Highlight(string html) => CsharpFence.Replace(html, m =>
        "<pre><code class=\"language-csharp\">"
        + CSharpHighlighter.Highlight(Decode(m.Groups[1].Value))
        + "</code></pre>");

    // &amp; last, or "&amp;lt;" would decode twice and turn into "<".
    static string Decode(string html) => html
        .Replace("&lt;", "<").Replace("&gt;", ">")
        .Replace("&quot;", "\"").Replace("&amp;", "&");
}
