#include <iostream>
#include <omp.h>
#include <chrono>
#include <cstring>
#include <iomanip>
#include <vector>


void bubbleSort(int* arr, int n) {
    for (int i = 0; i < n - 1; ++i) {
        for (int j = 0; j < n - i - 1; ++j) {
            if (arr[j] > arr[j + 1]) {
                std::swap(arr[j], arr[j + 1]);
            }
        }
    }
}

void parallelBubble(int* arr, int n) {
    bool isSorted = false;

    while (!isSorted) {
        isSorted = true;

#pragma omp parallel for
        for (int i = 1; i < n - 1; i += 2) {
            if (arr[i] > arr[i + 1]) {
                std::swap(arr[i], arr[i + 1]);
                isSorted = false;
            }
        }

#pragma omp parallel for
        for (int i = 0; i < n - 1; i += 2) {
            if (arr[i] > arr[i + 1]) {
                std::swap(arr[i], arr[i + 1]);
                isSorted = false;
            }
        }
    }
}

int partition(int* arr, int left, int right) {
    int pivotIndex = (left + right) / 2;
    int pivot = arr[pivotIndex];

    std::swap(arr[pivotIndex], arr[right]);

    int i = left - 1;
    for (int j = left; j < right; ++j) {
        if (arr[j] < pivot) {
            ++i;
            std::swap(arr[i], arr[j]);
        }
    }
    std::swap(arr[i + 1], arr[right]);
    return i + 1;
}


void quickSort(int* arr, int left, int right) {
    if (left < right) {
        int pivotIndex = partition(arr, left, right);
        quickSort(arr, left, pivotIndex - 1);
        quickSort(arr, pivotIndex + 1, right);
    }
}

void parallelQuickSort(int* arr, int left, int right) {
    if (left < right) {
        int pivotIndex = partition(arr, left, right);

        int size = right - left + 1;
        if (size <= 1000) {
            quickSort(arr, left, pivotIndex - 1);
            quickSort(arr, pivotIndex + 1, right);
        }
        else {
#pragma omp parallel sections
            {
#pragma omp section
                {
                    parallelQuickSort(arr, left, pivotIndex - 1);
                }
#pragma omp section
                {
                    parallelQuickSort(arr, pivotIndex + 1, right);
                }
            }
        }
    }
}


void Merge(std::vector<int>& unsortedArray, int left, int mid, int right) {
    int n1 = mid - left + 1;
    int n2 = right - mid;

    std::vector<int> L(n1), R(n2);

    for (int i = 0; i < n1; i++)
        L[i] = unsortedArray[left + i];
    for (int j = 0; j < n2; j++)
        R[j] = unsortedArray[mid + 1 + j];

    int i = 0, j = 0, k = left;

    while (i < n1 && j < n2) {
        if (L[i] <= R[j]) {
            unsortedArray[k] = L[i];
            i++;
        }
        else {
            unsortedArray[k] = R[j];
            j++;
        }
        k++;
    }

    while (i < n1) {
        unsortedArray[k] = L[i];
        i++; k++;
    }

    while (j < n2) {
        unsortedArray[k] = R[j];
        j++; k++;
    }
}

void LineralMergeSort(std::vector<int>& arr, int left, int right) {
    if (left >= right)
        return;

    int mid = left + (right - left) / 2;

    LineralMergeSort(arr, left, mid);
    LineralMergeSort(arr, mid + 1, right);

    Merge(arr, left, mid, right);
}


void ParallelMergeSort(std::vector<int>& unsortedArray, int left, int right, int threadsNum) {
    if (left >= right)
        return;

    int mid = left + (right - left) / 2;

#pragma omp parallel sections num_threads(threadsNum)
    {
#pragma omp section
        {
            ParallelMergeSort(unsortedArray, left, mid, threadsNum);
        }
#pragma omp section
        {
            ParallelMergeSort(unsortedArray, mid + 1, right, threadsNum);
        }
    }
    Merge(unsortedArray, left, mid, right);
}



void printy(int* arr, int n) {
    std::cout << "[";
    for (int i = 0; i < 10; ++i) {
        std::cout << arr[i];
        std::cout << ", ";
    }
    std::cout << "...]\n";
}

void filly(int* arr, int n) {
    for (int i = 0; i < n; ++i) {
        arr[i] = rand() % 10000;
    }
}

void copyy(int* src, int* dest, int n) {
    std::memcpy(dest, src, n * sizeof(int));
}

int main() {
    int sizes[] = { 10000, 50000, 100000, 150000, 1000000 };
    //int sizesQuick[] = { 1000000, 10000000, 100000000 };
    int threads[] = { 1, 2, 4, 6, 8, 10, 12 };

    for (int size : sizes) {
        int* arr = new int[size];
        int* copy = new int[size];
        filly(arr, size);

        printy(arr, size);


        copyy(arr, copy, size);                                         //
        auto start = std::chrono::high_resolution_clock::now();             //
        bubbleSort(copy, size);                                             //
        auto end = std::chrono::high_resolution_clock::now();               //
        std::chrono::duration<double> diff = end - start;                   //
        std::cout << "BubbleSort " << size << " " << diff.count() << " s\n";//
        printy(copy, size);                                             //


        copyy(arr, copy, size);
        start = std::chrono::high_resolution_clock::now();            //
        quickSort(copy, 0, size - 1);                                      
        end = std::chrono::high_resolution_clock::now();              //
        diff = end - start;             //    
        std::cout << "QuickSort " << size << " " << diff.count() << " s\n";
        printy(copy, size);

        for (int num_threads : threads) {
            omp_set_num_threads(num_threads);

            copyy(arr, copy, size);                                                                    //
            start = std::chrono::high_resolution_clock::now();                                             //
            parallelBubble(copy, size);                                                                    //
            end = std::chrono::high_resolution_clock::now();                                               //
            diff = end - start;                                                                            //
            std::cout << "ParallelBubble " << num_threads << " " << size << " " << diff.count() << " s\n"; //
            printy(copy, size);                                                                        //

            copyy(arr, copy, size);
            start = std::chrono::high_resolution_clock::now();
            parallelQuickSort(copy, 0, size - 1);
            end = std::chrono::high_resolution_clock::now();
            diff = end - start;
            std::cout << "ParallelQuickSort " << num_threads << " " << size << " " << diff.count() << " s\n";
            printy(copy, size);
        }

        delete[] arr;
        delete[] copy;
    }

    //for (int size : sizesQuick) {
    //    int* arr = new int[size];
    //    int* copy = new int[size];
    //    fillArray(arr, size);
    //
    //    printArray(arr, size);
    //
    //    copyArray(arr, copy, size);
    //    auto start = std::chrono::high_resolution_clock::now();
    //    quickSort(copy, 0, size - 1);
    //    auto end = std::chrono::high_resolution_clock::now();
    //    std::chrono::duration<double> diff = end - start;
    //    std::cout << "QuickSort " << size << " " << diff.count() << " s\n";
    //    printArray(copy, size);
    //
    //    for (int num_threads : threads) {
    //        omp_set_num_threads(num_threads);
    //
    //        copyArray(arr, copy, size);
    //        start = std::chrono::high_resolution_clock::now();
    //        parallelQuickSort(copy, 0, size - 1);
    //        end = std::chrono::high_resolution_clock::now();
    //        diff = end - start;
    //        std::cout << "ParallelQuickSort " << num_threads << " " << size << " " << diff.count() << " s\n";
    //        printArray(copy, size);
    //    }
    //
    //    delete[] arr;
    //    delete[] copy;
    //}

    return 0;
}
