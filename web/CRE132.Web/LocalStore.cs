using Microsoft.JSInterop;

namespace CRE132.Web;

// Per-entry auto-save, so edits to Lesson 2 and Lab 1 do not collide. Work survives a refresh
// or a closed tab, but not clearing site data and not moving to another machine - the page says
// so, because a lab machine that resets profiles overnight will eat it.
//
// Each save records the fingerprint of the source it was made against, in a second key. That is
// what stops a save outliving its lesson: renumbering the course reused ids 1-10 for different
// files, and a returning visitor got the old lesson's work in the new lesson's slot, presented
// as their own edit. On a mismatch the save is simply not offered, so the fix costs nothing and
// covers every future reshuffle rather than just that one. Anything from the pre-fingerprint
// scheme has no base key, reads as a mismatch, and is therefore ignored - correct, since those
// are exactly the orphaned saves.
//
// Mismatched values are left in place rather than deleted: they are already unreachable, the
// next save overwrites both keys anyway, and quietly destroying something a student might have
// written is worse than leaving a few kilobytes behind.
public sealed class LocalStore
{
    const string SourcePrefix = "cre132.entry.";
    const string BasePrefix = "cre132.base.";

    readonly IJSRuntime js;

    public LocalStore(IJSRuntime js) => this.js = js;

    public async Task<string?> LoadAsync(string entryId, string shippedFingerprint)
    {
        string? savedAgainst = await js.InvokeAsync<string?>("localStorage.getItem", BasePrefix + entryId);
        if (savedAgainst != shippedFingerprint) return null;

        return await js.InvokeAsync<string?>("localStorage.getItem", SourcePrefix + entryId);
    }

    // Source first, fingerprint second, deliberately. If the second write fails - a full quota
    // is the realistic case - the next load sees a mismatch and falls back to the shipped file.
    // The other order would leave the old source looking freshly valid, which is the one outcome
    // worth ruling out: showing the wrong code as if the student wrote it.
    public async Task SaveAsync(string entryId, string source, string shippedFingerprint)
    {
        await js.InvokeVoidAsync("localStorage.setItem", SourcePrefix + entryId, source);
        await js.InvokeVoidAsync("localStorage.setItem", BasePrefix + entryId, shippedFingerprint);
    }

    public async Task ClearAsync(string entryId)
    {
        await js.InvokeVoidAsync("localStorage.removeItem", SourcePrefix + entryId);
        await js.InvokeVoidAsync("localStorage.removeItem", BasePrefix + entryId);
    }
}
