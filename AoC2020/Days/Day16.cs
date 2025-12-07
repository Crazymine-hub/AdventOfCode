using AdventOfCode.Days.Tools.Day16;
using AdventOfCode.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    class Day16 : DayBase
    {
        public override string Title => "Ticket Translation";

        Dictionary<string, List<TicketRange>> criterias;
        List<List<string>> possibleNames = new List<List<string>>();
        public override string Solve(string input, bool part2)
        {
            //The input contains criterias, my ticket and other tickets, split and get criterias
            List<string> inputGroups = GetGroupedLines(input);
            criterias = GetCriteria(inputGroups[0]);

            //Split for each ticket and skip headline
            int invalidSum = 0;
            List<string> tickets = GetLines(inputGroups[2]);
            tickets.RemoveAt(0);
            int[] myTicket =
                GetLines(inputGroups[1])[1].
                Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).
                Select(x => int.Parse(x)).ToArray();
            //initialize a list keeping track of all possible names for each position
            foreach (var field in myTicket)
                possibleNames.Add(criterias.Keys.ToList());

            //Analyze the tickets
            ConsoleAssist assis = new ConsoleAssist();
            foreach (string ticket in tickets)
            {
                Console.CursorLeft = 0;
                Console.Write(assis.GetNextProgressChar());
                invalidSum += ValidateTicket(ticket);
            }
            Console.WriteLine();

            //Order the fields by ambiguity (unambiguous -> most ambiguous)
            var orderedFields = possibleNames.OrderBy(x => x.Count).ToList();
            for(int i = 0; i < possibleNames.Count; i++)
            {//get the next unambiguous field(first after all already done)
                var current = orderedFields.First(x => orderedFields.IndexOf(x) >= i && x.Count == 1);
                //Remove it's name from all other field lists
                foreach(var nameList in possibleNames)
                {
                    if (nameList == current) continue;
                    nameList.Remove(current[0]);
                }
            }

            //print my ticket with the right names and multiply all values starting with departure.
            long product = 1;
            for (int i = 0; i < myTicket.Length; i++)
            {
                if (possibleNames[i].Count > 1) return "Ambiguous field name detected!";
                Console.WriteLine(possibleNames[i][0] + ": " + myTicket[i]);
                if (possibleNames[i][0].StartsWith("departure", StringComparison.InvariantCultureIgnoreCase))
                    product *= myTicket[i];
            }

            if (part2)
                return "Ticket Product:" + product;
            else
                return "Invalid Tickets: " + invalidSum;
        }

        private Dictionary<string, List<TicketRange>> GetCriteria(string criteriaInput)
        {
            var result = new Dictionary<string, List<TicketRange>>();
            foreach (Match detail in Regex.Matches(criteriaInput, @"(?<Criteria>[^:\r\n]+)(?<Ranges>[: or]+(?<Lower>\d+)-(?<Upper>\d+))+"))
            {
                List<TicketRange> ranges = new List<TicketRange>();
                //add all allowed ranges for this criteria
                for (int i = 0; i < detail.Groups["Ranges"].Captures.Count; i++)
                {
                    int low = int.Parse(detail.Groups["Lower"].Captures[i].Value);
                    int up = int.Parse(detail.Groups["Upper"].Captures[i].Value);
                    ranges.Add(new TicketRange(low, up));
                }
                //put this criteria with all of it's ranges to the record.
                result.Add(detail.Groups["Criteria"].Value, ranges);
            }
            return result;
        }

        private List<string> GetMatchingCriteria(int value)
        {
            List<string> validCriterias = new List<string>();
            foreach (var crit in criterias)
            {   //Check whether the value matches at least one range of the current criteria.
                foreach (var range in crit.Value)
                    if (range.ValueInRange(value))
                    {
                        validCriterias.Add(crit.Key);
                        break;
                    }
            }
            return validCriterias;
        }

        private int ValidateTicket(string ticket)
        {
            //extract all fields and iterate over them.
            int[] fields = ticket.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
            int invalidSum = 0;
            for (int i = 0; i < fields.Length; i++)
            {
                var matches = GetMatchingCriteria(fields[i]);
                //if the field is invalid, add it's value to the invalidSum.
                //otherwise remove all names from the possible names, that this field value doesn't match (keep names, that were found)
                if (matches.Count == 0)
                    invalidSum += fields[i];
                else
                    possibleNames[i] = possibleNames[i].Where(x => matches.Contains(x)).ToList();
            }
            return invalidSum;
        }
    }
}
