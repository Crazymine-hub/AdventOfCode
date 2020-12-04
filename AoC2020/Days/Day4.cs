using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day4 : DayBase
    {
        public override string Title => "Passport Processing";

        public override string Solve(string input, bool part2)
        {
            Dictionary<string, Func<string, bool>> requiredFields = new Dictionary<string, Func<string, bool>>() 
            {
                {"byr", ValidateBirthYear},
                {"iyr", ValidateIssueYear},
                {"eyr", ValidateExpireYear},
                {"hgt", ValidateHeight},
                {"hcl", ValidateHairColor},
                {"ecl", ValidateEyeColor},
                {"pid", ValidatePassNumber}
            };

            string[] passports = input.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int validPassports = 0;
            foreach (string passport in passports)
            {
                var passFields = Regex.Matches(passport, @"(\w{3}):(.*?)(?:\s|$)").Cast<Match>()
                    .Select(x => new Tuple<string, string>(x.Groups[1].Value, x.Groups[2].Value));
                Console.WriteLine("=========NEXT PASSPORT===========");
                validPassports++;
                foreach (var field in requiredFields)
                {
                    var passEntry = passFields.SingleOrDefault(x => x.Item1 == field.Key);
                    if (passEntry != null)
                    {
                        Console.Write($"FIELD: {passEntry.Item1} -> {passEntry.Item2}");
                        if (part2 && !field.Value(passEntry.Item2))
                        {
                            validPassports--;
                            Console.WriteLine(" INVALID VALUE");
                            break;
                        }
                        Console.WriteLine(" OK");
                    }
                    else
                    {
                        Console.WriteLine("INVALID: Missing required field " + field.Key);
                        validPassports--;
                        break;
                    }
                }
            }

            return "Valid Passports: " + validPassports;
        }

        private bool ValidateBirthYear(string value)
        {
            if (int.TryParse(value, out int year))
                return 1920 <= year && year <= 2002;
            return false;
        }

        private bool ValidateIssueYear(string value)
        {
            if (int.TryParse(value, out int year))
                return 2010 <= year && year <= 2020;
            return false;
        }

        private bool ValidateExpireYear(string value)
        {
            if (int.TryParse(value, out int year))
                return 2020 <= year && year <= 2030;
            return false;
        }

        private bool ValidateHeight(string value)
        {
            var heightInf = Regex.Match(value, @"(\d+)(in|cm)");
            if (heightInf.Success && int.TryParse(heightInf.Groups[1].Value, out int height))
            {
                if(heightInf.Groups[2].Value == "cm")
                    return 150 <= height && height <= 193;
                else
                    return 59 <= height && height <= 76;
            }
            return false;
        }

        private bool ValidateHairColor(string value)
        {
            return Regex.Match(value, @"^#[0-9a-z]{6}$").Success;
        }

        private bool ValidateEyeColor(string value)
        {
            List<string> validEyeColors = new List<string>()
            {"amb", "blu", "brn", "gry", "grn", "hzl", "oth",};
            return validEyeColors.Contains(value);
        }

        private bool ValidatePassNumber(string value)
        {
            return Regex.IsMatch(value, @"^\d{9}$");
        }
    }
}
