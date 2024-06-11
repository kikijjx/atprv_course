using System;
using System.Threading;

namespace RollerCoaster
{
    class Program
    {
        static void Main(string[] args)
        {
            int passengersCount = 10; // количество пассажиров
            int seatsCount = 4; // количество мест в вагончике

            Semaphore semaphore = new Semaphore(seatsCount, seatsCount);
            Semaphore semaphoreForRide = new Semaphore(0, passengersCount);

            for (int i = 0; i < passengersCount; i++)
            {
                new Thread(Passenger).Start(i);
            }

            new Thread(RollerCoaster).Start(semaphoreForRide);

            Console.ReadLine();
        }

        static void Passenger(object number)
        {
            int passengerNumber = (int)number;

            Console.WriteLine($"Пассажир {passengerNumber + 1} пришел в очередь.");

            semaphore.WaitOne();

            Console.WriteLine($"Пассажир {passengerNumber + 1} сел в вагончик.");

            semaphoreForRide.Release();
        }

        static void RollerCoaster(object semaphoreForRide)
        {
            Semaphore semaphoreForRideLocal = (Semaphore)semaphoreForRide;

            while (true)
            {
                semaphoreForRideLocal.WaitOne();

                Console.WriteLine("Вагончик полный, едем!");

                Thread.Sleep(3000); // имитация движения вагончика

                Console.WriteLine("Вагончик приехал, пассажиры выходят.");

                semaphore.Release(4); // освобождаем все места в вагончике
            }
        }
    }
}
