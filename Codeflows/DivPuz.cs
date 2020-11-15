using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Codeflows
{
    public class DivPuz
    {
        private List<List<ulong>> _inputs;

        public void Run()
        {
            ParseInput();

            foreach (var input in _inputs)
            {
                var solution = new Dictionary<int, ulong>(input.Count);

                for (int i = 0; i < input.Count; ++i)
                {
                    int previous = i > 0 ? i - 1 : input.Count - 1;
                    int next = i < input.Count - 1 ? i + 1 : 0;

                    var lcm = LeastCommonMultiple(
                        input[i],
                        input[previous],
                        input[next]
                        );

                    solution[i] = lcm;
                    //if (lcm < 1e18)
                    //{
                    //    solution[i] = lcm;
                    //}
                    //else
                    //{
                    //    Thread.Sleep(5);
                    //}
                }

                for (int i = 0; i < solution.Count; ++i)
                {
                    if (solution[i] > 1e18)
                    {
                        // TODOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
                        Thread.Sleep(5);
                        //int previous = i > 0 ? i - 1 : input.Count - 1;
                        //int next = i < input.Count - 1 ? i + 1 : 0;
                        //int preprevious = i > 1 ? i - 2 : input.Count - 1 - (1 - i);
                        //int nextnext = i < input.Count - 2 ? i + 2 : 0 + (input.Count - 1 - i);

                        //var gcd = GreatestCommonDivisor(solution[previous] * solution[next], input[i]);

                        //var candidates = solution[i] / input[i];

                        //foreach (var candidate in GetFactors(candidates).Except(new[] { (ulong)1 }))
                        //{
                        //    //if (possibleSolution % input[i] == 0
                        //    //     && solution[previous] % candidate == 0 && solution[previous] / candidate % input[previous] == 0
                        //    //     && solution[next] % candidate == 0 && solution[next] / candidate % input[next] == 0)v
                        //    var possibleSolution = solution[i] / candidate;


                        //    if (possibleSolution / input[i] >= 1
                        //    && possibleSolution % input[i] == 0
                        //     && (solution[previous] * solution[next]) / (possibleSolution) >= 1
                        //     && (solution[previous] * solution[next]) % (possibleSolution) == 0
                        //     && (solution[preprevious] * (possibleSolution)) / solution[previous] >= 1
                        //     && (solution[preprevious] * (possibleSolution)) % solution[previous] == 0
                        //     && (solution[nextnext] * (possibleSolution)) / solution[next] >= 1
                        //     && (solution[nextnext] * (possibleSolution)) % solution[next] == 0)
                        //    //&& (solution[previous] / candidates) % input[previous] == 0
                        //    //&& (solution[next] / candidates) % input[next] == 0)
                        //    {
                        //        solution[i] = possibleSolution;
                        //        //    solution[previous] /= gcd;
                        //        //    solution[next] /= gcd;
                        //    }
                        //}

                    }
                    //var newSol = LeastCommonMultiple(input[i], solution[previous] / input[i] / input[preprevious], solution[next] / input[i] / input[nextnext]);
                }

                Console.WriteLine(string.Join(" ", solution.OrderBy(p => p.Key).Select(p => p.Value)));
            }
        }

        public static IEnumerable<ulong> GetFactors(ulong x)
        {
            for (ulong i = 1; i * i <= x; i++)
            {
                if ((x % i) == 0)
                {
                    yield return i;
                    if (i != (x / i))
                    {
                        yield return x / i;
                    }
                }
            }
        }

        private static ulong LeastCommonMultiple(params ulong[] numbers)
        {
            return LeastCommonMultiple(numbers.Where(n => n != 0));
        }

        private static ulong LeastCommonMultiple(IEnumerable<ulong> enumerable)
        {
            return enumerable.Aggregate(LeastCommonMultiple);
        }

        /// <summary>
        /// Dividing first to avoid overflows
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static ulong LeastCommonMultiple(ulong a, ulong b)
        {
            return a / GreatestCommonDivisor(a, b) * b;
        }

        /// <summary>
        /// Euclidean Algorithm
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static ulong GreatestCommonDivisor(ulong a, ulong b)
        {
            while (b != 0)
            {
                var r = a % b;
                a = b;
                b = r;
            }

            return a;
        }

        //private static ulong GreatestCommonDivisor(params ulong[] array)
        //{
        //    return GreatestCommonDivisor(array.AsEnumerable());
        //}

        //private static ulong GreatestCommonDivisor(IEnumerable<ulong> enumerable)
        //{
        //    var result = enumerable.ElementAt(0);
        //    for (int i = 1; i < enumerable.Count(); i++)
        //    {
        //        result = GreatestCommonDivisor(enumerable.ElementAt(i), result);

        //        if (result == 1)
        //        {
        //            return 1;
        //        }
        //    }

        //    return result;
        //}

        public void ParseInput()
        {
            _inputs = new List<List<ulong>>();
            var numberOfCases = int.Parse(Console.ReadLine());

            for (var _ = 0; _ < numberOfCases; ++_)
            {
                int size = int.Parse(Console.ReadLine());
                var array = Console.ReadLine().Split(' ').Select(ulong.Parse).ToList();

                if (array.Count != size)
                {
                    throw new Exception();
                }

                _inputs.Add(array);
            }
        }
    }
}
