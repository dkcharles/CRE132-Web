using CRE132.Web;
using Xunit;

namespace CRE132.Tests;

// The home page derives its three columns from Parts alone: it walks FirstId..LastId and looks
// each id up in Entries. A part that overlaps another, skips a number, or names the wrong count
// of planned titles produces a lesson that is listed twice, or vanishes from the site entirely,
// with no build error anywhere. These facts are what stop that.
public class WebCatalogTests
{
    [Fact]
    public void Every_lesson_belongs_to_exactly_one_part()
    {
        foreach (Entry e in WebCatalog.Entries)
        {
            Assert.True(int.TryParse(e.Id, out int n), $"lesson id '{e.Id}' is not a number");
            int owners = WebCatalog.Parts.Count(p => n >= p.FirstId && n <= p.LastId);
            Assert.True(owners == 1, $"lesson {e.Id} is claimed by {owners} parts, expected 1");
        }
    }

    [Fact]
    public void The_parts_are_contiguous_and_do_not_overlap()
    {
        Assert.NotEmpty(WebCatalog.Parts);
        Assert.Equal(0, WebCatalog.Parts[0].FirstId);
        for (int i = 0; i < WebCatalog.Parts.Count; i++)
        {
            Part p = WebCatalog.Parts[i];
            Assert.True(p.LastId >= p.FirstId, $"'{p.Title}' spans backwards ({p.FirstId}..{p.LastId})");
            if (i > 0)
                Assert.Equal(WebCatalog.Parts[i - 1].LastId + 1, p.FirstId);
        }
    }

    [Fact]
    public void A_part_with_no_lessons_written_names_one_planned_title_per_id()
    {
        var unwritten = WebCatalog.Parts.Where(p => WebCatalog.EntriesIn(p).Count == 0).ToList();
        Assert.NotEmpty(unwritten);
        foreach (Part p in unwritten)
        {
            int span = p.LastId - p.FirstId + 1;
            Assert.True(p.PlannedTitles.Count == span,
                $"'{p.Title}' spans {span} ids but names {p.PlannedTitles.Count} planned titles");
            Assert.All(p.PlannedTitles, t => Assert.False(string.IsNullOrWhiteSpace(t)));
        }
    }

    [Fact]
    public void A_part_whose_lessons_are_all_written_names_no_planned_titles()
    {
        foreach (Part p in WebCatalog.Parts)
        {
            int span = p.LastId - p.FirstId + 1;
            if (WebCatalog.EntriesIn(p).Count != span) continue;
            Assert.True(p.PlannedTitles.Count == 0,
                $"'{p.Title}' is fully written but still names planned titles");
        }
    }

    [Fact]
    public void EntriesIn_returns_that_parts_entries_in_catalog_order()
    {
        var seen = new List<Entry>();
        foreach (Part p in WebCatalog.Parts)
        {
            IReadOnlyList<Entry> got = WebCatalog.EntriesIn(p);
            // Catalog order, not numeric order or dictionary order.
            Assert.Equal(WebCatalog.Entries.Where(got.Contains).ToList(), got);
            seen.AddRange(got);
        }
        // Between them the parts account for the whole catalog, once each.
        Assert.Equal(WebCatalog.Entries, seen);
    }

    [Fact]
    public void Part_three_names_the_planned_Snake_curriculum()
    {
        Part p = WebCatalog.Parts[^1];
        Assert.Equal(20, p.FirstId);
        Assert.Equal(26, p.LastId);
        Assert.Equal(
            new[] { "Your first class", "Objects together", "Vectors", "Game state",
                    "Animation & timing", "Mini-game: Snake", "Going further" },
            p.PlannedTitles);
    }
}
