using System;
using System.Collections.Generic;
using System.Linq;
using FileParser;

namespace Codeflows
{
    public enum TaskType { Unknown, One, Two, Three };

    public class Cake
    {
        public double Height { get; set; }

        public Cake(double height)
        {
            Height = height;
        }
    }

    public class Task
    {
        public TaskType Type { get; set; }

        public int L { get; set; }
        public int R { get; set; }
        public int X { get; set; }

        public Task(int taskType)
        {
            Type = (TaskType)taskType;
        }

        public Task(int taskType, int l, int r, int x) : this(taskType)
        {
            L = l;
            R = r;
            X = x;
        }
    }

    public class Cakezoned
    {
        private List<Cake> _cakes;
        private List<Task> _tasks;

        public void Run()
        {
            ParseInput();

            var oddCakes = _cakes.Where((_, index) => index % 2 == 0).ToList();     // Our lists are 0-indexed
            var evenCakes = _cakes.Except(oddCakes);

            var oddTotal = oddCakes.Sum(c => c.Height);
            var evenTotal = evenCakes.Sum(c => c.Height);

            foreach (var task in _tasks)
            {
                switch (task.Type)
                {
                    case TaskType.One:
                        {
                            for (int i = task.L; i <= task.R; ++i)
                            {
                                if (i % 2 == 0)
                                {
                                    evenTotal += task.X;
                                }
                                else
                                {
                                    oddTotal += task.X;
                                }
                            }

                            break;
                        }

                    case TaskType.Two:
                        {
                            Console.WriteLine(oddTotal);
                            break;
                        }

                    case TaskType.Three:
                        {
                            Console.WriteLine(evenTotal);
                            break;
                        }

                    default:
                        {
                            throw new Exception();
                        }
                }
            }
        }

        private void ParseInput()
        {
            _cakes = new List<Cake>();
            _tasks = new List<Task>();

            var numberOfCakes = int.Parse(Console.ReadLine());
            var heights = Console.ReadLine().Split(' ');

            if (heights.Length != numberOfCakes)
            {
                throw new ParsingException();
            }
            _cakes.AddRange(heights.Select(x => new Cake(double.Parse(x))));

            var numberOfTasks = int.Parse(Console.ReadLine());
            for (int i = 1; i <= numberOfTasks; ++i)
            {
                var inputs = Console.ReadLine().Split(' ');
                var type = int.Parse(inputs[0]);
                _tasks.Add(type == 1
                    ? new Task(type, int.Parse(inputs[1]), int.Parse(inputs[2]), int.Parse(inputs[3]))
                    : new Task(type));
            }
        }
    }
}
