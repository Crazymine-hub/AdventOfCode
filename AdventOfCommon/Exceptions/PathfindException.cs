using System;

namespace AdventOfCode.Exceptions;

[Serializable]
internal class PathfindException : Exception
{
    public PathfindException()
    {
    }

    public PathfindException(string? message) : base(message)
    {
    }

    public PathfindException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}