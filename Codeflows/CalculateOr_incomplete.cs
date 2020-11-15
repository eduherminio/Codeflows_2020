using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Codeflows
{
    public class CalculateOr_incomplete
    {
        private List<List<int>> _inputs;
        private IList<int> A;

        private int _orInvocations;
        public void Run()
        {
            ParseInput();

            foreach (var input in _inputs)
            {
                var minOrInvocations = int.MaxValue;
                var permutations = input.Permutations().ToList();
                var candidates = permutations.Where(Foo);

                var eval = candidates.Any() ? candidates : permutations;

                foreach (var permutation in eval)
                {
                    _orInvocations = 0;
                    A = permutation;
                    Calculate_or(1);
                    if (_orInvocations < minOrInvocations)
                    {
                        minOrInvocations = _orInvocations;
                    }
                }

                Console.WriteLine(minOrInvocations);
            }
        }

        private bool Foo(IList<int> arg)
        {
            for (int i = 0; i < arg.Count; ++i)
            {
                if (arg[0] == OR(arg.Skip(i).Reverse()))
                    return true;
            }

            return false;
        }

        private static int OR(IEnumerable<int> enumerable)
        {
            if (!enumerable.Any())
            {
                return -1;
            }
            return enumerable.Aggregate(OR);
        }

        private static int OR(int a, int b) => a | b;

        private int Calculate_or(int i)
        {
            ++_orInvocations;

            var currentIndex = i - 1;   // 0-index arrays

            if (i == A.Count)
            {
                return A[currentIndex];
            }
            if (Calculate_or(i + 1) == A[currentIndex])
            {
                return A[currentIndex];
            }

            return Calculate_or(i + 1) | A[currentIndex];
        }

        public void ParseInput()
        {
            _inputs = new List<List<int>>();
            var numberOfCases = int.Parse(Console.ReadLine());

            for (var _ = 0; _ < numberOfCases; ++_)
            {
                int size = int.Parse(Console.ReadLine());
                var array = Console.ReadLine().Split(' ').Select(int.Parse).ToList();

                if (array.Count != size)
                {
                    throw new Exception();
                }

                _inputs.Add(array);
            }
        }
    }
}
