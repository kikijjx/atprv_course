using Accord.Math;
using Accord.Statistics.Testing;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System;
using System.Runtime.InteropServices;
using System.IO;


int Fun(int M, int s, int N)
{
    int[,] a = new int[N, N];
    DateTime t1, t2;
    TimeSpan[] times = new TimeSpan[M];
    for (int k = 0; k < M; k++)
    {
        switch (s)
        {
            case 0:
                t1 = DateTime.Now;
                for (int j = 0; j < N; j++)
                {
                    for (int i = 0; i < N; i++)
                    {
                        a[j, i] = i / (j + 1);
                    }
                }
                t2 = DateTime.Now;
                var dt = t2 - t1;
                times[k] = dt;
                break;
            case 1:
                t1 = DateTime.Now;
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        a[j, i] = i / (j + 1);
                    }
                }
                t2 = DateTime.Now;
                dt = t2 - t1;
                times[k] = dt;
                break;
            case 2:
                t1 = DateTime.Now;
                for (int i = N - 1; i > 0; i--)
                {
                    for (int j = N - 1; j > 0; j--)
                    {
                        a[i, j] = i / (j + 1);
                    }
                }
                t2 = DateTime.Now;
                dt = t2 - t1;
                times[k] = dt;
                break;
            case 3:
                t1 = DateTime.Now;
                for (int j = N - 1; j > 0; j--)
                {
                    for (int i = N - 1; i > 0; i--)
                    {
                        a[j, i] = i / (j + 1);
                    }
                }
                t2 = DateTime.Now;
                dt = t2 - t1;
                times[k] = dt;
                break;
        }
        if (k % 10 == 0)
        {
            //Console.WriteLine(k);
        }
    }
    int avg = times.Sum(t => t.Milliseconds) / times.Length;
    int min = times.Min(t => t.Milliseconds);
    int max = times.Max(t => t.Milliseconds);
    double[] disArr = new double[M];
    for (int i = 0; i < M; i++)
    {
        int temp = Convert.ToInt32(times[i].Milliseconds);
        disArr[i] = Math.Pow((avg - temp), 2);
    }
    double dis = disArr.Sum() / M;
    double mse = Math.Sqrt(dis);
    double leftBorder = avg - mse;
    double rightBorder = avg + mse;
    double[] deviation = new double[M];
    for (int i = 0; i < M; i++)
    {
        deviation[i] = times[i].Milliseconds;
    }
    var res = new ShapiroWilkTest(deviation);
    bool resultTesting;
    if (res.PValue > 0.05) resultTesting = true;
    else resultTesting = false;
    Console.WriteLine("avg {0}\nmin {1}\nmax {2}\ndis {3}\nmse {4}\ninterval ot {5} do {6}\nnormality test {7}"
        , avg, min, max, dis, mse, leftBorder, rightBorder, resultTesting);




    return Convert.ToInt32(avg);
}


static void LinearLeastSquares(int[] x, int[] y, out double a, out double b)
{
    if (x.Length != y.Length || x.Length <= 1)
        throw new Exception();
    double a11 = 0.0, a12 = 0.0, a22 = x.Length, b1 = 0.0, b2 = 0.0;
    for (int i = 0; i < x.Length; i++)
    {
        a11 += x[i] * x[i];
        a12 += x[i];
        b1 += x[i] * y[i];
        b2 += y[i];
    }
    double det = a11 * a22 - a12 * a12;
    if (Math.Abs(det) < 1e-17)
        throw new Exception();
    a = (b1 * a22 - a12 * b2) / det;
    b = (a11 * b2 - b1 * a12) / det;
}




int M = 10; //1000
int c = 4;
//int N = 1000;

//var times = new int[15,4];
for (int i = 0; i < c; i++)
{
    var times = new int[15];
    var xs = new int[15];
    Console.WriteLine("---------------");
    Console.WriteLine($"Способ: {i+1}");
    for (int N = 1000; N < 16000; N += 1000)
    {
        Console.WriteLine("---------------");
        Console.WriteLine($"Размерность: {N}");
        //times[N/1000 - 1, i] = Fun(M, i, N);
        times[N/1000 - 1] = Fun(M, i, N);
        xs[N / 1000 - 1] = N;
    }
    int width = 800;
    int height = 400;
    double a, b;
    double aQuad, bQuad, cQuad;
    LinearLeastSquares(xs, times, out a, out b);

    using (Bitmap bitmap = new Bitmap(width, height))
    {
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.White);
            using (Brush brush = new SolidBrush(Color.Black))
            {
                graphics.DrawLine(Pens.Black, 50, height - 50, 50, 50);
                graphics.DrawLine(Pens.Black, 50, height - 50, width - 50, height - 50);
                for (int x = 1; x <= 15; x++)
                {
                    graphics.DrawString((x * 1000).ToString(), new Font("Arial", 8), brush, x * (width - 100) / 15, height - 30);
                }
                int maxValue = Enumerable.Range(0, times.GetLength(0))
                    .Select(x => times[x])
                    .Max();
                float columnWidth = (float)(width - 100) / 15;
                //for (int l = 0; l < 15; l++)
                //{
                //    float x = l * columnWidth + 50;
                //    float y = height - (height - 100) * times[l] / maxValue;
                //    float columnHeight = (height - 100) * times[l] / maxValue;
                //    graphics.DrawRectangle(Pens.Blue, x, y - 51, columnWidth, columnHeight);
                //    graphics.DrawString(times[l].ToString(), new Font("Arial", 8), brush, x, y - 70);
                //}

                for (int l = 1; l < 15; l++)
                {
                    float x1 = (l - 1) * columnWidth + 50;
                    float y1 = height - (height - 100) * times[l - 1] / maxValue;
                    float x2 = l * columnWidth + 50;
                    float y2 = height - (height - 100) * times[l] / maxValue;
                    graphics.DrawLine(Pens.Blue, x1, y1 - 51, x2, y2 - 51);
                    graphics.DrawString(times[l - 1].ToString(), new Font("Arial", 8), brush, x1 - 10, y1 - 65);
                    graphics.DrawString(times[l].ToString(), new Font("Arial", 8), brush, x2 - 10, y2 - 65);
                }


                float x11 = xs[0];
                float y11 = (float)a * x11 + (float)b;
                float x22 = xs[xs.Length - 1];
                float y22 = (float)a * x22 + (float)b;

                x11 = 50 + (x11 - xs[0]) * (width - 100) / (xs[xs.Length - 1] - xs[0]);
                x22 = 50 + (x22 - xs[0]) * (width - 100) / (xs[xs.Length - 1] - xs[0]);
                y11 = height - (50 + (y11 - times.Min()) * (height - 100) / (maxValue - times.Min()));
                y22 = height - (50 + (y22 - times.Min()) * (height - 100) / (maxValue - times.Min()));

                graphics.DrawLine(Pens.Red, x11, y11, x22, y22);


            }
        }
        bitmap.Save($"graphic{i + 1}.png", System.Drawing.Imaging.ImageFormat.Png);
    }

}
Console.WriteLine("");