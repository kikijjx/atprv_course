using System;
using System.Diagnostics;
using System.Threading;

namespace ATPRV_lab3_2
{
    internal class Program
    {
        static object locker = new object();

        static void Main(string[] args)
        {
            double a = 0;
            double b = Math.PI;

            double[] epsilons = new double[] { 0.001, 0.0001, 0.00001, 0.000001, 0.0000001, 0.00000001, 0.000000001 };
            int[] threadCounts = new int[] { 1, 2, 4, 8, 16, 20 };

            for (int i = 0; i < epsilons.Length; i++)
            {
                double epsilon = epsilons[i];

                for (int j = 0; j < threadCounts.Length; j++)
                {
                    int threadCount = threadCounts[j];
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    double result = Integrate(a, b, epsilon, threadCount);

                    stopwatch.Stop();
                    Console.WriteLine($"e = {epsilon}, количество потоков = {threadCount}, время выполнения = {stopwatch.ElapsedMilliseconds} мс, {result}");
                }
            }

            Console.ReadKey();
        }

        static double Integrate(double a, double b, double epsilon, int threadCount)
        {
            int n = 1;
            double h = (b - a) / n;
            double result = 0;

            while (true)
            {
                double prevResult = result;
                result = 0;

                Thread[] threads = new Thread[threadCount];

                for (int t = 0; t < threadCount; t++)
                {
                    int from = t * n / threadCount;
                    int to = (t == threadCount - 1) ? n : (t + 1) * n / threadCount;

                    threads[t] = new Thread(() =>
                    {
                        double partialResult = 0;

                        for (int i = from; i < to; i++)
                        {
                            double x = a + i * h;
                            partialResult += Math.Sin(x);
                        }

                        lock (locker)
                        {
                            result += partialResult;
                        }
                    });

                    threads[t].Start();
                }

                for (int t = 0; t < threadCount; t++)
                {
                    threads[t].Join();
                }

                result *= h;

                if (Math.Abs(result - prevResult) < epsilon)
                {
                    break;
                }

                n *= 2;
                h = (b - a) / n;
            }

            return result;
        }
    }
}
