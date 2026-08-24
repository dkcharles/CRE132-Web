using System.Text;

namespace CRE132.Engine;

// Captures a program's console output, up to a cap. The cap exists for the same reason the
// step budget does: an endless printing loop on a single-threaded runtime has to be stopped
// from inside, and the writer is the only code guaranteed to run on every print.
public sealed class BoundedWriter : TextWriter
{
    readonly StringBuilder text = new();
    readonly int limit;

    public BoundedWriter(int limit) => this.limit = limit;

    public override Encoding Encoding => Encoding.UTF8;

    public string Text => text.ToString();

    public override void Write(char value)
    {
        Guard(1);
        text.Append(value);
    }

    public override void Write(string? value)
    {
        if (string.IsNullOrEmpty(value)) return;
        Guard(value.Length);
        text.Append(value);
    }

    void Guard(int incoming)
    {
        if (text.Length + incoming > limit) throw new OutputLimitException();
    }
}
