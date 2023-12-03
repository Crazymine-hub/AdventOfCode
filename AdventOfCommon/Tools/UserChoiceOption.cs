using System;

namespace AdventOfCode.Tools
{
    public record UserChoiceOption<TResultType>(string DisplayText, char DisplayChar, bool IsDefault, TResultType ReturnValue, params ConsoleKey[] ConsoleKeys)
    {
    }
}