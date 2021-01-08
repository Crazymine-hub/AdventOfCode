using AdventOfCode.Days.Tools.Day13;
using AdventOfCode.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day13 : DayBase
    {
        public override string Title => "Shuttle Search";
        int rowWidth;
        long arrival;


        public override string Solve(string input, bool part2)
        {
            var plan = GetLines(input);
            arrival = long.Parse(plan[0]);
            long time = arrival;
            rowWidth = arrival.ToString().Length + 2;

            List<ShuttleBus> shuttles = new List<ShuttleBus>();
            string[] shuttleIDs = plan[1].Split(',');
            for (int i = 0; i < shuttleIDs.Length; i++)
            {
                if (shuttleIDs[i] == "x") continue;
                int id = int.Parse(shuttleIDs[i]);
                long trips = arrival / (long)id;
                shuttles.Add(new ShuttleBus(id, i) { Departures = part2 ? 0 : trips });
                long lastStopTime = trips * (long)id;
                if (lastStopTime < time)
                    time = lastStopTime;
                if (id.ToString().Length + 5 > rowWidth) rowWidth = id.ToString().Length + 7;
            }


            Console.CursorLeft = rowWidth;
            foreach (var shuttle in shuttles)
                Console.Write("|" + ConsoleAssist.Center("Bus: " + shuttle.ID, rowWidth));
            Console.WriteLine("|");
            if (part2)
                return GetOrdered(shuttles);
            else
                return GetFirstDepart(shuttles, time);
        }

        private string GetOrdered(List<ShuttleBus> shuttles)
        {
            bool orderedDeparture = true;
            long startTime = 0;
            int reference = shuttles.Max(x => x.ID);
            reference = shuttles.IndexOf(shuttles.Single(x => x.ID == reference));
            reference = 0;
            List<ShuttleBus> trackedDepart = new List<ShuttleBus>();
            trackedDepart.Add(shuttles[0]);
            long trackDepartCount = 0;
            long stepSize = 1;
            rowWidth = 4;
            do
            {
                orderedDeparture = true;
                Console.Write(startTime.ToString().PadLeft(rowWidth));
                List<ShuttleBus> departed = new List<ShuttleBus>();
                for (int i = 0; i < shuttles.Count; i++)
                {
                    ShuttleBus shuttle = shuttles[i];
                    shuttle.Departures = startTime / (long)shuttle.ID;
                    //if (i <= reference) --shuttle.Departures;
                    while (startTime + (long)(shuttle.Offset - shuttles[reference].Offset) > shuttle.Departures * (long)shuttle.ID)
                        ++shuttle.Departures;
                    if (startTime + (long)(shuttle.Offset - shuttles[reference].Offset) == shuttle.Departures * (long)shuttle.ID)
                    {
                        Console.Write("|" + ConsoleAssist.Center(shuttle.Offset.ToString(), rowWidth));
                        if (i == departed.Count)
                            departed.Add(shuttle);
                    }
                    else
                    {
                        Console.Write("|" + ConsoleAssist.Center(".", rowWidth));
                        orderedDeparture = false;
                    }
                }
                if (departed.Count >= (trackedDepart?.Count ?? 0))
                {
                    if (departed.Except(trackedDepart).Count() == departed.Count - trackedDepart.Count)
                    {
                        stepSize = MathHelper.LeastCommonMultiple(stepSize, trackedDepart.Last().ID);
                        Console.Write("|\t" + stepSize);
                        trackDepartCount = startTime;
                        trackedDepart = departed;
                    }
                }
                Console.WriteLine("|");

                startTime += stepSize;
            } while (!orderedDeparture);

            return "First ordered Departure at " + (startTime + (shuttles[0].Offset - shuttles[reference].Offset) - stepSize);
        }

        private string GetFirstDepart(List<ShuttleBus> shuttles, long time)
        {
            long firstShuttle = 0;
            long departure = 0;
            List<ShuttleBus> departed = new List<ShuttleBus>();

            do
            {
                Console.Write(time.ToString().PadLeft(rowWidth, time == arrival ? '=' : ' '));
                bool hasDeparture = false;
                for (int i = 0; i < shuttles.Count; i++)
                {
                    ShuttleBus shuttle = shuttles[i];
                    if (time == shuttle.Departures * (long)shuttle.ID)
                    {
                        hasDeparture = true;
                        Console.Write("|" + ConsoleAssist.Center("D", rowWidth));
                        if (time >= arrival)
                        {
                            if (departed.Count == 0)
                            {
                                firstShuttle = (long)shuttle.ID;
                                departure = time;
                            }
                            departed.Add(shuttle);
                        }
                        ++shuttle.Departures;
                    }
                    else Console.Write("|" + ConsoleAssist.Center(".", rowWidth));
                }
                if (!hasDeparture && time != arrival)
                    Console.CursorLeft = 0;
                else Console.WriteLine("|");
                time++;
            } while (shuttles.Except(departed).Count() > 0);

            return string.Format("Departing: Shuttle {0} @{1} Result: {2}", firstShuttle, departure, firstShuttle * (departure - arrival));
        }
    }
}
