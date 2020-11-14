using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codeflows
{
    public class PalindromeAntipalindrome
    {
        private const char Zero = '0';
        private const char One = '1';

        private List<string> _inputs;

        private string Flip(string str)
        {
            var sb = new StringBuilder();
            foreach (var ch in str)
            {
                sb.Append(ch == Zero ? One : Zero);
            }
            return sb.ToString();
        }

        private bool IsPalindrome(string str) => str.Equals(string.Join("", str.Reverse())) || str?.Length == 0;
        private bool IsAntiPalindrome(string str) => str.Equals(string.Join("", Flip(str).Reverse())) || str?.Length == 0;

        public void Run()
        {
            ParseInput();

            foreach (var input in _inputs)
            {
                string palindromeStr = "";
                var palindrome = new HashSet<int>();

                string antiPalindromeStr = "";
                var antiPalindrome = new HashSet<int>();

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
                        palindromeStr = palindromeStr.Insert(palindromeStr.Length / 2, $"{ch1}");
                        break;
                    }
                    if (ch1 == ch2)
                    {
                        palindrome.Add(i + 1);
                        palindrome.Add(antiI + 1);
                        palindromeStr = palindromeStr.Insert(palindromeStr.Length / 2, $"{ch1}{ch2}");
                    }
                    else
                    {
                        antiPalindrome.Add(i + 1);
                        antiPalindrome.Add(antiI + 1);
                        antiPalindromeStr = antiPalindromeStr.Insert(antiPalindromeStr.Length / 2, $"{ch1}{ch2}");
                    }
                }

                if (!IsPalindrome(palindromeStr) || !IsAntiPalindrome(antiPalindromeStr))
                {
                    throw new Exception();
                }

                Console.WriteLine($"{palindrome.Count} {antiPalindrome.Count}");
                Console.WriteLine(string.Join(" ", palindrome.OrderBy(index => index)));
                Console.WriteLine(string.Join(" ", antiPalindrome.OrderBy(index => index)));
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
