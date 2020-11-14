using System;
using System.Collections.Generic;
using System.Linq;

namespace Codeflows
{
    public class PalindromeAntipalindrome
    {
        private List<string> _inputs;

        public void Run()
        {
            ParseInput();

            foreach (var input in _inputs)
            {
                var palindrome = new HashSet<int>();

                var limit = input.Length % 2 == 0
                    ? input.Length / 2
                    : (input.Length / 2) + 1;

                for (int i = 0; i < limit; ++i)
                {
                    var antiI = input.Length - 1 - i;
                    var ch1 = input[i];
                    var ch2 = input[antiI];

                    if (i == antiI)
                    {
                        palindrome.Add(i + 1);
                        break;
                    }

                    if (ch1 == ch2)
                    {
                        palindrome.Add(i + 1);
                        palindrome.Add(antiI + 1);
                    }
                }

                Console.WriteLine($"{palindrome.Count} {input.Length - palindrome.Count}");
                Console.WriteLine(string.Join(" ", palindrome.OrderBy(index => index)));
                Console.WriteLine(string.Join(" ", Enumerable.Range(1, input.Length).Except(palindrome)));
            }
        }

        public void ParseInput()
        {
            _inputs = new List<string>();

            var numberOfCases = int.Parse(Console.ReadLine());
            for (int i = 0; i < numberOfCases; ++i)
            {
                _inputs.Add(Console.ReadLine());
            }
        }
    }
}
