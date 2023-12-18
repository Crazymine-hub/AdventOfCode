using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.Extensions;
public static class StringExtensions
{
    public static List<string> GetLines(this string input)
    {
        return input.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public static List<string> GetGroupedLines(this string input)
    {
        return input.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
