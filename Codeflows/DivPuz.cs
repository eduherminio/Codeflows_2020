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
                }


                while (false) // Subtask 1
                              //while (true)
                {
                    var nonCompliant = solution.Where(n => n.Value > 1e18).Select((n, index) => Tuple.Create(n, index)).ToList();
                    if (nonCompliant.Count == 0)
                    {
                        break;
                    }

                    foreach (var pair in nonCompliant)
                    {
                        var i = pair.Item2;

                        int prev = i > 0 ? i - 1 : input.Count - 1;
                        int next = i < input.Count - 1 ? i + 1 : 0;

                        foreach (var factor in PrimeNumbers)
                        {
                            if (solution[i] < factor || solution[i] % factor != 0) continue;

                            var candidate = solution[i] / factor;

                            var prod = solution[prev] * solution[next];     // Overflow that ruins the checks?

                            if (candidate >= input[i] && candidate % input[i] == 0
                                && prod >= candidate && prod % candidate == 0)
                            {
                                solution[i] = candidate;
                                if (solution[i] <= 1e18) break;
                            }
                        }
                    }
                }

                Console.WriteLine(string.Join(" ", solution.OrderBy(p => p.Key).Select(p => p.Value)));
            }
        }

        private static List<ulong> PrimeNumbers = FirstPrimeNumbers().ToList();
        private static IEnumerable<ulong> FirstPrimeNumbers()
        {
            int total = 0;
            ulong n = 2;
            while (total < 100)
            {
                if (isPrime(n))
                {
                    ++total;
                    yield return n;
                }
                n++;
            }
        }

        public static bool isPrime(ulong n)
        {
            ulong x = (ulong)Math.Floor(Math.Sqrt(n));

            if (n == 1) return false;
            if (n == 2) return true;

            for (ulong i = 2; i <= x; ++i)
            {
                if (n % i == 0) return false;
            }

            return true;
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
            var numberOfCases = int.Parse(Console.ReadLine());
            _inputs = new List<List<ulong>>(numberOfCases + 1);

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
