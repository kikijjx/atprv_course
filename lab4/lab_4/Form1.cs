using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab_4
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        Color[,] matrix;
        string actualmethod;
        public Form1()
        {
            InitializeComponent();
            openFileDialog1.Filter = "PNG(*.png)|*.png|JPG(*.jpg)|*.jpg|All files(*.*)|*.*";
            trackBar1.Minimum = 0;
            trackBar1.Maximum = 0;
            label1.Text = "0";
            actualmethod = "один поток";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            System.IO.FileStream fs = new System.IO.FileStream(openFileDialog1.FileName, System.IO.FileMode.Open);
            System.Drawing.Image img = System.Drawing.Image.FromStream(fs);
            fs.Close();
            pictureBox1.Image = img;
            bmp = new Bitmap(img.Width, img.Height);
            bmp = (Bitmap)img;
            matrix = BitmapToMatrix(bmp);
            trackBar1.Maximum = img.Width;
            textBox1.Text += $"Загружена картинка размером {img.Width}x{img.Height}" + Environment.NewLine;
        }
        public Color[,] BitmapToMatrix(Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            Color[,] matrix = new Color[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    matrix[i, j] = bmp.GetPixel(j, i);
                }
            }

            return matrix;
        }
        public Bitmap MatrixToBitmap(Color[,] matrix)
        {
            int height = matrix.GetLength(0);
            int width = matrix.GetLength(1);
            Bitmap bmp = new Bitmap(width, height);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    bmp.SetPixel(j, i, matrix[i, j]);
                }
            }

            return bmp;
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bmp == null) return;
            Color[,] m;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (checkBox1.Checked)
            {
                m = NegativeParallel(matrix);
            }
            else if (checkBox2.Checked)
            {
                m = NegativeThreaded(matrix);
            }
            else
            {
                m = Negative(matrix);
            }
            stopwatch.Stop();
            bmp = MatrixToBitmap(m);
            pictureBox1.Image = bmp;
            textBox1.AppendText($"Негатив ({actualmethod}):" + Environment.NewLine + $"Время: {stopwatch.ElapsedMilliseconds} мс" + Environment.NewLine + "---------------------" + Environment.NewLine);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (bmp == null) return;
            Color[,] m;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (checkBox1.Checked)
            {
                m = MirrorParallel(matrix);
            }
            else if (checkBox2.Checked)
            {
                m = MirrorThreaded(matrix);
            }
            else
            {
                m = Mirror(matrix);
            }
            stopwatch.Stop();
            bmp = MatrixToBitmap(m);
            textBox1.AppendText($"Отражение ({actualmethod}):"  + Environment.NewLine + $"Время: {stopwatch.ElapsedMilliseconds} мс" + Environment.NewLine + "---------------------" + Environment.NewLine);
            pictureBox1.Image = bmp;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (bmp == null || trackBar1.Value == 0) return;
            Color[,] m;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (checkBox1.Checked)
            {
                m = TransferParallel(matrix);
            }
            else if (checkBox2.Checked)
            {
                m = TransferThreaded(matrix);
            }
            else
            {
                m = Transfer(matrix);
            }
            stopwatch.Stop();
            bmp = MatrixToBitmap(m);
            textBox1.AppendText($"Перенос ({actualmethod}):" + Environment.NewLine + $"Время: {stopwatch.ElapsedMilliseconds} мс" + Environment.NewLine + "---------------------" + Environment.NewLine);
            pictureBox1.Image = bmp;

        }

        private void button5_Click(object sender, EventArgs e)
        {
        }

        

        private void Negative()
        {
            int width = bmp.Width;
            int height = bmp.Height;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int r = bmp.GetPixel(i, j).R;
                    int g = bmp.GetPixel(i, j).G;
                    int b = bmp.GetPixel(i, j).B;
                    bmp.SetPixel(i, j, Color.FromArgb(255 - r, 255 - g, 255 - b));
                }
            }
        }

        public Color[,] Negative(Color[,] matrix)
        {
            int height = matrix.GetLength(0);
            int width = matrix.GetLength(1);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int r = 255 - matrix[i, j].R;
                    int g = 255 - matrix[i, j].G;
                    int b = 255 - matrix[i, j].B;
                    matrix[i, j] = Color.FromArgb(r, g, b);
                }
            }

            return matrix;
        }
        private object lockObject = new object();

        private void NegativeParallel()
        {
            int width = bmp.Width;
            int height = bmp.Height;
            Parallel.For(0, width, i =>
            {
                for (int j = 0; j < height; j++)
                {
                    lock (lockObject)
                    {
                        int r = bmp.GetPixel(i, j).R;
                        int g = bmp.GetPixel(i, j).G;
                        int b = bmp.GetPixel(i, j).B;
                        bmp.SetPixel(i, j, Color.FromArgb(255 - r, 255 - g, 255 - b));
                    }
                }
            });
        }
        private void NegativeThreaded()
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int numThreads = 6;
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < numThreads; i++)
            {
                int start = i * height / numThreads;
                int end = (i == numThreads - 1) ? height : (i + 1) * height / numThreads;

                Thread thread = new Thread(() =>
                {
                    for (int j = start; j < end; j++)
                    {
                        for (int k = 0; k < width; k++)
                        {
                            lock (lockObject)
                            {
                                int r = bmp.GetPixel(k, j).R;
                                int g = bmp.GetPixel(k, j).G;
                                int b = bmp.GetPixel(k, j).B;
                                bmp.SetPixel(k, j, Color.FromArgb(255 - r, 255 - g, 255 - b));
                            }
                        }
                    }
                });

                threads.Add(thread);
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }

        private Color[,] NegativeParallel(Color[,] matrix)
        {
            int width = matrix.GetLength(1);
            int height = matrix.GetLength(0);

            Parallel.For(0, width, i =>
            {
                for (int j = 0; j < height; j++)
                {
                    int r = 255 - matrix[j, i].R;
                    int g = 255 - matrix[j, i].G;
                    int b = 255 - matrix[j, i].B;
                    matrix[j, i] = Color.FromArgb(r, g, b);
                }
            });
            return matrix;
        }

        private Color[,] NegativeThreaded(Color[,] matrix)
        {
            int width = matrix.GetLength(1);
            int height = matrix.GetLength(0);
            int numThreads = 6;
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < numThreads; i++)
            {
                int start = i * height / numThreads;
                int end = (i == numThreads - 1) ? height : (i + 1) * height / numThreads;

                Thread thread = new Thread(() =>
                {
                    for (int j = start; j < end; j++)
                    {
                        for (int k = 0; k < width; k++)
                        {
                            int r = 255 - matrix[j, k].R;
                            int g = 255 - matrix[j, k].G;
                            int b = 255 - matrix[j, k].B;
                            matrix[j, k] = Color.FromArgb(r, g, b);
                        }
                    }
                });

                threads.Add(thread);
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            return matrix;
        }


        private void MirrorThreaded()
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int numThreads = 6;
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < numThreads; i++)
            {
                int start = i * width / numThreads;
                int end = (i == numThreads - 1) ? width : (i + 1) * width / numThreads;

                Thread thread = new Thread(() =>
                {
                    for (int k = start; k < end; k++)
                    {
                        for (int j = 0, l = height - 1; j < l; j++, l--)
                        {
                            lock (lockObject)
                            {
                                var t = bmp.GetPixel(k, j);
                                bmp.SetPixel(k, j, bmp.GetPixel(k, l));
                                bmp.SetPixel(k, l, t);
                            }
                        }
                    }
                });

                threads.Add(thread);
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }
        private void TransferThreaded()
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int offset = trackBar1.Value;
            int numThreads = 6;
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < numThreads; i++)
            {
                int start = i * height / numThreads;
                int end = (i == numThreads - 1) ? height : (i + 1) * height / numThreads;

                Thread thread = new Thread(() =>
                {
                    for (int j = start; j < end; j++)
                    {
                        Color[] tmp = new Color[offset];
                        for (int l = 0; l < offset; l++)
                        {
                            lock (lockObject)
                            {
                                tmp[l] = bmp.GetPixel(width - 1 - l, j);
                            }
                        }
                        for (int k = width - 1; k >= offset; --k)
                        {
                            lock (lockObject)
                            {
                                bmp.SetPixel(k, j, bmp.GetPixel(k - offset, j));
                            }
                        }
                        for (int l = 0; l < offset; l++)
                        {
                            lock (lockObject)
                            {
                                bmp.SetPixel(l, j, tmp[l]);
                            }
                        }
                    }
                });

                threads.Add(thread);
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }
        private void Mirror()
        {
            int width = bmp.Width;
            int height = bmp.Height;
            for (int i = 0; i < width; i++)
            {
                for (int k = 0, j = height - 1; k < j; k++, j--)
                {
                    var t = bmp.GetPixel(i, k);
                    bmp.SetPixel(i, k, bmp.GetPixel(i, j));
                    bmp.SetPixel(i, j, t);
                }
            }
        }
        private void MirrorParallel()
        {
            int width = bmp.Width;
            int height = bmp.Height;
            Parallel.For(0, width, i =>
            {
                for (int k = 0, j = height - 1; k < j; k++, j--)
                {
                    lock (lockObject)
                    {
                        var t = bmp.GetPixel(i, k);
                        bmp.SetPixel(i, k, bmp.GetPixel(i, j));
                        bmp.SetPixel(i, j, t);
                    }
                }
            });
        }
        private void Transfer()
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int offset = trackBar1.Value;
            for (int i = 0; i < height; i++)
            {
                Color[] tmp = new Color[offset];
                for (int l = 0; l < offset; l++)
                {
                    tmp[l] = bmp.GetPixel(width - 1 - l, i);
                }
                for (int k = width - 1; k >= offset; --k)
                {
                    bmp.SetPixel(k, i, bmp.GetPixel(k - offset, i));
                }
                for (int l = 0; l < offset; l++)
                {
                    bmp.SetPixel(l, i, tmp[l]);
                }
            }
        }
        private void TransferParallel()
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int offset = trackBar1.Value;
            Parallel.For(0, height, i =>
            {
                Color[] tmp = new Color[offset];
                for (int l = 0; l < offset; l++)
                {
                    lock (lockObject)
                    {
                        tmp[l] = bmp.GetPixel(width - 1 - l, i);
                    }
                }
                for (int k = width - 1; k >= offset; --k)
                {
                    lock (lockObject)
                    {
                        bmp.SetPixel(k, i, bmp.GetPixel(k - offset, i));
                    }
                }
                for (int l = 0; l < offset; l++)
                {
                    lock (lockObject)
                    {
                        bmp.SetPixel(l, i, tmp[l]);
                    }
                }
            });
        }
        private Color[,] Mirror(Color[,] matrix)
        {
            int width = matrix.GetLength(1);
            int height = matrix.GetLength(0);
            Color[,] result = new Color[height, width];

            for (int i = 0; i < width; i++)
            {
                for (int k = 0, j = height - 1; k < j; k++, j--)
                {
                    result[k, i] = matrix[j, i];
                    result[j, i] = matrix[k, i];
                }
            }

            return result;
        }

        private Color[,] MirrorParallel(Color[,] matrix)
        {
            int width = matrix.GetLength(1);
            int height = matrix.GetLength(0);
            Color[,] result = new Color[height, width];

            Parallel.For(0, width, i =>
            {
                for (int k = 0, j = height - 1; k < j; k++, j--)
                {
                    result[k, i] = matrix[j, i];
                    result[j, i] = matrix[k, i];
                }
            });

            return result;
        }

        private Color[,] MirrorThreaded(Color[,] matrix)
        {
            int width = matrix.GetLength(1);
            int height = matrix.GetLength(0);
            int numThreads = 6;
            Color[,] result = new Color[height, width];
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < numThreads; i++)
            {
                int start = i * width / numThreads;
                int end = (i == numThreads - 1) ? width : (i + 1) * width / numThreads;

                Thread thread = new Thread(() =>
                {
                    for (int k = start; k < end; k++)
                    {
                        for (int j = 0, l = height - 1; j < l; j++, l--)
                        {
                            result[j, k] = matrix[l, k];
                            result[l, k] = matrix[j, k];
                        }
                    }
                });

                threads.Add(thread);
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            return result;
        }

        private Color[,] Transfer(Color[,] matrix)
        {
            int offset = trackBar1.Value;
            int width = matrix.GetLength(1);
            int height = matrix.GetLength(0);
            Color[,] result = new Color[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    result[i, j] = matrix[i, (j + offset) % width];
                }
            }

            return result;
        }

        private Color[,] TransferParallel(Color[,] matrix)
        {
            int offset = trackBar1.Value;

            int width = matrix.GetLength(1);
            int height = matrix.GetLength(0);
            Color[,] result = new Color[height, width];

            Parallel.For(0, height, i =>
            {
                for (int j = 0; j < width; j++)
                {
                    result[i, j] = matrix[i, (j + offset) % width];
                }
            });

            return result;
        }

        private Color[,] TransferThreaded(Color[,] matrix)
        {
            int offset = trackBar1.Value;

            int width = matrix.GetLength(1);
            int height = matrix.GetLength(0);
            int numThreads = 6;
            Color[,] result = new Color[height, width];
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < numThreads; i++)
            {
                int start = i * height / numThreads;
                int end = (i == numThreads - 1) ? height : (i + 1) * height / numThreads;

                Thread thread = new Thread(() =>
                {
                    for (int j = start; j < end; j++)
                    {
                        for (int k = 0; k < width; k++)
                        {
                            result[j, k] = matrix[j, (k + offset) % width];
                        }
                    }
                });

                threads.Add(thread);
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            return result;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;
                actualmethod = "Parallel.For";
            }
            else if (!checkBox1.Checked && !checkBox2.Checked)
            {
                actualmethod = "один поток";
            }
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked) 
            { 
                checkBox1.Checked = false;
                actualmethod = "Thread";
            }
            else if (!checkBox1.Checked && !checkBox2.Checked)
            {
                actualmethod = "один поток";
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            if (bmp == null) return;
            textBox1.AppendText("Запуск теста..." + Environment.NewLine);
            string[] methods = { "Негатив", "Негатив (Parallel.For)", "Негатив (Thread)", "Отражение", "Отражение (Parallel.For)", "Отражение (Thread)", "Перенос", "Перенос (Parallel.For)", "Перенос (Thread)" };
            int width = bmp.Width;
            int height = bmp.Height;
            int numRuns = 20;
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Метод\t\tВремя (мс)\tРазмер изображения");
            sb.AppendLine("---------------------------------------------------");
            for (int i = 0; i < methods.Length; i++)
            {
                textBox1.AppendText($"Тест ({i + 1}/9)" + Environment.NewLine);
                long totalTime = 0;

                for (int j = 0; j < numRuns; j++)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    switch (methods[i])
                    {
                        case "Негатив":
                            Negative(matrix);
                            break;
                        case "Негатив (Parallel.For)":
                            NegativeParallel(matrix);
                            break;
                        case "Негатив (Thread)":
                            NegativeThreaded(matrix);
                            break;
                        case "Отражение":
                            Mirror(matrix);
                            break;
                        case "Отражение (Parallel.For)":
                            MirrorParallel(matrix);
                            break;
                        case "Отражение (Thread)":
                            MirrorThreaded(matrix);
                            break;
                        case "Перенос":
                            Transfer(matrix);
                            break;
                        case "Перенос (Parallel.For)":
                            TransferParallel(matrix);
                            break;
                        case "Перенос (Thread)":
                            TransferThreaded(matrix);
                            break;
                    }

                    stopwatch.Stop();
                    totalTime += stopwatch.ElapsedMilliseconds;
                }

                double avgTime = (double)totalTime / numRuns;
                sb.AppendLine($"{methods[i], 25}\t{avgTime, 6:F2}\t{width}x{height}");
            }

            textBox1.AppendText(sb.ToString());
        }

    }
}
