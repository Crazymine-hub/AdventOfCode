using AdventOfCode.Days.Tools.Day7;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day7 : DayBase
    {
        public override string Title => "No Space Left On Device";

        private Directory root = new Directory("/", null);
        private Directory cd;
        private List<Directory> knownDirs = new List<Directory>();

        private int diskSize = 70000000;
        private int updateSize = 30000000;

        public override string Solve(string input, bool part2)
        {
            Console.WriteLine("AoC Terminal v1.0");
            foreach (var line in GetLines(input))
                ParseTerminalLine(line);
            Console.WriteLine("A:\\>tree /F");
            RenderTree(root);
            var freeSpace = diskSize - root.Size;

            var candidate = knownDirs
                .Where(x => x.Size + freeSpace >= updateSize)
                .OrderBy(x => x.Size)
                .First();

            return $"Disk space hoggers are {knownDirs.Where(x => x.Size <= 100000).Sum(x => x.Size)} space units \r\n" +
                $"Proposing to delete Directory {candidate.Name} to free up {candidate.Size} units";
        }

        private void RenderTree(IFileSystemEntry fsEntry, string indentation = null, bool isLast = true)
        {
            if (indentation != null)
            {
                Console.Write(indentation);
                Console.Write((isLast ? "└" : "├"));
            }
            Console.WriteLine(fsEntry.ToString());
            var dir = fsEntry as Directory;
            if (dir != null)
            {
                dir.SortChildren();
                for (int i = 0; i < dir.Children.Count; ++i)
                {
                    var newIndentation = indentation ?? "";
                    if(indentation != null)
                        newIndentation +=  isLast ? " " : "│";
                    RenderTree(dir.Children[i], newIndentation, i == dir.Children.Count - 1);
                    Task.Delay(1).Wait();
                }
            }
        }

        private void ParseTerminalLine(string line)
        {
            if (line.StartsWith("$ cd"))
            {
                ChangeDirectory(line.Replace("$ cd ", ""));
                return;
            }
            if (line.StartsWith("$ ls")) return;
            ParseListingEntry(line);
        }

        private void ChangeDirectory(string directoryString)
        {
            if (directoryString == "/")
            {
                cd = root;
                return;
            }
            if (directoryString == "..")
            {
                cd = cd.ParentEntry;
                return;
            }
            var found = (Directory)cd.Children.FirstOrDefault(x => x.Name == directoryString);
            if (found == null)
            {
                found = new Directory(directoryString, cd);
                knownDirs.Add(found);
                cd.Children.Add(found);
            }
            cd = found;
        }

        private void ParseListingEntry(string entryLine)
        {
            var details = entryLine.Split(' ');
            if (details[0] == "dir")
            {
                if (!cd.Children.Any(x => x.Name == details[1]))
                {
                    var newDir = new Directory(details[1], cd);
                    cd.Children.Add(newDir);
                    knownDirs.Add(newDir);
                }
                return;
            }
            cd.Children.Add(new File(details[1], int.Parse(details[0]), cd));
        }
    }
}
