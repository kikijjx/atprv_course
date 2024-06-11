using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ATPRV_lab3_3
{
    internal class Program
    {
        static double Integral(double a, double b, ulong n)
        {
            double sum = 0;
            double dx = (b - a) / n;
            for (ulong i = 0; i < n; i++)
            {
                sum += Math.Sin(a + i * dx);
            }
            return sum * dx;
        }

        static double IntegralParallel(double a, double b, ulong n, ulong numThreads)
        {
            double sum = 0;
            double dx = (b - a) / n;

            Thread[] threads = new Thread[numThreads];
            double[] Sums = new double[numThreads];

            for (ulong i = 0; i < numThreads; i++)
            {
                ulong threadNum = i;
                threads[i] = new Thread(() =>
                {
                    double Sum = 0;
                    ulong el = n / numThreads;
                    ulong startIndex = threadNum * el;
                    ulong endIndex = (threadNum == numThreads - 1) ? n : startIndex + el;

                    for (ulong j = startIndex; j < endIndex; j++)
                    {
                        Sum += Math.Sin(a + j * dx);
                    }

                    Sums[threadNum] = Sum;
                });

                threads[i].Start();
            }


            foreach (var thread in threads)
            {
                thread.Join();
            }

            foreach (var i in Sums)
            {
                sum += i;
            }

            return sum * dx;
        }

        static void Main()
        {
            Console.WriteLine("┌-------------┬--------------┬---------------------------------┬----------------------┬---------------┐");

            double a = 0;
            double b = Math.PI;
            double[] epsilons = new double[] { 0.001, 0.0001, 0.00001, 0.000001, 0.0000001, 0.00000001, 0.000000001, 0.0000000001, 0.00000000001, 0.00000000000001  };

            int[] numThreads = new int[] { 1, 2, 4, 8, 12, 16, 20 };
            for (int i = 0; i < epsilons.Length; i++)
            {
                foreach (ulong threads in numThreads)
                {

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    ulong n = 100;
                    double resultPrev = 0;
                    double resultCurrent = Integral(a, b, n);
                    double delta = Math.Abs(resultCurrent - resultPrev);

                    while (delta >= epsilons[i])
                    {
                        //var threadTimeout = TimeSpan.FromSeconds(2000000000);
                        n *= 2;
                        resultPrev = resultCurrent;

                        var task = Task.Run(() => IntegralParallel(a, b, n, threads));
                        //if (!task.Wait(threadTimeout))
                        //{
                        //    return;
                        //}
                        resultCurrent = task.Result;

                        delta = Math.Abs(resultCurrent - resultPrev);
                    }

                    stopwatch.Stop();
                    Console.WriteLine($"| e = {epsilons[i], 7} | потоки = {threads, 3} | интеграл = {resultCurrent, 20} | разбиений = {n, 8} | время = {stopwatch.ElapsedMilliseconds, 5} |");
                }
                Console.WriteLine("├-------------┼--------------┼---------------------------------┼----------------------┼---------------┤");
            }

            Console.ReadKey();
        }
    }
}
