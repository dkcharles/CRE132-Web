using CRE132.Web;
using Xunit;

namespace CRE132.Tests;

// The home page derives its four columns from Parts alone: it walks FirstId..LastId and, for
// each id, shows either that id's Entry as a link or WebCatalog.PlannedTitle(part, id) greyed
// out. A part that overlaps another, skips a number, or whose planned titles have slipped out of
// position produces a lesson listed twice, a blank line, or - worst, because it still looks
// right - a lesson advertised under its neighbour's title. Nothing else in the build notices.
//
// These facts are written to hold in EVERY state of the course, not just today's: a part with no
// lessons written, one fully written and one half written all satisfy the same invariant. The day
// lesson 20 is added they must still pass, and only if the change was made correctly.
public class WebCatalogTests
{
    [Fact]
    public void Every_id_in_every_part_resolves_to_a_lesson_or_to_a_planned_title()
    {
        // The invariant the home page depends on, and the one that survives a half-written part:
        // an Entry for 20 covers 20, and 21..26 must each still carry their own planned title AT
        // THEIR OWN POSITION. Deleting 20's title as its row is added - which slides "Vectors"
        // onto 21 and relabels every lesson after it - fails here, and so does adding the row
        // with no title for 20 in the first place.
        foreach (Part p in WebCatalog.Parts)
        {
            for (int id = p.FirstId; id <= p.LastId; id++)
            {
                bool written = WebCatalog.Find(id.ToString()) is not null;
                string planned = WebCatalog.PlannedTitle(p, id);
                Assert.True(written || !string.IsNullOrWhiteSpace(planned),
                    $"'{p.Title}': lesson {id} has neither an Entry nor a planned title");
            }
        }
    }

    [Fact]
    public void No_planned_title_sits_past_the_end_of_its_part()
    {
        // The other half of "indexed by position": a list longer than the span holds a title that
        // can never be shown, the signature of one having been inserted rather than replaced.
        foreach (Part p in WebCatalog.Parts)
        {
            int span = p.LastId - p.FirstId + 1;
            Assert.True(p.PlannedTitles.Count <= span,
                $"'{p.Title}' spans {span} ids but names {p.PlannedTitles.Count} planned titles");
        }
    }

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
    public void PlannedTitle_reads_by_id_position_and_never_throws()
    {
        Part last = WebCatalog.Parts[^1];
        Assert.Equal(last.PlannedTitles[0], WebCatalog.PlannedTitle(last, last.FirstId));
        Assert.Equal(last.PlannedTitles[^1], WebCatalog.PlannedTitle(last, last.LastId));
        // Off either end is empty, not an exception: the home page asks for every id of every
        // part, including the parts that name no planned titles at all.
        Assert.Equal("", WebCatalog.PlannedTitle(last, last.FirstId - 1));
        Assert.Equal("", WebCatalog.PlannedTitle(last, last.LastId + 1));
        Part first = WebCatalog.Parts[0];
        Assert.Equal("", WebCatalog.PlannedTitle(first, first.FirstId));
    }
}
