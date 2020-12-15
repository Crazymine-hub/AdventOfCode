using AdventOfCode.Days.Tools.Day7;
using AdventOfCode.Tools.TopologicalOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    class Day7 : DayBase
    {
        public override string Title => "Handy Haversacks";
        List<Bag> bags = new List<Bag>();
        Topological<Bag> bagSorter;

        public override string Solve(string input, bool part2)
        {
            //Set up the bags
            LoadBags(input);
            bagSorter = new Topological<Bag>(bags);
            bags = bagSorter.Ordered;
            foreach (Bag bag in bags)
                Console.WriteLine(bag.Name);

            Console.WriteLine("".PadLeft(10, '='));
            Console.WriteLine("RESULT");
            Console.WriteLine("".PadLeft(10, '='));

            Bag myBag = bags.Single(x => x.Name == "shiny gold");
            if (part2)
                return GetRequiredBacks(myBag);
            else
                return GetUsableBags(myBag);
        }

        private string GetRequiredBacks(Bag myBag)
        {
            Dictionary<Bag, int> required;
            Dictionary<Bag, int> newRequired = new Dictionary<Bag, int>();
            newRequired.Add(myBag, 1);
            do
            {//get all bags that are required for my bag to be valid and theirs aswell
                required = newRequired;
                newRequired = new Dictionary<Bag, int>();
                newRequired.Add(myBag, 1);
                foreach (var requiredBag in required)
                {
                    foreach(Bag bag in bags.Where(x => requiredBag.Key.ContainedBags.ContainsKey(x.Name)))
                    {
                        if (!newRequired.ContainsKey(bag))
                            newRequired.Add(bag, 0);
                        newRequired[bag] += requiredBag.Value * requiredBag.Key.ContainedBags[bag.Name];
                    }
                }
            } while (newRequired.Except(required).Count() != 0);

            //sum up all required bags
            long sum = 0;
            foreach (var bag in newRequired)
            {
                Console.WriteLine(bag.Key.Name + " " + bag.Value);
                sum += bag.Value;
            }

            //i already own my bag
            sum -= newRequired[myBag];

            return "Required Bags: " + sum;
        }

        private string GetUsableBags(Bag myBag)
        {
            List<Bag> usable;
            List<Bag> newUsable = new List<Bag>();
            do
            {//Get all bags, that contain Main bag and all bags, that contain such bags
                usable = newUsable;
                newUsable = bags.Where(x => x.ContainedBags.ContainsKey(myBag.Name)).ToList();
                foreach (Bag bag in usable)
                    newUsable.AddRange(bags.Where(x => x.ContainedBags.ContainsKey(bag.Name)));
                newUsable = newUsable.Distinct().ToList();
                //until no new bags are found
            } while (newUsable.Except(usable).Count() != 0);

            foreach (Bag bag in newUsable)
                Console.WriteLine(bag.Name);

            return "Possible Bags: " + newUsable.Count;
        }

        private void LoadBags(string input)
        {
            //Prepare Regex for Bag detection (style color bags(s))
            Regex bagMatch = new Regex(@"(\d )?(\w+ \w+) bag(?:s)?");
            foreach (string bagDescrioption in GetLines(input))
            {
                MatchCollection bagDetails = bagMatch.Matches(bagDescrioption);
                Bag currBag = null;
                for (int i = 0; i < bagDetails.Count; i++)
                {//get the bag if already exists otherwise add it.
                    string bagName = bagDetails[i].Groups[2].Value;
                    int bagAmount = 1;
                    if (bagDetails[i].Groups[1].Success)
                        bagAmount = int.Parse(bagDetails[i].Groups[1].Value);
                    if (bagName == "no other") continue;

                    Bag namedBag = bags.SingleOrDefault(x => x.Name == bagName);
                    if (namedBag == null)
                    {
                        namedBag = new Bag() { Name = bagName };
                        bags.Add(namedBag);
                    }
                    //The first bag is our active Bag, the other are contained bags.
                    if (i == 0)
                        currBag = namedBag;
                    else
                        currBag.ContainedBags.Add(bagName, bagAmount);
                }
            }
        }
    }
}
