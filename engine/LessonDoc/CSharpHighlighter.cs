using System.Text;

namespace CRE132.LessonDoc;

// Colours a C# listing at BUILD time. The browser ships no highlighter, for the same reason
// it ships no markdown parser: the read path is what every visitor pays for, and a listing
// a student only reads does not justify a syntax engine in the payload.
//
// One left-to-right scanner, no parser, no semantic analysis. Comments and strings win over
// everything - a // inside a string is not a comment - then numbers, then whole words checked
// against a fixed keyword list. Type names are deliberately NOT coloured: four colours is a
// textbook, seven is an IDE, and this is the quiet half of the page.
//
// Everything is HTML-encoded, including quotes, because the result is handed to the page as a
// MarkupString. A listing that got that wrong would be live markup rather than code.
public static class CSharpHighlighter
{
    // Only what this course actually writes. Deliberately short: an unknown word rendering as
    // plain text is invisible, while a mis-coloured one is a lie a student cannot check.
    // `get`/`set` are highlighted wherever they appear - in this course they are only ever
    // accessors, and the test for "is it followed by ; or {" costs more than it buys.
    static readonly HashSet<string> Keywords = new(StringComparer.Ordinal)
    {
        "var", "new", "true", "false", "null",
        "public", "private", "protected", "static", "void",
        "int", "float", "double", "bool", "string", "char",
        "class", "interface", "abstract", "virtual", "override",
        "namespace", "using",
        "if", "else", "for", "foreach", "while", "do", "switch", "case",
        "break", "continue", "return",
        "this", "base", "readonly", "const", "enum",
        "in", "out", "ref", "is", "as", "get", "set",
    };

    public static string Highlight(string source)
    {
        var sb = new StringBuilder(source.Length + source.Length / 3);
        int i = 0;
        int plain = 0;   // start of the run of untokenised text still to be flushed

        void Flush(int end)
        {
            if (end > plain) Encode(sb, source, plain, end);
        }

        void Token(string cls, int start, int end)
        {
            Flush(start);
            sb.Append("<span class=\"").Append(cls).Append("\">");
            Encode(sb, source, start, end);
            sb.Append("</span>");
            plain = end;
        }

        while (i < source.Length)
        {
            char c = source[i];
            int start = i;

            // Comments first: inside one, nothing else is code.
            if (c == '/' && Next(source, i) == '/')
            {
                int end = source.IndexOf('\n', i);
                if (end < 0) end = source.Length;
                Token("hl-c", start, end);
                i = end;
                continue;
            }

            if (c == '/' && Next(source, i) == '*')
            {
                int close = source.IndexOf("*/", i + 2, StringComparison.Ordinal);
                int end = close < 0 ? source.Length : close + 2;
                Token("hl-c", start, end);
                i = end;
                continue;
            }

            // Verbatim, interpolated-verbatim and plain/interpolated strings. The whole
            // literal is one token; the holes of a $"..." are not re-highlighted, because
            // finding their boundaries needs a real parser.
            int prefix = StringPrefix(source, i);
            if (prefix > 0)
            {
                // @"..", $@".." and @$".." all follow verbatim rules; only $".." does not.
                bool verbatim = source[i] == '@' || prefix == 3;
                int end = verbatim
                    ? EndOfVerbatim(source, i + prefix)
                    : EndOfQuoted(source, i + prefix, '"', interpolated: source[i] == '$');
                Token("hl-s", start, end);
                i = end;
                continue;
            }

            if (c == '\'')
            {
                int end = EndOfQuoted(source, i + 1, '\'');
                Token("hl-s", start, end);
                i = end;
                continue;
            }

            // A digit here always begins a number: the word scanner below swallows any
            // digits that trail an identifier, so `speed2` never reaches this point as `2`.
            if (char.IsAsciiDigit(c))
            {
                int end = EndOfNumber(source, i);
                Token("hl-n", start, end);
                i = end;
                continue;
            }

            if (IsWordStart(c))
            {
                int end = i;
                while (end < source.Length && IsWordChar(source[end])) end++;
                if (Keywords.Contains(source[i..end])) Token("hl-k", start, end);
                i = end;
                continue;
            }

            i++;
        }

        Flush(source.Length);
        return sb.ToString();
    }

    static char Next(string s, int i) => i + 1 < s.Length ? s[i + 1] : '\0';

    // The length of a string literal's opening punctuation, or 0 if one does not start here.
    static int StringPrefix(string s, int i) => s[i] switch
    {
        '"' => 1,
        '$' when Next(s, i) == '"' => 2,
        '@' when Next(s, i) == '"' => 2,
        '$' when Next(s, i) == '@' && i + 2 < s.Length && s[i + 2] == '"' => 3,
        '@' when Next(s, i) == '$' && i + 2 < s.Length && s[i + 2] == '"' => 3,
        _ => 0,
    };

    // Backslash escapes; an unterminated literal ends at the newline rather than eating the
    // rest of the file, so one typo in a listing cannot grey out everything below it.
    //
    // An interpolated literal counts brace depth, because a hole may hold a string of its own:
    // $"carrying {string.Join(", ", inventory)}" is ONE token, and ending it at the inner quote
    // split the line in two and mis-coloured its tail - which is what the Lesson 9 page shipped.
    // The holes are still not re-highlighted; the depth counter only decides where the literal
    // ends. {{ and }} are literal braces and must not move the depth.
    static int EndOfQuoted(string s, int i, char quote, bool interpolated = false)
    {
        int depth = 0;
        while (i < s.Length && s[i] != '\n')
        {
            if (s[i] == '\\') { i += 2; continue; }

            if (interpolated)
            {
                if ((s[i] == '{' || s[i] == '}') && Next(s, i) == s[i]) { i += 2; continue; }
                if (s[i] == '{') { depth++; i++; continue; }
                // Never below zero: a stray '}' in a badly typed listing must not make the
                // closing quote look like it is inside a hole.
                if (s[i] == '}') { if (depth > 0) depth--; i++; continue; }
            }

            if (s[i] == quote && depth == 0) return i + 1;
            i++;
        }
        return Math.Min(i, s.Length);
    }

    // No escapes; "" is one embedded quote. May span lines, which is the point of it.
    static int EndOfVerbatim(string s, int i)
    {
        while (i < s.Length)
        {
            if (s[i] == '"')
            {
                if (i + 1 < s.Length && s[i + 1] == '"') { i += 2; continue; }
                return i + 1;
            }
            i++;
        }
        return s.Length;
    }

    // Digits, one embedded dot that is followed by a digit (so `1.ToString()` keeps its dot),
    // hex, digit separators, an exponent, and the f/d/m/u/l suffixes.
    static int EndOfNumber(string s, int i)
    {
        if (s[i] == '0' && (Next(s, i) is 'x' or 'X')) i += 2;

        while (i < s.Length)
        {
            char c = s[i];
            if (char.IsAsciiLetterOrDigit(c) || c == '_') { i++; continue; }
            if (c == '.' && i + 1 < s.Length && char.IsAsciiDigit(s[i + 1])) { i++; continue; }
            if ((c == '+' || c == '-') && i > 0 && (s[i - 1] is 'e' or 'E')) { i++; continue; }
            break;
        }
        return i;
    }

    static bool IsWordStart(char c) => char.IsAsciiLetter(c) || c == '_';
    static bool IsWordChar(char c) => char.IsAsciiLetterOrDigit(c) || c == '_';

    static void Encode(StringBuilder sb, string s, int start, int end)
    {
        for (int i = start; i < end; i++)
        {
            switch (s[i])
            {
                case '&': sb.Append("&amp;"); break;
                case '<': sb.Append("&lt;"); break;
                case '>': sb.Append("&gt;"); break;
                case '"': sb.Append("&quot;"); break;
                default: sb.Append(s[i]); break;
            }
        }
    }
}
