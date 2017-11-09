using System;
using System.Collections.Generic;

namespace NumberWorder
{
    class Program
    {
        private static Dictionary<char, string> _map = new Dictionary<char, string>()
        {
            ['1'] = "one",
            ['2'] = "two",
            ['3'] = "three",
            ['4'] = "four",
            ['5'] = "five",
            ['6'] = "six",
            ['7'] = "seven",
            ['8'] = "eight",
            ['9'] = "nine",
            ['0'] = "zero"
        };

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Program expects only single numeric parameter to be passed, like \"NumberWorder.exe 123\"");
                Console.ReadKey();
                return;
            }

            var possiblyNumber = args[0];
            foreach (var ch in possiblyNumber)
            {
                Console.Write(
                    _map.ContainsKey(ch)
                        ? _map[ch].ToUpper() // make replacement
                        : $"{ch}"            // Simply put incorrect symbol to output and skip to next symbol
                );
            }

            var correctInput = new System.Text.RegularExpressions.Regex(@"^[0-9]+$").IsMatch(possiblyNumber);
            if (!correctInput)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("There was put incorrect symbols to the input. Please be more careful next time.");
                Console.ReadKey();
            }
        }
    }
}
