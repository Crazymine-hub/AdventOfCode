using AdventOfCode.Days.Tools.Day18;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    class Day18 : DayBase
    {
        public override string Title => "Operation Order";


        bool invertedOrder = false;
        public override string Solve(string input, bool part2)
        {
            invertedOrder = part2;
            input = input.Replace(" ", "");
            long mathsum = 0;
            foreach (var equation in GetLines(input))
                mathsum = checked(mathsum + CalcEquation(equation));
            return "Total sum of all results:" + mathsum;
        }

        private long CalcEquation(string equation)
        {
            Console.Write(equation + " = ");
            int depth = -1;
            List<EquationInfo> results = new List<EquationInfo>();
            int bracketLevel = 0;

            //Manage increasing of the tracked depth
            void IncreaseDepth()
            {
                ++depth;
                //check if we need to add an element to the array.
                if (depth >= results.Count)
                    results.Add(null);
                //For the second part we need to default to a multiplication to avoid issues.
                if (invertedOrder)
                    results[depth] = new EquationInfo(1, '*');
                else
                    results[depth] = new EquationInfo();
                results[depth].BracketLevel = bracketLevel;
            }

            //for depth analysis, we need to add a second working layer before
            IncreaseDepth();
            if (invertedOrder) IncreaseDepth();
            long currentNumber = 0;
            foreach (char eqChar in equation)
            {
                if (long.TryParse(eqChar.ToString(), out long digit))
                {   //add a digit to the current number
                    currentNumber *= 10;
                    currentNumber += digit;
                }
                else
                {
                    if (eqChar == '+' || eqChar == '*')
                    {//we got an operator. apply the current number to the last and set the new operator to this
                        DoOperation(results[depth], currentNumber);
                        int currDepth = depth;
                        if (invertedOrder && eqChar == '*')
                        {//part 2 only (operator priority)
                            //when we got multiplication, make sure to increase the working level for eventual addition
                            //if we already have multiplication on this level, keep it and apply the previus operand
                            //(addition result) to the previous multiplication
                            if (results[depth - 1].CurrOperator == '*' && results[depth - 1].BracketLevel == bracketLevel)
                                DoOperation(results[depth - 1], results[depth--].Value);
                            IncreaseDepth();
                        }
                        results[currDepth].CurrOperator = eqChar;
                    }
                    else if (eqChar == '(')
                    {
                        ++bracketLevel;
                        IncreaseDepth();
                    }
                    else if (eqChar == ')')
                    {
                        DoOperation(results[depth], currentNumber);
                        //we increased the level for possible additions in a multiplication in this bracket level.
                        //apply this bracket's result to the equation before it
                        if (invertedOrder && results[depth - 1].CurrOperator == '*' && results[depth-1].BracketLevel == bracketLevel)
                            DoOperation(results[depth - 1], results[depth--].Value);
                        currentNumber = results[depth--].Value;
                        bracketLevel--;
                        continue;
                    }
                    currentNumber = 0;
                }
            }
            //apply the last stored number to the main equation, because it hasn't been applied yet
            DoOperation(results[depth], currentNumber);
            //make sure, we got to the lowest level (unclosed brackets... shouldn't happen but well)
            while (depth > 0)
                DoOperation(results[depth - 1], results[depth--].Value);
            Console.WriteLine(results[0].Value);
            return results[0].Value;
        }

        private void DoOperation(EquationInfo equationInfo, long currentNumber)
        {
            switch (equationInfo.CurrOperator)
            {
                case '+':
                    equationInfo.Value += currentNumber;
                    break;
                case '*':
                    equationInfo.Value *= currentNumber;
                    break;
                default: throw new InvalidOperationException("unknown operator: " + equationInfo.CurrOperator);
            }
        }
    }
}
