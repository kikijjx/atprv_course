using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPRV_lab2_bogougodno
{
    internal class Program
    {
        static int Size = 3;
        static void Main(string[] args)
        {
            //    //WriteMatrixToBinaryFile(GenerateRandomMatrix(Size), "binaryMatrix1");     // Генерируем бинарный файл с первой матрицей
            //    WriteBinaryFileOnConsole("binaryMatrix1");                                  // Вывод матрицы на консоль
            //    //WriteMatrixToBinaryFile(GenerateRandomMatrix(Size), "binaryMatrix2");     // Генерируем бинарный файл со второй матрицей
            //    WriteBinaryFileOnConsole("binaryMatrix2");                                  // Вывод матрицы на консоль
            //    int[][] array = ReadBinaryFileAndFillArray("binaryMatrix1");                // Записываем массив строк первой матрицы
            //    ResultMatrixInFile(array, "binaryMatrix2");                                 // Записываем результатирующую матрицу в файл
            //    WriteBinaryFileOnConsole("resultMatrix");                                   // Выводим результирующую матрицу на консоль
            //    Console.ReadKey();



            for (int j = 3; j < 82; j += 3)
            {
                Size = j;

                WriteMatrixToBinaryFile(GenerateRandomMatrix(Size), "binaryMatrix1");     // Генерируем бинарный файл с первой матрицей
                WriteBinaryFileOnConsole("binaryMatrix1");                                  // Вывод матрицы на консоль
                WriteMatrixToBinaryFile(GenerateRandomMatrix(Size), "binaryMatrix2");     // Генерируем бинарный файл со второй матрицей
                WriteBinaryFileOnConsole("binaryMatrix2");                                  // Вывод матрицы на консоль
                int[][] array1 = ReadBinaryFileAndFillArray("binaryMatrix1");                // Записываем массив строк первой матрицы

                Stopwatch stopwatch = new Stopwatch();
                Console.WriteLine($"Размерность матрицы: {Size}");
                // стр*столб
                stopwatch.Restart();
                for (int i = 0; i < 10; i++) ResultMatrixInFileStrCol(array1, "binaryMatrix2");
                stopwatch.Stop();
                Console.WriteLine($"стр*столб: {stopwatch.ElapsedMilliseconds / 10} милисекунд");

                // стр*стр
                stopwatch.Restart();
                for (int i = 0; i < 10; i++) ResultMatrixInFileStrStr(array1, "binaryMatrix2");
                stopwatch.Stop();
                Console.WriteLine($"стр*стр: {stopwatch.ElapsedMilliseconds / 10} милисекунд");

                // столб*столб
                stopwatch.Restart();
                for (int i = 0; i < 10; i++) ResultMatrixInFileColCol(array1, "binaryMatrix2");
                stopwatch.Stop();
                Console.WriteLine($"столб*столб: {stopwatch.ElapsedMilliseconds / 10} милисекунд");

                // столб*стр
                stopwatch.Restart();
                for (int i = 0; i < 10; i++) ResultMatrixInFileColStr(array1, "binaryMatrix2");
                stopwatch.Stop();
                Console.WriteLine($"столб*стр: {stopwatch.ElapsedMilliseconds / 10} милисекунд");


                TransposeMatrixInFile("binaryMatrix2");
                // столб*стр(Т)
                stopwatch.Restart();
                for (int i = 0; i < 10; i++) ResultMatrixInFileStrCol(array1, "binaryMatrix2");
                stopwatch.Stop();
                Console.WriteLine($"стр*столб(Т): {stopwatch.ElapsedMilliseconds / 10} милисекунд");

                // стр*стр(Т)
                stopwatch.Restart();
                for (int i = 0; i < 10; i++) ResultMatrixInFileStrStr(array1, "binaryMatrix2");
                stopwatch.Stop();
                Console.WriteLine($"стр*стр(Т): {stopwatch.ElapsedMilliseconds / 10} милисекунд");

                // столб*столб(Т)
                stopwatch.Restart();
                for (int i = 0; i < 10; i++) ResultMatrixInFileColCol(array1, "binaryMatrix2");
                stopwatch.Stop();
                Console.WriteLine($"столб*столб(Т): {stopwatch.ElapsedMilliseconds / 10} милисекунд");

                // столб*стр(Т)
                stopwatch.Restart();
                for (int i = 0; i < 10; i++) ResultMatrixInFileColStr(array1, "binaryMatrix2");
                stopwatch.Stop();
                Console.WriteLine($"столб*стр(Т): {stopwatch.ElapsedMilliseconds / 10} милисекунд");

                TransposeMatrixInFile("binaryMatrix2");
            }


            Console.ReadKey();

        }

        static public int[,] GenerateRandomMatrix(int size) // Метод генерации матрицы размера Size и заполнения ее случайными элементами
        {
            Random rnd = new Random();
            int[,] matrix = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = rnd.Next(0, 10);
                }
            }
            return matrix;
        }


        static public void WriteBinaryFileOnConsole(string filePath)    // Метод вывода записанного инарного файла на консоль
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    int elems = Size;
                    for (int i = 0; i < elems; i++)
                    {
                        for (int j = 0; j < elems; j++)
                        {
                            Console.Write(binaryReader.ReadInt32() + " ");
                        }
                        Console.WriteLine();
                    }
                }
            }
            Console.WriteLine();
        }

        static void WriteMatrixToBinaryFile(int[,] matrix, string filePath) // Метод записи матрицы в бинарный файл
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        writer.Write(matrix[i, j]);
                    }
                }
            }
        }

        static public int[][] ReadBinaryFileAndFillArray(string filePath)   // Считываем матрицу из бинарного файла и записываем в массив размера Size
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    int elems = Size;
                    int[][] array = new int[elems][];

                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = new int[elems];
                        for (int j = 0; j < array.Length; j++)
                        {
                            array[i][j] = binaryReader.ReadInt32();
                        }
                    }
                    return array;
                }
            }
        }

        static void ResultMatrixInFile(int[][] rowArray, string filePath)    // Записываем результирующую матрицу в файл
        {
            int[,] resultArray = new int[Size, Size];
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                int elems = Size;
                int[,] array = new int[elems, elems];
                for (int i = 0; i < elems; i++)
                {
                    for (int j = 0; j < elems; j++)
                    {
                        array[i, j] = reader.ReadInt32();
                    }
                }
                for (int j = 0; j < elems; j++)
                {
                    for (int i = 0; i < elems; i++)
                    {
                        resultArray[i, j] = 0;
                        for (int k = 0; k < elems; k++)
                        {
                            resultArray[i, j] += rowArray[i][k] * array[k, j];
                        }
                    }
                }
                WriteMatrixToBinaryFile(resultArray, "resultMatrix");
            }
        }


        static void ResultMatrixInFileStrCol(int[][] rowArray, string filePath)    // Записываем результирующую матрицу в файл (стр*столб)
        {
            int[,] resultArray = new int[Size, Size];
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                int elems = Size;
                int[,] array = new int[elems, elems];
                for (int i = 0; i < elems; i++)
                {
                    for (int j = 0; j < elems; j++)
                    {
                        array[i, j] = reader.ReadInt32();
                    }
                }
                for (int j = 0; j < elems; j++)
                {
                    for (int i = 0; i < elems; i++)
                    {
                        resultArray[i, j] = 0;
                        for (int k = 0; k < elems; k++)
                        {
                            resultArray[i, j] += rowArray[i][k] * array[k, j];
                        }
                    }
                }
                WriteMatrixToBinaryFile(resultArray, "resultMatrixStrCol.bin");
            }
        }

        static void ResultMatrixInFileStrStr(int[][] rowArray, string filePath)    // Записываем результирующую матрицу в файл (стр*стр)
        {
            int[,] resultArray = new int[Size, Size];
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                int elems = Size;
                int[,] array = new int[elems, elems];
                for (int i = 0; i < elems; i++)
                {
                    for (int j = 0; j < elems; j++)
                    {
                        array[i, j] = reader.ReadInt32();
                    }
                }
                for (int j = 0; j < elems; j++)
                {
                    for (int i = 0; i < elems; i++)
                    {
                        resultArray[i, j] = 0;
                        for (int k = 0; k < elems; k++)
                        {
                            resultArray[i, j] += rowArray[i][k] * array[k, j];
                        }
                    }
                }
                WriteMatrixToBinaryFile(resultArray, "resultMatrixStrStr.bin");
            }
        }


        static void ResultMatrixInFileColCol(int[][] rowArray, string filePath)    // Записываем результирующую матрицу в файл (столб*столб)
        {
            int[,] resultArray = new int[Size, Size];
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                int elems = Size;
                int[,] array = new int[elems, elems];
                for (int i = 0; i < elems; i++)
                {
                    for (int j = 0; j < elems; j++)
                    {
                        array[i, j] = reader.ReadInt32();
                    }
                }
                for (int i = 0; i < elems; i++)
                {
                    for (int k = 0; k < elems; k++)
                    {
                        resultArray[i, k] = 0;
                        for (int j = 0; j < elems; j++)
                        {
                            resultArray[i, k] += rowArray[i][j] * array[j, k];
                        }
                    }
                }
                WriteMatrixToBinaryFile(resultArray, "resultMatrixColCol.bin");
            }
        }

        static void ResultMatrixInFileColStr(int[][] rowArray, string filePath)    // Записываем результирующую матрицу в файл (столб*стр)
        {
            int[,] resultArray = new int[Size, Size];
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                int elems = Size;
                int[,] array = new int[elems, elems];
                for (int i = 0; i < elems; i++)
                {
                    for (int j = 0; j < elems; j++)
                    {
                        array[i, j] = reader.ReadInt32();
                    }
                }
                for (int i = 0; i < elems; i++)
                {
                    for (int j = 0; j < elems; j++)
                    {
                        resultArray[i, j] = 0;
                        for (int k = 0; k < elems; k++)
                        {
                            resultArray[i, j] += rowArray[k][i] * array[k, j];
                        }
                    }
                }
                WriteMatrixToBinaryFile(resultArray, "resultMatrixColStr.bin");
            }
        }

        static public void TransposeMatrixInFile(string filePath)
        {
            int[][] matrix = ReadBinaryFileAndFillArray(filePath);
            int[,] transposedMatrix = new int[Size, Size];

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    transposedMatrix[j, i] = matrix[i][j];
                }
            }

            WriteMatrixToBinaryFile(transposedMatrix, filePath);
        }


    }
}
