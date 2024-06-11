using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;

namespace ATPRV_lab3
{
    internal class Program
    {
        static Random rnd = new Random();
        static object locker = new object();

        static void Main(string[] args)
        {
            int[] sizes = new int[] { 1000, 5000, 10000, 15000, 20000 };
            //int[] sizes = new int[] { 10 };
            int[] threadCounts = new int[] { 1, 2, 4,6, 8, 12, 16, 20 };

            for (int i = 0; i < sizes.Length; i++)
            {
                int N = sizes[i];
                double[] a = new double[N];
                double[] b = new double[N];

                for (int k = 0; k < a.Length; k++)
                {
                    a[k] = rnd.Next(1, 100);
                }

                for (int j = 0; j < threadCounts.Length; j++)
                {
                    int threadCount = threadCounts[j];
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    Thread[] threads = new Thread[threadCount];

                    int batchSize = N / threadCount;
                    for (int t = 0; t < threadCount; t++)
                    {
                        int from = t * batchSize;
                        int to = (t == threadCount - 1) ? N : (t + 1) * batchSize;
                        threads[t] = new Thread(() => getB(a, b, from, to));
                        threads[t].Start();
                    }

                    for (int t = 0; t < threadCount; t++)
                    {
                        threads[t].Join();
                    }

                    stopwatch.Stop();
                    Console.WriteLine($"Размер массива: {N}, количество потоков: {threadCount}, время выполнения: {stopwatch.ElapsedMilliseconds} мс");
                }
            }


            for (int i = 0; i < sizes.Length; i++)
            {
                int N = sizes[i];
                double[] a = new double[N];
                double[] b = new double[N];

                for (int k = 0; k < a.Length; k++)
                {
                    a[k] = rnd.Next(1, 3);
                    //Console.Write(a[k] + " ");
                }
                Console.WriteLine();

                for (int j = 0; j < threadCounts.Length; j++)
                {
                    int threadCount = threadCounts[j];
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    Thread[] threads = new Thread[threadCount];

                    int batchSize = N / threadCount;
                    for (int t = 0; t < threadCount; t++)
                    {
                        int from = t * batchSize;
                        int to = (t == threadCount - 1) ? N : (t + 1) * batchSize;
                        threads[t] = new Thread(() => getBParallel(a, b, from, to, locker));
                        threads[t].Start();
                    }

                    for (int t = 0; t < threadCount; t++)
                    {
                        threads[t].Join();
                    }

                    stopwatch.Stop();
                    Console.WriteLine($"Размер массива: {N}, количество потоков: {threadCount}, время выполнения: {stopwatch.ElapsedMilliseconds} мс");
                    //ReturnB(b);
                }
            }

            Console.ReadKey();
        }

        static void getB(double[] a, double[] b, int from, int to)
        {
            for (int i = from; i < to; i++)
            {
                for (int j = 0; j < i + 1; j++)
                {
                    b[i] += Math.Pow(a[j], 1.789);
                }
            }
        }
        static void getBParallel(double[] a, double[] b, int from, int to, object locker)
        {
            //for (int i = from; i < to; i++)
            //{
            //    lock (locker)
            //    {
            //        for (int j = 0; j < i + 1; j++)
            //        {
            //            b[i] += Math.Pow(a[j], 1.789);
            //        }
            //    }
            //}

                for (int i = 0; i <= from; i++)
                {
                    b[from] += Math.Pow(a[i], 1.789);
                }
                
                for (int i = from + 1; i < to; i++)
                {
                    b[i] = b[i - 1] + Math.Pow(a[i], 1.789);
                }
            
        }

        static void ReturnB(double[] b)
        {
            for(int i = 0; i < b.Length; i++)
            {

                Console.Write(b[i] + " ");
            }
            Console.WriteLine();
        }
    }
}
