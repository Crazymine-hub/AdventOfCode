using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using AdventOfCode.Days.Classes.Day10;
using AdventOfCode.Tools;
using System.Threading;

namespace AdventOfCode.Days
{
    /// <summary>
    /// AsteroidMap
    /// </summary>
    class Day10 : IDay
    {
        string[] map;
        List<AsteroidInfo> Asteroids = new List<AsteroidInfo>();
        AsteroidInfo StationInfo;
        public string Solve(string input, bool part2)
        {

            /*input = ".#..##.###...#######\r\n" +
                "##.############..##.\r\n" +
                ".#.######.########.#\r\n" +
                ".###.#######.####.#.\r\n" +
                "#####.##.#.##.###.##\r\n" +
                "..#####..#.#########\r\n" +
                "####################\r\n" +
                "#.####....###.#.#.##\r\n" +
                "##.#################\r\n" +
                "#####.##.###..####..\r\n" +
                "..######..##.#######\r\n" +
                "####.##.####...##..#\r\n" +
                ".#####..#.######.###\r\n" +
                "##...#.##########...\r\n" +
                "#.##########.#######\r\n" +
                ".####.#.###.###.#.##\r\n" +
                "....##.##.###..#####\r\n" +
                ".#.#.###########.###\r\n" +
                "#.#.#.#####.####.###\r\n" +
                "###.##.####.##.#..##";*/

            /*input = ".#....#####...#..\r\n" +
                "##...##.#####..##\r\n" +
                "##...#...#.#####.\r\n" +
                "..#.....#...###..\r\n" +
                "..#.#.....#....##";*/

            map = input.Split(new string[] { "\n" }, StringSplitOptions.None);

            for (int y = 0; y < map.Length; y++)
            {
                string line = map[y].Replace("\r", "");
                for (int x = 0; x < line.Length; x++)
                    if (line[x] == '#')
                        Asteroids.Add(new AsteroidInfo(new Point(x, y), new Point(), 0, 0));
            }
            StationInfo = GetBestStation();

            if (part2)
            {
                GetStationDetection(StationInfo);

                Dictionary<Point, List<AsteroidInfo>> AsteroidGroups = new Dictionary<Point, List<AsteroidInfo>>();
                //Gruppieren
                foreach (AsteroidInfo asteroid in Asteroids)
                {
                    if (!AsteroidGroups.ContainsKey(asteroid.Direction))
                        AsteroidGroups.Add(asteroid.Direction, new List<AsteroidInfo>());
                    AsteroidGroups[asteroid.Direction].Add(asteroid);
                }

                //Gruppen Sortieren
                var groups = AsteroidGroups.OrderBy(x => GetAsteroidAngle(x));
                var orderedGroups = new Dictionary<Point, List<AsteroidInfo>>();
                foreach (var asteroidGroup in groups)
                {
                    var group = asteroidGroup.Value.OrderBy(x => x.Distance).ToList();
                    asteroidGroup.Value.Clear();
                    //Gruppeninhalt sortieren
                    foreach (AsteroidInfo asteroid in group)
                        asteroidGroup.Value.Add(asteroid);
                    orderedGroups.Add(asteroidGroup.Key, asteroidGroup.Value);
                }

                Asteroids.Clear();
                int pos = 0;
                while (orderedGroups.Count != 0)
                {
                    if (pos >= orderedGroups.Count)
                        pos = 0;
                    Point currKey = orderedGroups.ElementAt(pos).Key;

                    Asteroids.Add(orderedGroups[currKey][0]);
                    orderedGroups[currKey].RemoveAt(0);

                    if (orderedGroups[currKey].Count == 0)
                    {
                        orderedGroups.Remove(currKey);
                        pos--;
                    }
                    pos++;
                }

                Console.SetCursorPosition(0, 1);
                Console.Write(input.Replace(".", " "));
                for(int i = 0; i < Asteroids.Count; i++)
                {
                    Console.SetCursorPosition(Asteroids[i].Position.X, Asteroids[i].Position.Y+1);
                    Console.Write(i == 0 ? 'X': ' ');
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("".PadLeft(Console.BufferWidth));
                    Console.SetCursorPosition(0, 0);
                    Console.Write(string.Format("i = {0} Asteroid: {1}", i , Asteroids[i].ToString()));
                    Thread.Sleep(50);
                }

                Console.Clear();
                Point location = Asteroids[200].Position;
                return $"Vaporizing Asteroid#200 @{location.X}|{location.Y} CHECK: {location.X * 100 + location.Y}";
            }
            return $"Maximale Asteroiden gefunden: {StationInfo.Detections} @{StationInfo.Position.X}|{StationInfo.Position.Y}";
        }

        private AsteroidInfo GetBestStation()
        {
            AsteroidInfo bestDetection = new AsteroidInfo(new Point(), new Point(), 0, 0);

            foreach (AsteroidInfo asteroid in Asteroids)
            {
                GetStationDetection(asteroid);
                if (asteroid.Detections > bestDetection.Detections)
                    bestDetection = asteroid;
            }
            return bestDetection;
        }

        private void GetStationDetection(AsteroidInfo origin)
        {
            List<Point> detects = new List<Point>();
            foreach (AsteroidInfo asteroid in Asteroids)
            {
                //if (asteroid.Position == origin.Position) continue;
                asteroid.Direction = new Point(asteroid.Position.X - origin.Position.X, asteroid.Position.Y - origin.Position.Y);
                asteroid.Distance = Tools.VectorAssist.GetLength(asteroid.Direction);
                if (asteroid.Distance == 0)
                    asteroid.Direction = new Point();
                else
                    asteroid.Direction = Tools.VectorAssist.Minimize(asteroid.Direction);

                if (!detects.Contains(asteroid.Direction))
                    detects.Add(asteroid.Direction);
            }
            origin.Detections = detects.Count;
        }

        private double GetAsteroidAngle(KeyValuePair<Point, List<AsteroidInfo>> asteroidGroup)
        {
            double angle = VectorAssist.GetAngleBetween(asteroidGroup.Key, new Point(0, -1));
            if (asteroidGroup.Key.X < 0)
                angle = 360 - angle;
            return angle;
        }
    }
}
