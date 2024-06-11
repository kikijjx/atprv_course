using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Faker;

namespace lab6
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            label4.Text = "";

        }

        private void GenerateTexts(int textCount, int wordCount)
        {
            for (int i = 0; i < textCount; i++)
            {
                string text = "";
                for (int j = 0; j < wordCount; j++)
                {
                    text += Faker.TextFaker.Sentence() + "\n";
                }
                string fileName = $"alblak52/text_{i}.txt";
                File.WriteAllText(fileName, text);
            }

            MessageBox.Show($"Сгенерировано {textCount} текстов.");
        }



        private async void button2_Click(object sender, EventArgs e)
        {
            label4.Text = "Выполняется...";
            await Task.Run(() => ProcessTextsAsync());
        }



        public List<string> ReadTextsFromFiles(string folderPath, int filesCount)
        {
            var texts = new List<string>();
            var files = Directory.GetFiles(folderPath).OrderBy(f => Guid.NewGuid()).Take(filesCount);

            foreach (var file in files)
            {
                texts.Add(File.ReadAllText(file));
            }

            return texts;
        }
        public Dictionary<string, int> CountWordsInTexts(List<string> texts, List<string> words)
        {
            var wordsCount = new Dictionary<string, int>();

            foreach (var word in words)
            {
                wordsCount[word] = 0;
            }

            Parallel.ForEach(texts, text =>
            {
                foreach (var word in words)
                {
                    var regex = new Regex($@"\b{word}\b", RegexOptions.IgnoreCase);
                    var matches = regex.Matches(text);

                    lock (wordsCount)
                    {
                        wordsCount[word] += matches.Count;
                    }
                }
            });

            return wordsCount;
        }

        public Dictionary<string, int> Map(string text, List<string> words)
        {
            var wordsCount = new Dictionary<string, int>();

            foreach (var word in words)
            {
                wordsCount[word] = 0;
            }

            foreach (var word in words)
            {
                var regex = new Regex($@"\b{word}\b", RegexOptions.IgnoreCase);
                var matches = regex.Matches(text);

                wordsCount[word] += matches.Count;
            }

            return wordsCount;
        }
        public Dictionary<string, int> Reduce(List<Dictionary<string, int>> results)
        {
            var finalResult = new Dictionary<string, int>();

            foreach (var result in results)
            {
                foreach (var item in result)
                {
                    if (finalResult.ContainsKey(item.Key))
                    {
                        finalResult[item.Key] += item.Value;
                    }
                    else
                    {
                        finalResult[item.Key] = item.Value;
                    }
                }
            }

            return finalResult;
        }
        public Dictionary<string, int> CountWordsInTextsMapReduce(List<string> texts, List<string> words)
        {
            var mappedResults = new ConcurrentBag<Dictionary<string, int>>();

            Parallel.ForEach(texts, text =>
            {
                mappedResults.Add(Map(text, words));
            });

            return Reduce(mappedResults.ToList());
        }


        public void PrintResults(Dictionary<string, int> results, TimeSpan time, string methodName)
        {
            if (textBox2.InvokeRequired)
            {
                textBox2.Invoke((MethodInvoker)delegate {
                    PrintResults(results, time, methodName);
                });
                return;
            }

            textBox2.AppendText($"Метод: {methodName}{Environment.NewLine}");
            textBox2.AppendText($"Время: {time.TotalMilliseconds} мс{Environment.NewLine}");

            foreach (var result in results)
            {
                textBox2.AppendText($"{result.Key}: {result.Value}{Environment.NewLine}");
            }

            textBox2.AppendText(Environment.NewLine);
        }


        public async Task ProcessTextsAsync()
        {
            var folderPath = "alblak52";
            var words = textBox1.Lines
                                .Where(line => !string.IsNullOrWhiteSpace(line))
                                .ToList();
            var filesCount = (int)numericUpDown3.Value;

            var stopwatch = new Stopwatch();

            stopwatch.Restart();
            var texts = ReadTextsFromFiles(folderPath, filesCount);
            var resultsParallel = CountWordsInTexts(texts, words);
            stopwatch.Stop();
            PrintResults(resultsParallel, stopwatch.Elapsed, "Parallel.ForEach");

            stopwatch.Restart();
            var textsMapReduce = ReadTextsFromFiles(folderPath, filesCount);
            var resultsMapReduce = CountWordsInTextsMapReduce(textsMapReduce, words);
            stopwatch.Stop();
            PrintResults(resultsParallel, stopwatch.Elapsed, "MapReduce");

            if (label4.InvokeRequired)
            {
                label4.Invoke((MethodInvoker)delegate {
                    label4.Text = "Готово";
                });
            }
            else
            {
                label4.Text = "Готово";
            }
        }











        private void button1_Click(object sender, EventArgs e)
        {
            GenerateTexts((int)numericUpDown1.Value, (int)numericUpDown2.Value);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void Form1_Shown(object sender, EventArgs e)
        {

        }

    }
}
