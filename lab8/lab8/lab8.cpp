#include <iostream>
#include <omp.h>

void task1() {
    std::cout << "-----" << std::endl << "Task 1" << std::endl << "-----" << std::endl;
    int N;
    std::cout << "N: ";
    std::cin >> N;

    int totalSum = 0;
    omp_set_num_threads(2);

#pragma omp parallel reduction(+:totalSum)
    {
        int threadId = omp_get_thread_num();

        if (threadId == 0) {
            for (int i = 1; i <= N / 2; ++i) {
                totalSum += i;
            }
        }
        else if (threadId == 1) {
            for (int i = N / 2 + 1; i <= N; ++i) {
                totalSum += i;
            }
        }

#pragma omp critical
        {
            std::cout << "[" << threadId << "]: Sum = " << totalSum << std::endl;
        }
    }

    std::cout << "Sum = " << totalSum << std::endl;
}

void task2() {
    std::cout << "-----" << std::endl << "Task 2" << std::endl << "-----" << std::endl;
    int N, K;
    std::cout << "N: ";
    std::cin >> N;
    std::cout << "K: ";
    std::cin >> K;

    int totalSum = 0;

    omp_set_num_threads(K);

#pragma omp parallel reduction(+:totalSum)
    {
        int threadId = omp_get_thread_num();
        int batch = (N + K - 1) / K;

        int start = threadId * batch + 1;
        int end = std::min((threadId + 1) * batch, N);

        for (int i = start; i <= end; ++i) {
            totalSum += i;
        }

#pragma omp critical
        {
            std::cout << "[" << threadId << "]: Sum = " << totalSum << std::endl;
        }
    }

    std::cout << "Sum = " << totalSum << std::endl;
}

void task3() {
    std::cout << "-----" << std::endl << "Task 3" << std::endl << "-----" << std::endl;
    int N;
    std::cout << "N: ";
    std::cin >> N;
    int K;
    std::cout << "K: ";
    std::cin >> K;
    int totalSum = 0;

    omp_set_num_threads(K);

#pragma omp parallel reduction(+:totalSum)
    {
        int threadId = omp_get_thread_num();
        int numThreads = omp_get_num_threads();

#pragma omp for
        for (int i = 1; i <= N; ++i) {
            totalSum += i;
        }

#pragma omp critical
        {
            std::cout << "[" << threadId << "]: Sum = " << totalSum << std::endl;
        }

    }

    std::cout << "Sum = " << totalSum << std::endl;

}

void task4() {
    std::cout << "-----" << std::endl << "Task 4" << std::endl << "-----" << std::endl;
    int N;
    std::cout << "N: ";
    std::cin >> N;
    int K;
    std::cout << "K: ";
    std::cin >> K;
    int totalSum = 0;

    omp_set_num_threads(K);

#pragma omp parallel reduction(+:totalSum)
    {
        int threadId = omp_get_thread_num();
        int numThreads = omp_get_num_threads();

#pragma omp for schedule(guided, 2)
        for (int i = 1; i <= N; ++i) {
            totalSum += i;
#pragma omp critical
            std::cout << "[" << threadId << "]: Sum = " << totalSum << ", iteration " << i << std::endl;
        }
    }

    std::cout << "Sum = " << totalSum << std::endl;

}

int main() {
    task1();
    task2();
    task3();
    task4();
}
