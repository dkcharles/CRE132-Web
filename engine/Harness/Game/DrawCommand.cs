namespace CRE132.Game;

public enum DrawKind { Clear, Rect, Circle, Line, Text }

// One Screen call, recorded. Field meaning by Kind: Rect x,y,w,h; Circle x,y,r,-; Line x1,y1,x2,y2;
// Text x,y,-,- + Text; Clear uses only Colour. A flat record rather than a hierarchy so it
// crosses JS interop as nine primitives per command.
public sealed record DrawCommand(DrawKind Kind, double A, double B, double C, double D, Colour Colour, string Text = "");
