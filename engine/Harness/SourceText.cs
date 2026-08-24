using System.Globalization;
using System.Text;

namespace CRE132.Engine;

// Line endings decide whether the browser downloads a 3 MB compiler.
//
// Source ships as .txt copied on Windows, so it carries CRLF; a browser editor hands text back
// with LF. Compared raw, EVERY sample looks edited, every visitor downloads Roslyn, and nothing
// appears broken. Both sides go through here first.
public static class SourceText
{
    public static string Normalise(string text) => text.Replace("\r\n", "\n").Replace("\r", "\n");

    // Identifies which shipped source an edit was made against, so the browser can tell a
    // student's real work from a save left behind by a file that has since been replaced.
    //
    // FNV-1a 64-bit, written out rather than taken from the framework for two reasons.
    // string.GetHashCode is randomised per process, so it would discard every save on every
    // visit. And a cryptographic hash would be both overkill and heavier in the browser: this
    // only has to notice that a baseline changed, never resist anyone trying to forge one.
    public static string Fingerprint(string text)
    {
        const ulong offsetBasis = 14695981039346656037;
        const ulong prime = 1099511628211;

        ulong hash = offsetBasis;
        foreach (byte b in Encoding.UTF8.GetBytes(Normalise(text)))
            unchecked
            {
                hash ^= b;
                hash *= prime;
            }

        return hash.ToString("x16", CultureInfo.InvariantCulture);
    }
}
