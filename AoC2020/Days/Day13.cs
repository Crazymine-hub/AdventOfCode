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

            //get all specified shuttle infos
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

            //Print the header
            Console.CursorLeft = rowWidth;
            foreach (var shuttle in shuttles)
                Console.Write("|" + ConsoleAssist.Center("Bus: " + shuttle.ID, rowWidth));
            Console.WriteLine("|");
            //start the right routine
            if (part2)
                return GetOrdered(shuttles);
            else
                return GetFirstDepart(shuttles, time);
        }

        private string GetOrdered(List<ShuttleBus> shuttles)
        {
            bool orderedDeparture = true;
            long time = 0;
            //our reference shuttle (we need to go in the specified order, hence the constant)
            const int reference = 0;
            List<ShuttleBus> trackedDepart = new List<ShuttleBus>();
            trackedDepart.Add(shuttles[0]);
            long trackDepartCount = 0;
            long stepSize = 1;
            rowWidth = 4;
            //progress over time loop
            do
            {
                orderedDeparture = true;
                Console.Write(time.ToString().PadLeft(rowWidth));
                List<ShuttleBus> departed = new List<ShuttleBus>();
                //check each Shuttle for this step
                for (int i = 0; i < shuttles.Count; i++)
                {
                    ShuttleBus shuttle = shuttles[i];
                    //get the departures and make sure it is not before the required Time (time + offset)
                    shuttle.Departures = time / (long)shuttle.ID;
                    while (time + (long)(shuttle.Offset - shuttles[reference].Offset) > shuttle.Departures * (long)shuttle.ID)
                        ++shuttle.Departures;
                    //Check whether the shuttle departs at this time + offset timestamp (offset is always relative to our reference)
                    if (time + (long)(shuttle.Offset - shuttles[reference].Offset) == shuttle.Departures * (long)shuttle.ID)
                    {
                        Console.Write("|" + ConsoleAssist.Center(shuttle.Offset.ToString(), rowWidth));
                        //Create a list of all shuttles that departed in the right order
                        if (i == departed.Count)
                            departed.Add(shuttle);
                    }
                    else
                    {//the shuttle didn't depart as required
                        Console.Write("|" + ConsoleAssist.Center(".", rowWidth));
                        orderedDeparture = false;
                    }
                }
                //check if we had another shuttle departing
                if (departed.Count > (trackedDepart?.Count ?? 0))
                {
                    if (departed.Except(trackedDepart).Count() == departed.Count - trackedDepart.Count)
                    {//adjust the step size and refresh our found departures
                        stepSize = MathHelper.LeastCommonMultiple(stepSize, trackedDepart.Last().ID);
                        Console.Write("|\t" + stepSize);
                        trackDepartCount = time;
                        trackedDepart = departed;
                    }
                }
                Console.WriteLine("|");

                //Go to the next time where the found shuttles depart together
                time += stepSize;
            } while (!orderedDeparture);

            return "First ordered Departure at " + (time + (shuttles[0].Offset - shuttles[reference].Offset) - stepSize);
        }

        private string GetFirstDepart(List<ShuttleBus> shuttles, long time)
        {
            long firstShuttle = 0;
            long departure = 0;
            List<ShuttleBus> departed = new List<ShuttleBus>();

            //Iterate over time while shuttles need to depart
            do
            {
                Console.Write(time.ToString().PadLeft(rowWidth, time == arrival ? '=' : ' '));
                bool hasDeparture = false;
                //iterate over the shuttles
                for (int i = 0; i < shuttles.Count; i++)
                {
                    ShuttleBus shuttle = shuttles[i];
                    //check if our shuttle currently departs
                    if (time == shuttle.Departures * (long)shuttle.ID)
                    {
                        hasDeparture = true;
                        Console.Write("|" + ConsoleAssist.Center("D", rowWidth));
                        //are we at the bus stop yet?
                        if (time >= arrival)
                        {
                            if (departed.Count == 0)
                            {//this is the first bus, we can catch and it's departure time
                                firstShuttle = (long)shuttle.ID;
                                departure = time;
                            }
                            //track this shuttle as departed
                            departed.Add(shuttle);
                        }
                        ++shuttle.Departures;
                    }
                    else Console.Write("|" + ConsoleAssist.Center(".", rowWidth));
                }
                //if the time has no departure, reuse this line to save vertical screen space
                if (!hasDeparture && time != arrival)
                    Console.CursorLeft = 0;
                else Console.WriteLine("|");
                time++;
                //check if all shuttles are departed
            } while (shuttles.Except(departed).Count() > 0);

            return string.Format("Departing: Shuttle {0} @{1} Result: {2}", firstShuttle, departure, firstShuttle * (departure - arrival));
        }
    }
}
