using AdventOfCode.Tools;

namespace AdventOfCode.Days.Tools.Day16;

internal record struct LightBeam(int X, int Y, TraceChars.Direction Direction)
{
    public static implicit operator (int x, int y, TraceChars.Direction)(LightBeam value) => (value.X, value.Y, value.Direction);
    public static implicit operator LightBeam((int x, int y, TraceChars.Direction direction) value) => new LightBeam(value.x, value.y, value.direction);
}