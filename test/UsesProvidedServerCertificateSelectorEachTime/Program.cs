using Microsoft.AspNetCore.Server.Kestrel.FunctionalTests;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UsesProvidedServerCertificateSelectorEachTime
{
    class Program
    {
        private static readonly int _tasks = Environment.ProcessorCount * 2;

        private static DateTime[] _lastStarted;
        private static int[] _testsStarted;
        private static int[] _testsCompleted;

        static Program()
        {
            _lastStarted = new DateTime[_tasks];
            _testsStarted = new int[_tasks];
            _testsCompleted = new int[_tasks];
        }

        static void Main(string[] args)
        {
            var tasks = new Task[_tasks];
            for (var i=0; i < _tasks; i++)
            {
                tasks[i] = RunTests(i);
            }

            var timer = new Timer(PrintResults, state: null, dueTime: 0, period: 1000);

            Task.WaitAll(tasks);

            Console.WriteLine("All test tasks completed.  Press any key to exit.");
            Console.ReadLine();
        }
        
        private static async Task RunTests(int index)
        {
            while (true)
            {
                _lastStarted[index] = DateTime.Now;
                _testsStarted[index]++;
                await (new HttpsConnectionAdapterTests()).UsesProvidedServerCertificateSelectorEachTime();
                _testsCompleted[index]++;
            }
        }

        private static void PrintResults(object state)
        {
            Console.Clear();

            Console.SetCursorPosition(0, 0);

            Console.WriteLine($"Current Time: {DateTime.Now}");

            for (var i=0; i < _tasks; i++)
            {
                Console.WriteLine($"[{i}] Started: {_testsStarted[i]}, Completed: {_testsCompleted[i]}, " +
                    $"Running: {_testsStarted[i] - _testsCompleted[i]}, LastStarted: {_lastStarted[i]}");
            }

            Console.WriteLine($"[TOTAL] Started: {_testsStarted.Sum()}, Completed: {_testsCompleted.Sum()}, " +
                $"Running: {_testsStarted.Sum() - _testsCompleted.Sum()}, LastStarted: {_lastStarted.Min()}");
        }
    }
}