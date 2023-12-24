namespace AdventOfCode.Days.Tools.Day16;
internal class ContraptionItem(char config, bool energized)
{
    public char Config { get; set; } = config;
    public bool Energized { get; set; } = energized;
}
