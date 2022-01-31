namespace AdventOfCode.Days.Tools.Day18
{
    internal interface ISnailLiteral
    {
        int Magnitude { get; }
        SnailNumber Parent { get; set; }
        int Depth { get; }
    }
}