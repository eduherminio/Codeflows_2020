using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codeflows
{
    public class MultiSet
    {
        public ulong Zeros { get; set; }

        public ulong Ones { get; set; }

        public ulong Twos { get; set; }

        public MultiSet(ulong zeros, ulong ones, ulong twos)
        {
            Zeros = zeros;
            Ones = ones;
            Twos = twos;
        }
    }

    public class Div3
    {
        private List<MultiSet> _sets;

        public void Run()
        {
            ParseInput();

            foreach (var input in _sets)
            {
                ulong totalSubSets = 0;

                totalSubSets += input.Zeros;

                if (input.Ones == input.Twos)
                {
                    totalSubSets += input.Ones;
                }
                else
                {
                    var possibleOneTwoGroups = Math.Min(input.Ones, input.Twos);

                    var onesLeft = input.Ones - possibleOneTwoGroups;
                    var twosLeft = input.Twos - possibleOneTwoGroups;

                    totalSubSets += possibleOneTwoGroups + onesLeft / 3 + twosLeft / 3;
                }

                Console.WriteLine(totalSubSets);
            }
        }

        public void ParseInput()
        {
            _sets = new List<MultiSet>();
            var numberOfCases = int.Parse(Console.ReadLine());

            for (int i = 0; i < numberOfCases; ++i)
            {
                var input = Console.ReadLine().Split(' ');
                _sets.Add(new MultiSet(ulong.Parse(input[0]), ulong.Parse(input[1]), ulong.Parse(input[2])));
            }
        }
    }
}
